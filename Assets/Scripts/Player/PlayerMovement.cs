using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using UnityEngine.VFX;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IEffectable
{
       public float speed = 15.0f;
       public float reduccionVelocidad = 4f;

       public float reduccionVelocidadOriginal;
       public float rotationSpeed = 10.0f;
       public float gravity = 20.0f;
       public bool speedbuff = false;
       public float DashSpeed;
       private float lastDashTime = -Mathf.Infinity;

       public float dashTime;  

       PlayerMovement playerMovement;
       private CharacterController controller;
       private Vector3 moveDirection = Vector3.zero;
       bool canMove = true;   
       bool isLookingAtTarget = false;      
       public static bool hasRotated = false; // Añade esta variable al principio de tu clase
       Vector3 targetPosition;
       public Animator animator;
       public bool isAttacking = false;
       public bool isAttackingP = false;
       public static bool hasAttacked = false;
       public static bool enterAttack = false;
       public bool verificarArma = false;
       public static bool cambioarma = false;

       public static PlayerMovement instance;
       public VisualEffect vfx;

       public static bool slowless;
       public GameObject boomerangPrefab; // Prefab del boomerang
       public GameObject boomerangPrefab2; // Prefab del boomerang
       public string etiquetaJugador = "Player"; // Etiqueta del jugador

       public GameObject grimorio1;
       public GameObject grimorio2;
       public  bool isDashing = false;
       public Life lifeInstance;
       public Renderer cambioColoAlPegar; // Asegúrate de asignar este valor en el inspector de Unity
       private Color originalColor; // Para almacenar el color original del material

       ZombiEnemy zombiEnemy;

       public float puntosUltimate = 0f;
       public float maxPuntosUltimate= 20f;

     // Buffos velocidad.
    public float BuffSpeed = 1.0f;  // Controla el porcentaje de velocidad y animación

    private float originalSpeed;  // Almacena la velocidad original del jugador
    private float originalAnimatorSpeed;  // Almacena la velocidad original de las animaciones


       public GameObject objectPrefab;
private GameObject[] objects = new GameObject[3];
public float radius = 5f;
public float orbitDuration = 10f;
public float rotationSpeedUlt = 20f;

private bool isButtonPressed = false;
private float buttonPressTime = 0f;
public Image ultimateImage;


  public GameObject dashObjec;


        IEnumerator FinRef()
    {
        yield return new WaitForSeconds(0.1f); // Espera breve para permitir la inicialización

        // Encontrar el DefaultHUD(Clone) globalmente
        GameObject hud = GameObject.Find("DefaultHUD(Clone)");

        if (hud != null)
        {
            // Buscar CombatUI dentro de DefaultHUD(Clone) y encontrar el Image de Ultimate
            Transform combatUITransform = hud.transform.Find("CombatUI");

            if (combatUITransform != null)
            {
                // Asignar la referencia al componente Image de Ultimate
                ultimateImage = combatUITransform.Find("Ultimate").GetComponent<Image>();

                if (ultimateImage != null)
                {
                    Debug.Log("Ultimate Image encontrado correctamente.");
                }
                else
                {
                    Debug.LogError("No se encontró el componente Image en Ultimate.");
                }
            }
            else
            {
                Debug.LogError("No se pudo encontrar CombatUI dentro de DefaultHUD(Clone).");
            }

            // Buscar Grimory dentro de DefaultHUD(Clone) y encontrar Grimorio1 y Grimorio2
            Transform grimoryTransform = hud.transform.Find("Grimory");

            if (grimoryTransform != null)
            {
                // Asignar las referencias a Grimorio1 y Grimorio2
                grimorio1 = grimoryTransform.Find("Grimorio1").gameObject;
                grimorio2 = grimoryTransform.Find("Grimorio2").gameObject;

                if (grimorio1 != null && grimorio2 != null)
                {
                    Debug.Log("Grimorio1 y Grimorio2 encontrados correctamente.");
                }
                else
                {
                    Debug.LogError("No se pudieron encontrar Grimorio1 o Grimorio2.");
                }
            }
            else
            {
                Debug.LogError("No se pudo encontrar Grimory dentro de DefaultHUD(Clone).");
            }
        }
        else
        {
            Debug.LogError("No se pudo encontrar DefaultHUD(Clone). Asegúrate de que está en la escena.");
        }
    }
    
        private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        originalSpeed = speed;
        originalAnimatorSpeed = animator.speed;
        reduccionVelocidadOriginal = reduccionVelocidad;
        instance = this;
        boomerangPrefab = Resources.Load<GameObject>("BoomerangPrefab");
        boomerangPrefab2 = Resources.Load<GameObject>("BoomerangPrefab2");
       
       
       
    }
      
          void Start()
    {
         controller = GetComponent<CharacterController>();
         playerMovement = GetComponent<PlayerMovement>();

         animator = GetComponent<Animator>();

        verificarArma = true;
        cambioarma =true;

         originalColor = cambioColoAlPegar.material.color;
          StartCoroutine(FinRef());

         
    }

