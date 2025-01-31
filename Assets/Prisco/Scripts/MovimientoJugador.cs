using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MovimientoJugador : MonoBehaviour
{
    // Variables del player
    [Header("Player Settings")]
    [SerializeField] public static float speed = 15.0f;
    public static float fuerzaEmpujeAL2 = 3.0f;  
    public static float fuerzaEmpujeAP3 = 100.0f;  
    public  float rotationSpeed = 10.0f;
    public float gravity = 20.0f;
    public static bool hasAttacked = false;
    public static bool hasRotated = false;
    public static MovimientoJugador instance; 

    public DamageDealer damageDealerL1;    
    public DamageDealer damageDealerL2;     
    public DamageDealer damageDealerL3;     
    public DamageDealer damageDealerP; 

    
      

    [Header("Movement Settings")]
    private Rigidbody controller;     
    bool canMove = true; 
    bool isLookingAtTarget = false;           
    Vector3 targetPosition;

    [Header("Attack Settings")]     
    public static bool ataqueL = false; 
    public static bool ataqueP = false; 
    public static bool ataqueD = false;  
    public Collider ataqueCollider;
    public MeshRenderer ataqueMesh;
    public float rangoMaximo = 10.0f;
    public float velocidadExpansion = 2.0f;
    private bool atacando = false; 
    private Coroutine mirarCoroutine; 
    public float tiempoEsperaAtaque = 3.0f; 
    private float tiempoUltimoAtaque;   

    [Header("Distance Settings")]  
    public GameObject prefab;
    public Transform spawnPosition;
    public int maxBalas = 3;
    public int balasActuales = 3;
    public float tiempoEntreDisparos = 0.5f;
    private float tiempoUltimoDisparo = -Mathf.Infinity;
    public float tiempoRecarga = 2.0f;
    private float tiempoUltimaRecarga = -Mathf.Infinity;    
    public Slider[] balaSliders;
    public Color fullColor = Color.green;
    public Color emptyColor = Color.red;

    [Header("Animation Settings")]
    public Animator animator;    
    public static bool enterAttack = false;
    public bool verificarArma = false;
    public static bool cambioarma = false;       

    [Header("Visual Settings")]
    private Color originalColor; 
    public Renderer cambioColoAlPegar; 

    [Header("Dash Settings")]
    public static bool isDashing = false;
    private bool canDash = true;
    private float dashTime;

    public GameObject dashObjec;     
    public float dashSpeed = 20.0f; // Velocidad del dash
    public float dashDuration = 0.2f; // Duración del dash en segundos
    public float dashCooldown = 1.0f; // Tiempo de recarga del dash en segundos
    [Header("Bullet time")]
    public float bulletTimeDuration = 6f;
    public bool bulletTime = false;
    public TestAfterImage afterImageEffect;
    public static float bulletTimeScale = 1f;
    public float slowedBulletTimeScale = 0.05f;
    public float bulletTimeCooldown = 5f;
    private float lastBulletTimeUse = -Mathf.Infinity;
    public Image bulletTimeCooldownImage;

    [Header("Fall")]     
    private Queue<Vector3> safePositions = new Queue<Vector3>();
    private bool isGrounded;

    [Header("Sounds")]     
    [SerializeField] private FMODUnity.EventReference bulletTimeStart;
    [SerializeField] private FMODUnity.EventReference bulletTimeEnd;
    [SerializeField] private FMODUnity.EventReference shot;
    [SerializeField] private FMODUnity.EventReference heavy;
    [SerializeField] private FMODUnity.EventReference dash;

    public static bool isInDodgeArea = false;
    public static bool godMode = false;

    #region *Bertadas
    public void speedUp()
    {
        speed = +5f;
    }

        public void speedDown()
    {
        speed = -5f;
    }

    #endregion Bertadas

    #region Fall
    
    private void OnTriggerEnter(Collider other)
    {
/*         if (other.CompareTag("DodgeArea"))
        {
            isInDodgeArea = true;
            Debug.Log("Dodge area entered");
        } */

        if (other.CompareTag("FallZone"))
        {
            Debug.Log("Caída detectada, teletransportando a la última posición segura.");
            StartCoroutine(TeleportToLastSafePosition());
        }
    }

    private IEnumerator UpdateSafePosition()
    {
        while (true)
        {
            if (isGrounded)
            {
                if (safePositions.Count >= 5)
                {
                    safePositions.Dequeue();
                }
                safePositions.Enqueue(transform.position);
            }
            yield return new WaitForSeconds(0.15f);
        }
    }

    private IEnumerator TeleportToLastSafePosition()
    {
        //controller.enabled = false;

        // Teletransportar a la posición más antigua
        transform.position = safePositions.Peek();

        yield return null;

        //controller.enabled = true;
    }
    #endregion Fall

    #region BulletTime
    private void OnTriggerExit(Collider other)
    {
        //ForceTriggerExit (other);
        if (other.CompareTag("DodgeArea"))
        {
            isInDodgeArea = false;
            Debug.Log("Dodge area exited");
        }
    }

/*
    public void ForceTriggerExit (Collider other)
    {
        if (other.CompareTag("DodgeArea"))
        {
            isInDodgeArea = false;
            Debug.Log("Dodge area exited");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger stay " + other.gameObject.name);
        if (other.CompareTag("DodgeArea") && Input.GetKeyDown(KeyCode.Space)  && !bulletTime)
        {
            if (afterImageEffect != null)
            {
                StartCoroutine(ActivateBulletTime());
            }
            else
            {
                Debug.LogError("afterImageEffect no está asignado en el Inspector.");
            }
        }
    } */

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DodgeArea") && !bulletTime)
        {
            Debug.Log("Trigger stay " + other.gameObject);
            isInDodgeArea = true;
        }
    }
    private IEnumerator ActivateBulletTime()
    {
        FMODUnity.RuntimeManager.PlayOneShot(bulletTimeStart);
        bulletTime = true;
        afterImageEffect.enabled = true;
        bulletTimeScale = slowedBulletTimeScale;
        DefaultHUD.Instance.EnableBulletTimeUI();
        //Debug.Log("Bullet time on, time scale = " + bulletTimeScale);

        yield return new WaitForSeconds(bulletTimeDuration - 0.3f);

        FMODUnity.RuntimeManager.PlayOneShot(bulletTimeEnd);

        yield return new WaitForSeconds(0.3f);

        bulletTime = false;
        afterImageEffect.enabled = false;
        bulletTimeScale = 1f;
        DefaultHUD.Instance.DisableBulletTimeUI();
        //Debug.Log("Bullet time off, time scale = " + bulletTimeScale);

        lastBulletTimeUse = Time.time;
    }

    public static event Action OnBulletTimeEnd;

    public static void EndBulletTime()
    {
        bulletTimeScale = 1f;
        OnBulletTimeEnd?.Invoke();
    }

    public bool IsBulletTimeActive()
    {
        return bulletTime;
    }

    #endregion BulletTime

    #region *Priscada
   void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        controller = GetComponent<Rigidbody>();      
        instance = GetComponent<MovimientoJugador>();
        animator = GetComponent<Animator>();   
        tiempoUltimoAtaque = -tiempoEsperaAtaque;

        for (int i = 0; i < 5; i++)
        {
            safePositions.Enqueue(transform.position);
        }

        StartCoroutine(UpdateSafePosition());
    }

  void Update()
{
    if (Input.GetKeyDown(KeyCode.G))
    {
        godMode = !godMode;
        Debug.Log("God Mode: " + (godMode ? "Activated" : "Deactivated"));
        rangoMaximo = 999;
    }

    if (Input.GetKeyDown(KeyCode.O))
    {
        enterAttack = true;
    }
    if (Input.GetKeyDown(KeyCode.P))
    {
        enterAttack = false;
    }

    if (isInDodgeArea && Input.GetKeyDown(KeyCode.Space) && !bulletTime)
    {
        if (Time.time >= lastBulletTimeUse + bulletTimeCooldown && canDash && !isDashing )
        {
            if (afterImageEffect != null)
            {
                StartCoroutine(ActivateBulletTime());
                StartCoroutine(Dash());
            }
            else
            {
                Debug.LogError("afterImageEffect no está asignado en el Inspector.");
            }
        }
    }

    if (canMove)
    {
        Movimientojugador();
    }

    if (!isInDodgeArea && Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing)
    {
        StartCoroutine(Dash());
    }

    AtaqueJugador();
    AtaqueLigero();
    AtaquePesado();

    if (atacando)
    {
        ExpandirCollider();
    }

    AtaqueDistancia();
    RecargarBalas();
    UpdateBalaSliders();

    if (bulletTimeCooldownImage != null)
    {
        float cooldownProgress = Mathf.Clamp01((Time.time - lastBulletTimeUse) / bulletTimeCooldown);
        bulletTimeCooldownImage.fillAmount = cooldownProgress;

        bulletTimeCooldownImage.gameObject.SetActive(cooldownProgress < 1);
    }

    isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

    if (isGrounded)
    {
        //Debug.Log("is grounded");
    }
}

