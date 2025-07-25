using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;


public class MovimientoJugador : MonoBehaviour
{
    //pruebas enemigos
    public float pushForce = 10f;

    // Variables del player
    [SerializeField] private Life life;
    
    [Header("Player Settings")]
    [SerializeField] public static float speed = 15.0f;
    public static float fuerzaEmpujeAL2 = 3.0f;  
    public static float fuerzaEmpujeAP3 = 50.0f;  
    public  float rotationSpeed = 10.0f;
    public float gravity = 20.0f;
    public static bool hasAttacked = false;
    public static bool hasRotated = false;
    public static MovimientoJugador instance; 
    public LayerMask obstacleLayers;
    private Vector3 direccionRaycast;

    public DamageDealer damageDealerL1;    
    public DamageDealer damageDealerL2;     
    public DamageDealer damageDealerL3;     
    public DamageDealer damageDealerP; 

    [Header("Movement Settings")]
    private Rigidbody rb;  
 
    [Header("Attack Settings")]     
    public static bool ataqueL = false; 
    public static bool ataqueP = false; 
    public static bool ataqueD = false;  
    public static bool ataqueD2 = false;
    public Collider ataqueCollider;
    public MeshRenderer ataqueMesh;
    public float rangoMaximo = 10.0f;
    public float velocidadExpansion = 2.0f;
    private bool atacando = false; 
    private Coroutine mirarCoroutine; 
    private Coroutine disparoCoroutine; // Nueva variable para manejar la corrutina del disparo cargado 
    public float tiempoEsperaAtaque = 1.5f; // Reducido de 3.0f a 1.5f
    private float tiempoUltimoAtaque;   
    public static bool canAttack = true;
    // Remover esta línea: public bool canChargeShot = true; // Nueva variable para controlar el disparo cargado

    [Header("Distance Settings")]  
    public GameObject prefabDisparoNormal;
    public GameObject prefabDisparoCargado;
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

    [Header("Visual Settings")]
    private Color originalColor; 
    public Renderer cambioColoAlPegar; 

    [Header("Dash Settings")]
    public static bool isDashing = false;
    private bool canDash = true;
    private float dashTime;    
    public float dashSpeed = 20.0f; // Velocidad del dash
    public float dashDuration = 0.2f; // Duración del dash en segundos
    public float dashCooldown = 1.0f; // Tiempo de recarga del dash en segundos
    
    [Header("Bullet time")]
    Collider[] btColliders;
    public Collider btTriggerCollider;
    public Material btColliderOff;
    public Material btColliderOn;
    public GameObject BTCollider;
    //
    public float bulletTimeDuration = 6f;
    public bool bulletTime = false;
    public TestAfterImage afterImageEffect;
    public static float bulletTimeScale = 1f;
    public float slowedBulletTimeScale = 0.05f;
    public float bulletTimeCooldown = 5f;
    private float lastBulletTimeUse = -Mathf.Infinity;
    public Image bulletTimeCooldownImage;

    [Header("Fall")]     
    //private Queue<Vector3> safePositions = new Queue<Vector3>();
    private bool isGrounded;

    [Header("Sounds")]     
    [SerializeField] private FMODUnity.EventReference bulletTimeStart;
    [SerializeField] private FMODUnity.EventReference bulletTimeEnd;
    [SerializeField] private FMODUnity.EventReference shot;
    [SerializeField] private FMODUnity.EventReference heavy;
    [SerializeField] private FMODUnity.EventReference dash;

    public static bool isInDodgeArea = false;
    public static bool godMode = false;

    /*     [Header("Progreso")]
        public int nivelActual = 1;
        public bool disparoDesbloqueado = false; */
    
        [SerializeField] public int nivelActual
    {
        get => CoreManager.nivelActual;
        set => CoreManager.nivelActual = value;
    }
    
