using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
        // Variables del player
    [Header("Player Settings")]
    [SerializeField] public static float speed = 15.0f;
    public static float fuerzaEmpujeAL2 = 2.0f;  
    public static float fuerzaEmpujeAP3 = 50.0f;  
    public  float rotationSpeed = 10.0f;
    public float gravity = 20.0f;
    public static bool hasAttacked = false;
    public static bool hasRotated = false;
    public static MovimientoJugador instance; 
      

    [Header("Movement Settings")]
    private CharacterController controller;     
    bool canMove = true; 
    bool isLookingAtTarget = false;           
    Vector3 targetPosition;

    [Header("Attack Settings")]     
    public static bool ataqueL = false; 
    public static bool ataqueP = false; 
    public static bool ataqueD = false;  
    public Collider ataqueCollider;
    public MeshRenderer ataqueMesh;
    public float rangoMaximo = 6.0f;
    public float velocidadExpansion = 1.0f;
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

    [Header("Animation Settings")]
    public Animator animator;    
    public static bool enterAttack = false;
    public bool verificarArma = false;
    public static bool cambioarma = false;       

    [Header("Visual Settings")]
    private Color originalColor; 
    public Renderer cambioColoAlPegar; 

    [Header("Dash Settings")]
    public float DashSpeed = 50.0f;
    public GameObject dashObjec;    
    public static bool isDashing = false;
    public float dashTime = 0.2f;   
    private float lastDashTime = -Mathf.Infinity;

    // Variables del Bullet Time
    public float bulletTimeDuration = 7f;
    public bool bulletTime = false;
    public TestAfterImage afterImageEffect;
    public static float bulletTimeScale = 1f;
     FMODUnity.StudioEventEmitter FmodEmitter;

    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("DodgeArea") && Input.GetKeyDown(KeyCode.Space) && !bulletTime)
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
    }

    private IEnumerator ActivateBulletTime()
    {
        Debug.Log("Bullet time on");
        //FmodEmitter.Play();
        bulletTime = true;
        afterImageEffect.enabled = true;
        bulletTimeScale = 0.1f;
        DefaultHUD.Instance.EnableBulletTimeUI();

        yield return new WaitForSeconds(bulletTimeDuration);

        bulletTime = false;
        afterImageEffect.enabled = false;
        bulletTimeScale = 1f;
        Debug.Log("Bullet time off");
        DefaultHUD.Instance.DisableBulletTimeUI();
    }

    public bool IsBulletTimeActive()
    {
        return bulletTime; // Devuelve el estado del bullet time
    }

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
    void Start()
    {
         controller = GetComponent<CharacterController>();      

         instance = GetComponent<MovimientoJugador>();

         animator = GetComponent<Animator>();   
        tiempoUltimoAtaque = -tiempoEsperaAtaque;
    }
    void Update()
{
        
   

    if (canMove)
    {
        Movimientojugador();
    }   

    if (Input.GetKeyDown(KeyCode.Space))
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

      
    }
    public void Movimientojugador()   