void Update()
{

    
 
    if (!slowless)
    {

        speed = originalSpeed * BuffSpeed;
        animator.speed = originalAnimatorSpeed * BuffSpeed;
        reduccionVelocidad = reduccionVelocidadOriginal * BuffSpeed;
    }
    else
    {
        speed = originalSpeed * BuffSpeed;
        animator.speed = originalAnimatorSpeed * BuffSpeed;
        reduccionVelocidad = reduccionVelocidadOriginal * BuffSpeed;
        speed = reduccionVelocidad;
        
    }


    Ultimate();

   DamagePlayer();

    if (canMove)
    {
        MovimientoJugador();
    }   


    if (Input.GetKeyDown(KeyCode.Space))
    {
        StartCoroutine(Dash());
    }

 AtaquesGrimorios();

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
   
        //vfx.transform.rotation = this.transform.rotation;
         CambioArma();

if (verificarArma)
{
    if (grimorio1 != null)
    {
        grimorio1.SetActive(true);
    }

    if (grimorio2 != null)
    {
        grimorio2.SetActive(false);
    }
}
else
{
    if (grimorio1 != null)
    {
        grimorio1.SetActive(false);
    }

    if (grimorio2 != null)
    {
        grimorio2.SetActive(true);
    }
}








}
public void CambioArma()
{
       if (Input.GetKeyDown(KeyCode.LeftShift) && cambioarma)
    {
      verificarArma = !verificarArma;
      Debug.Log(verificarArma);
    }
    
}
public void AtaquesGrimorios()
{
//Grimorio1
    if (Input.GetButtonDown("Fire1") && verificarArma)
    {
       isAttacking = true;
    }

//Grimorio2
   if (Input.GetButtonDown("Fire1") && !verificarArma)
    {
        isAttackingP = true;
    }
 
}

public void GrimorioDistancia()
{
                
                GameObject A = GameObject.Find("ColliderGrimorios");

                      GameObject jugador = GameObject.FindGameObjectWithTag(etiquetaJugador);
            if (jugador != null)
            {
                // Obtener la dirección hacia adelante (forward) del jugador
                Vector3 direccion = jugador.transform.forward;
                // Instanciar el boomerang en la posición del jugador y con su rotación
                GameObject boomerang = Instantiate(boomerangPrefab2, A.transform.position, Quaternion.identity);
                // Iniciar el movimiento del boomerang con la dirección hacia adelante del jugador
                boomerang.GetComponent<Boomerang>().IniciarMovimiento(direccion);
            }
               
}

public void GrimorioDistanciaVuelta()
{
                
                GameObject A = GameObject.Find("ColliderGrimorios");

                      GameObject jugador = GameObject.FindGameObjectWithTag(etiquetaJugador);
            if (jugador != null)
            {
                // Obtener la dirección hacia adelante (forward) del jugador
                Vector3 direccion = jugador.transform.forward;
                // Instanciar el boomerang en la posición del jugador y con su rotación
                GameObject boomerang = Instantiate(boomerangPrefab, A.transform.position, Quaternion.identity);
                // Iniciar el movimiento del boomerang con la dirección hacia adelante del jugador
                boomerang.GetComponent<Boomerang>().IniciarMovimiento(direccion);
            }
               
}