    [SerializeField] public bool disparoDesbloqueado
    {
        get => CoreManager.disparoDesbloqueado;
        set => CoreManager.disparoDesbloqueado = value;
    }

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
            //Debug.Log("Caída detectada, teletransportando a la última posición segura.");
            //StartCoroutine(TeleportToLastSafePosition());
        }
    }

   /*  private IEnumerator UpdateSafePosition()
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
    } */
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
        //limpiar tiempo bala

        bulletTime = false;
        bulletTimeScale = 1f;
        if (afterImageEffect != null)
            afterImageEffect.enabled = false;
        DefaultHUD.Instance?.DisableBulletTimeUI();

        if (instance == null)
        {
            instance = this;

            // Inicializar variables estáticas para asegurar valores correctos
            speed = 15.0f;
            hasAttacked = false;
            hasRotated = false;
            ataqueL = false;
            ataqueP = false;
            ataqueD = false;
            ataqueD2 = false;
            canAttack = true;
            isDashing = false;
            enterAttack = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        life = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();
        rb = GetComponent<Rigidbody>();      
        instance = GetComponent<MovimientoJugador>();
        animator = GetComponent<Animator>();   
        tiempoUltimoAtaque = -tiempoEsperaAtaque;
        
        // Asegurar estado inicial correcto
        canMove = true;
        canAttack = true;
        isDashing = false;
        atacando = false;
        ataqueEjecutado = false;
        bulletTime = false;
        
        // Asegurar velocidad correcta
        speed = 15.0f;

/*         for (int i = 0; i < 5; i++)
                        {
                            safePositions.Enqueue(transform.position);
                        }

                        StartCoroutine(UpdateSafePosition()); */

        btColliders = Physics.OverlapSphere(transform.position, 10000);
    }


  void Update()
    { 
        if (Input.GetKeyDown(KeyCode.G))
        {
            godMode = !godMode;
            Debug.Log("God Mode: " + (godMode ? "Activated" : "Deactivated"));
            rangoMaximo = 999;
            
            Debug.Log($"Nivel Actual: {nivelActual}");
            Debug.Log($"Disparo Desbloqueado: {disparoDesbloqueado}");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            disparoDesbloqueado = true;
            nivelActual = 4;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            enterAttack = true;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            enterAttack = false;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            BTCollider.SetActive(!BTCollider.activeSelf);
        } 

/*      if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CountBTProjectiles();
        } */

        // Lógica combinada para tiempo bala y dash - MEJORADA
        if (isInDodgeArea && Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time >= lastBulletTimeUse + bulletTimeCooldown && canDash)
            {
                if (!bulletTime && afterImageEffect != null)
                {
                    // Activar tiempo bala y dash juntos
                    StartCoroutine(ActivateBulletTime());
                    StartCoroutine(Dash());
                }
                else if (bulletTime)
                {
                    // Solo dash si ya está en tiempo bala
                    StartCoroutine(Dash());
                }
                else
                {
                    Debug.LogError("afterImageEffect no está asignado en el Inspector.");
                }
            }
        }

        // Dash normal fuera del área de dodge - MEJORADA
        if (!isInDodgeArea && Input.GetKeyDown(KeyCode.Space) && canDash)
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

         Movimientojugador();
    }

    private void FixedUpdate()
    {
       
    }
    
    public void CountBTProjectiles()
    {
        if (instance == null)
        {
            Debug.Log("MovimientoJugador instance is null.");
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4f);
        int count = 0;
        foreach (var hitCollider in hitColliders)   
        {
            if (hitCollider.CompareTag("Projectile"))
            {
                count++;
            }
        }
        //Debug.Log("proyectiles en rango: " + count);

        if (count > 0)
        {
            BTCollider.GetComponent<Renderer>().material = btColliderOn;
            isInDodgeArea = true;
        }

        if (count == 0)
        {
            BTCollider.GetComponent<Renderer>().material = btColliderOff;
            isInDodgeArea = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        Gizmos.DrawWireSphere(transform.position, 4f);
    }

  

   public void Movimientojugador()
{
    if (!canMove) 
    {
        // Desactivar la animación de correr cuando no se puede mover (disparo cargado)
        animator.SetBool("Run", false);
        return; // Si no puede moverse, salir del método
    }

    float hor = Input.GetAxisRaw("Horizontal");
    float ver = Input.GetAxisRaw("Vertical");
    Vector3 velocity = Vector3.zero;

    if (hor != 0 || ver != 0)
    {
        animator.SetBool("Run", true);
        
        Vector3 direction = new Vector3(hor, 0, ver).normalized;
        ultimaDireccion = direction; // Actualizar la última dirección
        
        // Evitar rotación durante el ataque 
        if (!enterAttack && !ataqueL && !ataqueP && !ataqueD)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.2f);
            hasRotated = false; // Solo resetear cuando realmente rotamos con movimiento
        }
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, obstacleLayers))
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity = direction * speed * Time.deltaTime;
        }
    }
    else
    {
        animator.SetBool("Run", false);
        // NO resetear hasRotated cuando está parado - esto permitirá mantener la dirección de ataque
    }

    rb.MovePosition(rb.position + velocity);
}