public void Movimientojugador()
{
    if (canMove)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveInput = new Vector3(horizontal, 0.0f, vertical);

        if ((horizontal != 0 || vertical != 0) && !enterAttack)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }

        Vector3 moveDirection = moveInput * speed;

        if (canMove || enterAttack)
        {
           Vector3 newPosition = controller.position + moveInput * speed * Time.deltaTime;
            controller.MovePosition(newPosition);
        }

        if (moveDirection != Vector3.zero && !isLookingAtTarget && !enterAttack)
        {
            Quaternion targetRotation = Quaternion.identity;

            switch (vertical)
            {
                case 1: // W 
                    switch (horizontal)
                    {
                        case 1: // D 
                            targetRotation = Quaternion.Euler(0, 45, 0);
                            break;
                        case -1: // A 
                            targetRotation = Quaternion.Euler(0, -45, 0);
                            break;
                        default:
                            targetRotation = Quaternion.Euler(0, 0, 0);
                            break;
                    }
                    break;
                case -1: // S 
                    switch (horizontal)
                    {
                        case 1: // D 
                            targetRotation = Quaternion.Euler(0, 135, 0);
                            break;
                        case -1: // A 
                            targetRotation = Quaternion.Euler(0, -135, 0);
                            break;
                        default:
                            targetRotation = Quaternion.Euler(0, 180, 0);
                            break;
                    }
                    break;
                default:
                    switch (horizontal)
                    {
                        case 1: // D 
                            targetRotation = Quaternion.Euler(0, 90, 0);
                            break;
                        case -1: // A 
                            targetRotation = Quaternion.Euler(0, -90, 0);
                            break;
                        default:
                            targetRotation = transform.rotation;
                            break;
                    }
                    break;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else if (targetPosition != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
public IEnumerator EmpujarJugadorAL2(Vector3 direccion, float duracion)
{
    float tiempoTranscurrido = 0;
    while (tiempoTranscurrido < duracion)
    {
        controller.MovePosition(transform.position + direccion * (Time.deltaTime / duracion));
        tiempoTranscurrido += Time.deltaTime;
        yield return null;
    }
}

   public IEnumerator EmpujarJugadorAL3(float duracion)
{
   
    yield return new WaitForSeconds(0.1f);

    Vector3 direccionEmpuje = transform.forward;
    float tiempoTranscurrido = 0;

    while (tiempoTranscurrido < duracion)
    {
        Vector3 futurePosition = transform.position + direccionEmpuje * fuerzaEmpujeAP3 * Time.deltaTime;

       
        if (Physics.Raycast(futurePosition, Vector3.down, out RaycastHit hit, 1.0f))
        {
            
            transform.position = futurePosition;
        }
        else
        {
           
            break;
        }

        tiempoTranscurrido += Time.deltaTime;
        yield return null;
    }
}

public bool EstaMoviendose()
{
    return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
}

public Vector3 ObtenerDireccionEmpuje()
{
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");

    Vector3 direccionEmpuje = Vector3.zero;

    if (vertical > 0) // W key
    {
        direccionEmpuje += Vector3.forward;
    }
    else if (vertical < 0) // S key
    {
        direccionEmpuje += Vector3.back;
    }

    if (horizontal > 0) // D key
    {
        direccionEmpuje += Vector3.right;
    }
    else if (horizontal < 0) // A key
    {
        direccionEmpuje += Vector3.left;
    }

    return direccionEmpuje.normalized * fuerzaEmpujeAL2;
}
    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        dashTime = dashDuration;

        Vector3 dashDirection = transform.forward;

        while (dashTime > 0)
        {
            controller.MovePosition(controller.position + dashDirection * dashSpeed * Time.deltaTime);
            dashTime -= Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

public void AtaqueJugador()
{
    if (Input.GetMouseButtonDown(0))
    {
        enterAttack = true;
        hasAttacked = true;

        if (!hasRotated)
        {
            // Crea un rayo desde la posición del mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Realiza el raycast sin límites, chequeando si el objeto tiene la capa asignada
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ataque")))
            {                               
                
                    // Determina la posición del objetivo
                    Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                    // Calcula la dirección hacia el objetivo
                    Vector3 directionToLook = (targetPosition - transform.position).normalized;

                    // Genera la rotación necesaria para mirar hacia el objetivo
                    Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

                    // Aplica la rotación al jugador
                    transform.rotation = targetRotation;

                    hasRotated = true;
                
            }
        }
    }
}


public void hasRotatedTrue()
{
    hasRotated = true;
}
public void hasRotatedFalse()
{
    hasRotated = false;
}
public void AtaqueLigero()
{
    if (Input.GetButtonDown("Fire1") && !ataqueL)
    {      
    
      ataqueL = true;
       
    }
}

public IEnumerator MirarAlMouseAL3()
{
    float startTime = Time.time;
    Vector3 lastMousePosition = Vector3.zero;

    while (Time.time < startTime + 0.15f)
    {      
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            lastMousePosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            // Calcular la dirección hacia la que el jugador debe mirar
            Vector3 directionToLook = (lastMousePosition - transform.position).normalized;

            // Crear una rotación que mire en la dirección del objetivo
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

            // Aplicar la rotación al jugador
            transform.rotation = targetRotation;
        }

        yield return null;
    }
   
}

private bool ataqueEjecutado = false;

public void AtaquePesado()
{
    if (Input.GetKeyDown(KeyCode.Q) && Time.time >= tiempoUltimoAtaque + tiempoEsperaAtaque && !ataqueEjecutado)
    {
        enterAttack = true;
        hasAttacked = true;
        ataqueP = true;
        ataqueEjecutado = true;
        tiempoUltimoAtaque = Time.time; // Actualizar el tiempo del último ataque
        FMODUnity.RuntimeManager.PlayOneShot(heavy);
    }

    // Restablecer ataqueEjecutado después del tiempo de espera
    if (Time.time >= tiempoUltimoAtaque + tiempoEsperaAtaque)
    {
        ataqueEjecutado = false;
    }
}

public void AtaquePesadoAnim()
{
    atacando = true;
    ataqueCollider.enabled = true;
    ataqueMesh.enabled = true;
    ataqueCollider.transform.localScale = new Vector3(0, ataqueCollider.transform.localScale.y, 0); // Resetear el tamaño del collider en X y Z, mantener Y
    
}

private void ExpandirCollider()
{
    if (ataqueCollider.transform.localScale.x < rangoMaximo)
    {
        ataqueCollider.transform.localScale += new Vector3(velocidadExpansion * Time.deltaTime, 0, velocidadExpansion * Time.deltaTime);
    }
    else
    {
        atacando = false;
        ataqueCollider.enabled = false;
        ataqueMesh.enabled = false;
        tiempoUltimoAtaque = Time.time; // Actualizar el tiempo del último ataque cuando el collider alcance el rango máximo
        
    }
}

   public void AtaqueDistancia()
{
    if (Input.GetButtonDown("Fire2") && balasActuales > 0 && Time.time - tiempoUltimoDisparo >= tiempoEntreDisparos)
    {
        FMODUnity.RuntimeManager.PlayOneShot(shot);
        enterAttack = true;
        ataqueD = true;
        if (mirarCoroutine != null)
        {
            StopCoroutine(mirarCoroutine);
        }
        mirarCoroutine = StartCoroutine(MirarAlMousePorUnSegundo());
    }
  
}

private IEnumerator MirarAlMousePorUnSegundo()
{
    float startTime = Time.time;
    Vector3 lastMousePosition = Vector3.zero;

    while (Time.time < startTime + 0.5f)
    {
        if (Input.GetButtonUp("Fire2") && balasActuales > 0 && Time.time - tiempoUltimoDisparo >= tiempoEntreDisparos)
        {
            //EjecutarAtaqueDistancia();
            
            yield break;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            lastMousePosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            // Calcular la dirección hacia la que el jugador debe mirar
            Vector3 directionToLook = (lastMousePosition - transform.position).normalized;

            // Crear una rotación que mire en la dirección del objetivo
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

            // Aplicar la rotación al jugador
            transform.rotation = targetRotation;
        }

        yield return null;
    }

    // Ejecutar el ataque a distancia después de 1 segundo
    //EjecutarAtaqueDistancia();
}

private void EjecutarAtaqueDistancia()
{
    
    tiempoUltimoDisparo = Time.time;
    balasActuales--;

    GameObject boomerangObj = Instantiate(prefab, spawnPosition.position, spawnPosition.rotation);
    Boomerang boomerang = boomerangObj.GetComponent<Boomerang>();
    boomerang.Lanzar(spawnPosition.forward); // Lanzar en la dirección del objeto vacío
}
    private void RecargarBalas()
    {
        if (balasActuales < maxBalas)
        {
            if (Time.time - tiempoUltimaRecarga >= tiempoRecarga)
            {
                balasActuales++;
                tiempoUltimaRecarga = Time.time;
            }
        }
        else
        {
            tiempoUltimaRecarga = Time.time; // Resetear el tiempo de recarga cuando las balas están al máximo
        }
    }

    private void UpdateBalaSliders()
    {
        for (int i = 0; i < balaSliders.Length; i++)
        {
            if (i < balasActuales)
            {
                balaSliders[i].value = 1; // Bala cargada
                balaSliders[i].fillRect.GetComponent<Image>().color = fullColor; // Cambiar color a lleno
            }
            else if (i == balasActuales)
            {
                float tiempoDesdeUltimaRecarga = Time.time - tiempoUltimaRecarga;
                balaSliders[i].value = Mathf.Clamp01(tiempoDesdeUltimaRecarga / tiempoRecarga); // Progreso de recarga
                balaSliders[i].fillRect.GetComponent<Image>().color = emptyColor; // Color de recarga
            }
            else
            {
                balaSliders[i].value = 0; // Bala no cargada
                balaSliders[i].fillRect.GetComponent<Image>().color = emptyColor; // Cambiar color a vacío
            }
        }
    }


public void OnAttackEndl1()
    {
        damageDealerL1.ResetDamage();
    }

       public void OnAttackEndl2()
    {
        damageDealerL2.ResetDamage();
    }
   public void OnAttackEndl3()
    {
        damageDealerL3.ResetDamage();
    }
       public void OnAttackEndP()
    {
        damageDealerP.ResetDamage();
    }
    #endregion priscada
}