{
    if (canMove && controller.isGrounded)
    {
        // Obtener la entrada del jugador
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Crear un vector de movimiento basado en la entrada del jugador
        Vector3 moveInput = new Vector3(horizontal, 0, vertical);

        // Verificar si se presionan las teclas de movimiento
        if ((horizontal != 0 || vertical != 0) && !enterAttack)
        {
            // Si se presionan las teclas de movimiento y el jugador no está atacando, establecer el parámetro "Run" en true
            animator.SetBool("Run", true);
        }
        else
        {
            // Si no se presionan las teclas de movimiento o el jugador está atacando, establecer el parámetro "Run" en false
            animator.SetBool("Run", false);
        }

        // Normalizar el vector de movimiento para asegurar que la velocidad del jugador sea constante
        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }

        // Multiplicar el vector de movimiento por la velocidad del jugador para obtener la velocidad final
        Vector3 moveDirection = moveInput * speed;

        // Aplicar el movimiento si el jugador está en el suelo o atacando
        if (controller.isGrounded || enterAttack)
        {
            controller.Move(moveDirection * Time.deltaTime);
        }

        if (moveDirection != Vector3.zero && !isLookingAtTarget && !enterAttack) // Evita la rotación cuando el jugador no se está moviendo
        {
            Quaternion targetRotation = Quaternion.identity;

            switch (vertical)
            {
                case 1: // W key
                    switch (horizontal)
                    {
                        case 1: // D key
                            targetRotation = Quaternion.Euler(0, 45, 0);
                            break;
                        case -1: // A key
                            targetRotation = Quaternion.Euler(0, -45, 0);
                            break;
                        default:
                            targetRotation = Quaternion.Euler(0, 0, 0);
                            break;
                    }
                    break;
                case -1: // S key
                    switch (horizontal)
                    {
                        case 1: // D key
                            targetRotation = Quaternion.Euler(0, 135, 0);
                            break;
                        case -1: // A key
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
                        case 1: // D key
                            targetRotation = Quaternion.Euler(0, 90, 0);
                            break;
                        case -1: // A key
                            targetRotation = Quaternion.Euler(0, -90, 0);
                            break;
                        default:
                            targetRotation = transform.rotation;
                            break;
                    }
                    break;
            }

            // Aplicar la rotación al objeto del jugador
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else if (targetPosition != Vector3.zero) // Si el jugador no se está moviendo y hay una posición de click guardada
        {
            // Calcular la rotación objetivo
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

            // Aplicar la rotación al jugador
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Aplicar gravedad
    Vector3 gravityVector = new Vector3(0, speed * -gravity * Time.deltaTime, 0);
    controller.Move(gravityVector * Time.deltaTime);
}

    public IEnumerator EmpujarJugadorAL2(Vector3 direccion, float duracion)
{
    float tiempoTranscurrido = 0;
    while (tiempoTranscurrido < duracion)
    {
        controller.Move(direccion * (Time.deltaTime / duracion));
        tiempoTranscurrido += Time.deltaTime;
        yield return null;
    }
}

   public IEnumerator EmpujarJugadorAL3(float duracion)
    {
       
           // Esperar 1 segundo antes de ejecutar el bucle while
    yield return new WaitForSeconds(0.1f);
    
     Vector3 direccionEmpuje = transform.forward;
        float tiempoTranscurrido = 0;

    while (tiempoTranscurrido < duracion)
    {
        transform.position += direccionEmpuje * fuerzaEmpujeAP3 * Time.deltaTime;
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
    if (!isDashing && Time.time >= lastDashTime + dashTime)
    {
        float originalY = transform.position.y;

        isDashing = true;
        animator.SetBool("Dash", true);
        float startTime = Time.time;

        // Obtener la dirección de movimiento actual del jugador
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // Si no hay entrada de movimiento, usar la dirección actual del jugador
        if (moveDirection == Vector3.zero)
        {
            moveDirection = transform.forward;
        }

        while (Time.time < startTime + dashTime)
        {
            float fractionOfDashTimePassed = (Time.time - startTime) / dashTime;
            float currentDashSpeed = Mathf.Lerp(DashSpeed, 0, fractionOfDashTimePassed);
            instance.controller.Move(moveDirection * currentDashSpeed * Time.deltaTime);

            Vector3 newPosition = transform.position;
            newPosition.y = originalY;
            transform.position = newPosition;

            yield return null;
        }

        animator.SetBool("Dash", false);

        dashObjec.SetActive(true);

        yield return new WaitForSeconds(0.5f); 

        dashObjec.SetActive(false);

        Vector3 finalPosition = transform.position;
        finalPosition.y = originalY;
        transform.position = finalPosition;

        lastDashTime = Time.time;
        isDashing = false;
    }
}
   
public void AtaqueJugador()
 {
if (Input.GetMouseButtonDown(0)) 
   {
    enterAttack = true;
    hasAttacked = true;
      


    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit) && !hasRotated)
     {
        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

        // Calcular la dirección hacia la que el jugador debe mirar
        Vector3 directionToLook = (targetPosition - transform.position).normalized;

        // Crear una rotación que mire en la dirección del objetivo
        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

        // Aplicar la rotación al jugador
        transform.rotation = targetRotation;

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
        Debug.Log("sipi");
        ataqueP = true;
        ataqueEjecutado = true;
        tiempoUltimoAtaque = Time.time; // Actualizar el tiempo del último ataque
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
        speed = 0;
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
            EjecutarAtaqueDistancia();
            
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
    EjecutarAtaqueDistancia();
}

private void EjecutarAtaqueDistancia()
{
    speed = 15.0f;
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








      

}