public Vector3 ObtenerUltimaDireccion()
{
    return ultimaDireccion;
}

IEnumerator Dash()
{
    // Verificar si ya está en dash para evitar múltiples ejecuciones
    if (isDashing)
    {
        yield break;
    }

    canAttack = false; // Deshabilitar ataques
    animator.Play("Dash");
    isDashing = true;
    canDash = false;
    dashTime = dashDuration;

    // Deshabilitar ataques y otras animaciones durante el dash
    enterAttack = false;
    animator.SetBool("Run", false);
    
    FMODUnity.RuntimeManager.PlayOneShot(dash);

    Vector3 dashDirection = ObtenerDireccionDash();
    if (dashDirection == Vector3.zero)
    {
        dashDirection = transform.forward.normalized;
    }

    rb.useGravity = false;

    while (dashTime > 0)
    {
        life.enableInvencibility();

        float dashFrameDistance = dashSpeed * Time.fixedDeltaTime;

        if (Physics.Raycast(rb.position, dashDirection, out RaycastHit hit, dashFrameDistance, obstacleLayers))
        {
            break;
        }

        rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);

        dashTime -= Time.fixedDeltaTime;
        yield return new WaitForFixedUpdate();
    }

    isDashing = false;
    rb.useGravity = true;
    life.disableInvencibility();

    // Iniciar cooldown del dash
    StartCoroutine(DashCooldown());
    canAttack = true; // Habilitar ataques nuevamente
}

// Nueva corrutina separada para el cooldown del dash
private IEnumerator DashCooldown()
{
    yield return new WaitForSeconds(dashCooldown);
    canDash = true;
}

public IEnumerator EmpujarJugadorAL2(Vector3 direccion, float duracion)
{
    
    float tiempoTranscurrido = 0f;
   
    while (tiempoTranscurrido < duracion)
    {
        
        float movimientoFrame = Time.deltaTime / duracion;

       
        Vector3 posicionActual = rb.position;

        Vector3 posicionFutura = posicionActual + direccion * movimientoFrame;

        float distanciaAMover = (direccion * movimientoFrame).magnitude;

      
        if (Physics.Raycast(posicionActual, direccion, out RaycastHit hit, distanciaAMover + 0.1f))
        {            
            break;
        }

        
        rb.MovePosition(posicionFutura);

        tiempoTranscurrido += Time.deltaTime;
        yield return null;
    }
}

private Vector3 ObtenerDireccionDash()
{
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");

    Vector3 direccionDash = Vector3.zero;

    
    if (Mathf.Abs(horizontal) > 0 && Mathf.Abs(vertical) == 0)
    {
        direccionDash = new Vector3(horizontal, 0, 0).normalized;
    }
    else if (Mathf.Abs(vertical) > 0 && Mathf.Abs(horizontal) == 0)
    {
        direccionDash = new Vector3(0, 0, vertical).normalized;
    }
    else if (Mathf.Abs(horizontal) > 0 && Mathf.Abs(vertical) > 0)
    {
        direccionDash = new Vector3(horizontal, 0, vertical).normalized;
    }

    return direccionDash;
}