public void EfectoVisual()
{
    vfx.Play();
}
    
public void MovimientoJugador()
{
    if (canMove && controller.isGrounded)
    {
       // Get the player's input
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");

    // Create a movement vector based on the player's input
    Vector3 moveInput = new Vector3(horizontal, vertical, 0);

         // Check if any movement keys are pressed
      if ((horizontal != 0 || vertical != 0) && enterAttack == false)
{
    // If any movement keys are pressed and the player is not attacking, set the "Run" parameter to true
    animator.SetBool("Run", true);

}
         else if (enterAttack == true) 
        {
            // If no movement keys are pressed, set the "Run" parameter to false
            animator.SetBool("Run", false);
        }
        else
        {
            // If the player is attacking, set the "Run" parameter to false
            animator.SetBool("Run", false);
        }

    moveInput = Camera.main.transform.TransformDirection(moveInput);

    // Transform the movement vector from the camera's local coordinates to world coordinates
    Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.z);

    // Normalize the movement vector to ensure the player's speed is constant
    moveDirection.Normalize();

    // Multiply the movement vector by the player's speed to get the final velocity
    moveDirection *= speed;

    // If the player is grounded or attacking, apply the movement
    if (controller.isGrounded || enterAttack)
    {
        controller.Move(moveDirection * Time.deltaTime);
    }

if (moveDirection != Vector3.zero) // Si el jugador se está moviendo
{
 if (moveDirection != Vector3.zero && !isLookingAtTarget && !enterAttack) // Evita la rotación cuando el jugador no se está moviendo
{
    // Obtén la rotación de la cámara
    Quaternion cameraRotation = Camera.main.transform.rotation;

    // Crea una nueva rotación que ignore la inclinación de la cámara
    Quaternion targetRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
 
     switch (vertical)
{
    case 1: // W key
        switch (horizontal)
        {
            case 1: // D key
                targetRotation *= Quaternion.Euler(0, 45, 0);
                break;
            case -1: // A key
                targetRotation *= Quaternion.Euler(0, -45, 0);
                break;
        }
        break;
    case -1: // S key
        switch (horizontal)
        {
            case 1: // D key
                targetRotation *= Quaternion.Euler(0, 135, 0);
                break;
            case -1: // A key
                targetRotation *= Quaternion.Euler(0, -135, 0);
                break;
            default:
                targetRotation *= Quaternion.Euler(0, 180, 0);
                break;
        }
        break;
    default:
        switch (horizontal)
        {
            case 1: // D key
                targetRotation *= Quaternion.Euler(0, 90, 0);
                break;
            case -1: // A key
                targetRotation *= Quaternion.Euler(0, -90, 0);
                break;
        }
        break;
        
}         
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);     
      
}        
else if (targetPosition != Vector3.zero) // Si el jugador no se está moviendo y hay una posición de click guardada
{
    // Calculate the target rotation
    Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

    // Apply the rotation to the player
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
}
    }      


}


    // Aplicar gravedad
    moveDirection.y -= gravity * Time.deltaTime;

    // Mover al personaje
    controller.Move(moveDirection * Time.deltaTime);
}
IEnumerator Dash()
{
    if (!isDashing && Time.time >= lastDashTime + dashTime)
    {
        float originalY = transform.position.y;

        isDashing = true;
        animator.SetBool("Dash", true);
        float startTime = Time.time;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Obtener las direcciones de la cámara
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Ignorar la componente vertical (y) de los vectores de la cámara
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalizar los vectores (después de modificarlos)
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Usar la transformación del jugador para calcular la dirección del dash
        Vector3 dashDirection = (horizontalInput == 0 && verticalInput == 0) 
            ? transform.forward 
            : (horizontalInput * cameraRight + verticalInput * cameraForward).normalized;

        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Walls"))
        {
            //wall.GetComponent<Collider>().enabled = false;
        }

        while (Time.time < startTime + dashTime)
        {
            float fractionOfDashTimePassed = (Time.time - startTime) / dashTime;
            float currentDashSpeed = Mathf.Lerp(DashSpeed, 0, fractionOfDashTimePassed);
            playerMovement.controller.Move(dashDirection * currentDashSpeed * Time.deltaTime);

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
   

   public void ApplyEffect(StatusEffect effect)
    {
        effect.ApplyEffect(gameObject);
    }
    public void RemoveEffect(StatusEffect effect)
    {
        effect.RemoveEffect(gameObject);
    }


   public void DamagePlayer()
{
    if (lifeInstance != null && lifeInstance.DamagePlayer == true)
    {
        animator.SetBool("Damage", true);
        cambioColoAlPegar.material.color = Color.red; // Cambia el color del material a rojo
    }
    else if (lifeInstance != null && lifeInstance.DamagePlayer == false)
    {
        animator.SetBool("Damage", false);
        cambioColoAlPegar.material.color = originalColor; // Restaura el color original del material
    }
    else if (lifeInstance == null)
    {
      
    }
}
 
    public void IncrementFloatVariable()
    {
   

    if (puntosUltimate < maxPuntosUltimate)
    {
       
        puntosUltimate += 1f;
    }
    else
    {
        
    }


    }


public void Ultimate()
{
    if (puntosUltimate >= maxPuntosUltimate && verificarArma == true)
    {
         if (Input.GetKeyDown(KeyCode.R))
        {        
                      
               GrimorioDistancia2();
             
                puntosUltimate = 0f;
            
        }
       
    }
    else if (puntosUltimate >= maxPuntosUltimate && verificarArma == false)
    {
        if (Input.GetKeyDown(KeyCode.R))
                {       
                   
                // Instantiate the objects
                for (int i = 0; i < 3; i++)
                {
                    objects[i] = Instantiate(objectPrefab);
                    StartCoroutine(OrbitObject(objects[i], i));
                }

               
                puntosUltimate = 0f;
            
        }
        else
        {
            isButtonPressed = false;
        }
    }

    if (ultimateImage != null)
    {
        ultimateImage.fillAmount = puntosUltimate / maxPuntosUltimate;

        if (ultimateImage.fillAmount == 1)
        {
            ultimateImage.color = Color.yellow; 
        }
        else
        {
            ultimateImage.color = Color.white;
        }
    }
}

private IEnumerator OrbitObject(GameObject obj, int index)
{
    float elapsedTime = 0f;

    while (elapsedTime < orbitDuration)
    {
        // Update the object's position
        float angle = index * 2f * Mathf.PI / 3f + rotationSpeedUlt * elapsedTime; // 120 degrees
        Vector3 pos = this.transform.position;
        obj.transform.position = new Vector3(pos.x + radius * Mathf.Cos(angle), pos.y +1, pos.z + radius * Mathf.Sin(angle));

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Optionally, destroy the object after it's finished orbiting
    Destroy(obj);
}
public void GrimorioDistancia2()
{
    GameObject A = GameObject.Find("ColliderGrimorios");
    GameObject B = GameObject.Find("ColliderGrimoriosB"); // Reemplaza "NombreDeTuGameObjectB" con el nombre real de tu GameObject
    GameObject C = GameObject.Find("ColliderGrimoriosC"); // Reemplaza "NombreDeTuGameObjectC" con el nombre real de tu GameObject
    GameObject jugador = GameObject.FindGameObjectWithTag(etiquetaJugador);

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


    if (jugador != null)
    {
        Vector3 direccion = jugador.transform.forward;
        // Instancia un boomerang en la posición de cada GameObject
        InstantiateBoomerangAtPosition(A.transform.position, direccion, boomerangPrefab);
        InstantiateBoomerangAtPosition(B.transform.position, direccion, boomerangPrefab);
        InstantiateBoomerangAtPosition(C.transform.position, direccion, boomerangPrefab);
    }   
}


private void InstantiateBoomerangAtPosition(Vector3 position, Vector3 direction, GameObject boomerangPrefab)
{
    GameObject boomerang = Instantiate(boomerangPrefab, position, Quaternion.identity);
    boomerang.GetComponent<Boomerang>().IniciarMovimiento(direction);
}

public void SpeedChange()
{
    Debug.Log("SpeedChange");

}



}