public IEnumerator EmpujarJugadorAL3(float duracion)
{
    yield return new WaitForSeconds(0.1f);

    float tiempoTranscurrido = 0f;

    while (tiempoTranscurrido < duracion)
    {
        float desplazamientoFrame = fuerzaEmpujeAP3 * Time.deltaTime;

        if (Physics.Raycast(transform.position, direccionRaycast, out RaycastHit hitForward, desplazamientoFrame + 0.1f))
        {
            break;
        }

        Vector3 futurePosition = transform.position + direccionRaycast * desplazamientoFrame;
        if (Physics.Raycast(futurePosition, Vector3.down, out RaycastHit hitDown, 1.0f))
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

public Vector3 ObtenerDireccionMovimiento()
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

    return direccionEmpuje.normalized;
}

    // Función helper para cancelar todos los ataques en curso (excepto ataque pesado)
    private void CancelarAtaquesEnCurso()
    {
        // NO cancelar ataque pesado - debe completarse
        
        // Cancelar ataque a distancia si está en curso
        if (ataqueD || ataqueD2)
        {
            if (mirarCoroutine != null)
            {
                StopCoroutine(mirarCoroutine);
                mirarCoroutine = null;
            }
            if (disparoCoroutine != null)
            {
                StopCoroutine(disparoCoroutine);
                disparoCoroutine = null;
            }
            ataqueD = false;
            ataqueD2 = false;
            canMove = true; // Asegurar que el jugador pueda moverse
        }
        
        // Resetear todos los DamageDealer de ataques ligeros
        if (damageDealerL1 != null) damageDealerL1.ResetDamage();
        if (damageDealerL2 != null) damageDealerL2.ResetDamage();
        if (damageDealerL3 != null) damageDealerL3.ResetDamage();
        
        // Asegurar que el movimiento esté habilitado (en caso de que se haya deshabilitado por disparo cargado)
        canMove = true;
    }
  
    // Función específica para cancelar solo el ataque pesado cuando se inicia otro ataque pesado
    private void CancelarAtaquePesado()
    {
        if (atacando) 
        {
            atacando = false;
            ataqueCollider.enabled = false;
            ataqueMesh.enabled = false;
            ataqueP = false;
            ataqueEjecutado = false; // Resetear para permitir un nuevo ataque
            if (damageDealerP != null)
            {
                damageDealerP.ResetDamage();
            }
        }
    }
  
public void AtaqueJugador()
{
    if (Input.GetMouseButtonDown(0) && canAttack)
    {
        // No permitir ataques ligeros durante ejecución Y recuperación del ataque pesado
        if (atacando || ataqueP)
        {
            return;
        }
        
        // Cancelar solo ataques a distancia
        CancelarAtaquesEnCurso();
        
        enterAttack = true;

        // Permitir rotación en cada clic de mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ataque")))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 directionToLook = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            transform.rotation = targetRotation;
          
            direccionRaycast = directionToLook;

            hasRotated = true;
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
        if (Input.GetButtonDown("Fire1") && !ataqueL && (canAttack || godMode))
        {     
            // No permitir ataques ligeros durante ejecución Y recuperación del ataque pesado (excepto en god mode)
            if (!godMode && (atacando || ataqueP))
            {
                return;
            }
            
            ataqueL = true;
            // NO resetear hasRotated aquí para permitir cancelación de ataques
        }
    }

    private bool ataqueEjecutado = false;
    private Vector3 ultimaDireccion;

    public void AtaquePesado()
{
    if (Input.GetKeyDown(KeyCode.Q))
    {
        // Debug para identificar qué condición está bloqueando el ataque
        if (godMode)
        {
            Debug.Log("God Mode activo - ejecutando ataque pesado");
            EjecutarAtaquePesado();
            return;
        }

        // Verificar condiciones una por una con debug
        bool tiempoSuficiente = Time.time >= tiempoUltimoAtaque + tiempoEsperaAtaque;
        bool noEstaEjecutado = !ataqueEjecutado;
        bool noEstaAtacando = !atacando;
        bool noEstaAtaqueP = !ataqueP;
        bool puedeAtacar = canAttack;

        Debug.Log($"Ataque Pesado - Tiempo suficiente: {tiempoSuficiente}, No ejecutado: {noEstaEjecutado}, No atacando: {noEstaAtacando}, No ataque P: {noEstaAtaqueP}, Puede atacar: {puedeAtacar}");
        Debug.Log($"Tiempo actual: {Time.time}, Último ataque: {tiempoUltimoAtaque}, Diferencia: {Time.time - tiempoUltimoAtaque}, Requerido: {tiempoEsperaAtaque}");

        if (tiempoSuficiente && noEstaEjecutado && noEstaAtacando && noEstaAtaqueP && puedeAtacar)
        {
            Debug.Log("Ejecutando ataque pesado");
            EjecutarAtaquePesado();
        }
        else
        {
            Debug.Log("Ataque pesado bloqueado por condiciones");
        }
    }
}

private void EjecutarAtaquePesado()
{
    // Cancelar ataques a distancia (excepto en god mode)
    if (!godMode)
    {
        CancelarAtaquesEnCurso();
    }

    enterAttack = true;
    
    // Permitir rotación hacia el mouse al activar ataque pesado
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ataque")))
    {
        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        Vector3 directionToLook = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
        transform.rotation = targetRotation;
        direccionRaycast = directionToLook;
        hasRotated = true;
    }
  
    ataqueP = true;
    if (!godMode)
    {
        ataqueEjecutado = true;
        tiempoUltimoAtaque = Time.time; // Marcar el inicio del ataque
    }
    FMODUnity.RuntimeManager.PlayOneShot(heavy);
}

public void AtaquePesadoAnim()
{
    atacando = true;
    ataqueCollider.enabled = true;
    ataqueMesh.enabled = true;
    ataqueCollider.transform.localScale = new Vector3(0, ataqueCollider.transform.localScale.y, 0); // Resetear el tamaño del collider en X y Z, mantener Y
    
    // Activar el DamageDealer del ataque pesado si existe
    if (damageDealerP != null)
    {
        damageDealerP.ResetDamage(); // Resetear para permitir nuevo daño
    }
}

private void ExpandirCollider()
{
    if (ataqueCollider.transform.localScale.x < rangoMaximo)
    {
        ataqueCollider.transform.localScale += new Vector3(velocidadExpansion * Time.deltaTime, 0, velocidadExpansion * Time.deltaTime);
    }
    else
    {
        Debug.Log("Finalizando ataque pesado");
        atacando = false;
        ataqueCollider.enabled = false;
        ataqueMesh.enabled = false;
        ataqueP = false; // Resetear la bandera del ataque pesado
        
        if (!godMode)
        {
            // Simplificar el cooldown - solo marcar el tiempo actual
            tiempoUltimoAtaque = Time.time;
            
            // Resetear la bandera para permitir un nuevo ataque pesado después del cooldown
            ataqueEjecutado = false;
            Debug.Log($"Ataque pesado completado. Próximo disponible en: {Time.time + tiempoEsperaAtaque}");
        }
        
        // Resetear el DamageDealer del ataque pesado cuando termina
        if (damageDealerP != null)
        {
            damageDealerP.ResetDamage();
        }
    }
}

public void AtaqueDistancia()
{
    if (Input.GetButtonDown("Fire2") && (godMode || (balasActuales > 0 && Time.time - tiempoUltimoDisparo >= tiempoEntreDisparos)))
    {
        // No permitir ataques a distancia durante ejecución Y recuperación del ataque pesado (excepto en god mode)
        if (!godMode && (atacando || ataqueP))
        {
            return;
        }
        
        // Cancelar solo otros ataques a distancia (excepto en god mode)
        if (!godMode)
        {
            CancelarAtaquesEnCurso();
        }
        
        enterAttack = true;
        
        // Permitir rotación hacia el mouse al activar ataque a distancia
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ataque")))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 directionToLook = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            transform.rotation = targetRotation;
            direccionRaycast = directionToLook;
            hasRotated = true;
        }
        
        if (!disparoDesbloqueado)
        {
            // Reproducir la animación inmediatamente para disparo normal
            animator.Play("3P");
            EjecutarDisparoNormal(); // Disparo normal si el disparo cargado está desactivado
        }
        else
        {
            // NO reproducir animación aquí - se reproducirá en la corrutina DisparoNormalOCargado
            disparoCoroutine = StartCoroutine(DisparoNormalOCargado()); // Lógica actual si el disparo cargado está activado
        }
    }
}

private void EjecutarDisparoNormal()
{
    FMODUnity.RuntimeManager.PlayOneShot(shot); // Sonido de disparo normal
    Debug.Log("Disparo normal ejecutado");
    enterAttack = true;
    ataqueD = true;
    
    // Reproducir animación del disparo
    animator.Play("3P");

    // Ejecutar el disparo normal (crear el proyectil)
    EjecutarAtaqueDistancia();

    // Iniciar la corrutina de mirar al mouse DESPUÉS de ejecutar el disparo
    if (mirarCoroutine != null)
    {
        StopCoroutine(mirarCoroutine);
    }
    mirarCoroutine = StartCoroutine(MirarAlMousePorUnSegundo());
}

private IEnumerator DisparoNormalOCargado()
{
    float tiempoCarga = 0f;
    bool disparoCargado = false;

    canMove = false; // Deshabilitar movimiento del jugador
    
    // Reproducir la animación "3P" inmediatamente al empezar a cargar
    animator.Play("3P");
    
    while (Input.GetButton("Fire2"))
    {
        tiempoCarga += Time.deltaTime;

        // Mirar hacia donde apunta el ratón
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ataque")))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 directionToLook = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            transform.rotation = targetRotation;
        }

        if (tiempoCarga >= 0.5f && disparoDesbloqueado) // Reducido de 1f a 0.6f
        {            
            disparoCargado = true;
            break;
        }

        yield return null;
    }

    canMove = true; // Habilitar movimiento del jugador nuevamente
    disparoCoroutine = null; // Resetear la referencia a la corrutina

    if (disparoCargado)
    {
        FMODUnity.RuntimeManager.PlayOneShot(shot); // Sonido de disparo cargado
        Debug.Log("Disparo cargado ejecutado");      

        ataqueD2 = true;
        // Ejecutar el disparo cargado (crear el proyectil)
        EjecutarAtaqueDistanciaCargado();
        
        // Iniciar la corrutina de mirar al mouse DESPUÉS de ejecutar el disparo
        mirarCoroutine = StartCoroutine(MirarAlMousePorUnSegundo());
    }
    else
    {
        // Si no se cargó completamente, ejecutar disparo normal (pero sin duplicar la animación)
        FMODUnity.RuntimeManager.PlayOneShot(shot); // Sonido de disparo normal
        Debug.Log("Disparo normal ejecutado (no se completó la carga)");
        enterAttack = true;
        ataqueD = true;
        
        // Ejecutar el disparo normal (crear el proyectil)
        EjecutarAtaqueDistancia();

        // Iniciar la corrutina de mirar al mouse DESPUÉS de ejecutar el disparo
        if (mirarCoroutine != null)
        {
            StopCoroutine(mirarCoroutine);
        }
        mirarCoroutine = StartCoroutine(MirarAlMousePorUnSegundo());
    }
}

private void EjecutarAtaqueDistanciaCargado()
{
    if (!godMode)
    {
        tiempoUltimoDisparo = Time.time;
        balasActuales--;
    }

    GameObject boomerangObj = Instantiate(prefabDisparoCargado, spawnPosition.position, spawnPosition.rotation);
    DisparoCargado boomerang = boomerangObj.GetComponent<DisparoCargado>();
    boomerang.Lanzar(spawnPosition.forward); // Lanzar en la dirección del objeto vacío
}

private void EjecutarAtaqueDistancia()
{
    if (!godMode)
    {
        tiempoUltimoDisparo = Time.time;
        balasActuales--;
    }

    GameObject boomerangObj = Instantiate(prefabDisparoNormal, spawnPosition.position, spawnPosition.rotation);
    Boomerang boomerang = boomerangObj.GetComponent<Boomerang>();
    boomerang.Lanzar(spawnPosition.forward); // Lanzar en la dirección del objeto vacío
}

// Corrutina que orienta al jugador hacia donde apunta el mouse durante un tiempo corto
private IEnumerator MirarAlMousePorUnSegundo()
{
    float duration = 0.1f; // Duración muy corta después del disparo
    float elapsed = 0f;
    
    while (elapsed < duration)
    {
        // Crea un rayo desde la posición del mouse en la cámara
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Se realiza el raycast filtrando la capa "Ataque" (ajusta el nombre de la capa si es necesario)
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ataque")))
        {
            // Calcula la posición objetivo (manteniendo la altura del jugador)
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            
            // Calcula la dirección hacia el objetivo y la rotación correspondiente
            Vector3 directionToLook = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            
            // Asigna la rotación al jugador
            transform.rotation = targetRotation;
        }
        
        elapsed += Time.deltaTime;
        yield return null;
    }
    
    enterAttack = false;
    ataqueD = false;
    hasRotated = false; // Resetear para permitir nueva rotación
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

    public bool CanDash()
    {
        return canDash;
    }
    
    // Propiedad pública para verificar si hay un ataque pesado en curso
    public bool EstaAtacandoPesado()
    {
        return atacando;
    }
    #endregion priscada

    public bool canMove = true; // Nueva variable para controlar el movimiento del jugador
}