using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
       public float speed = 6.0f;
    public float maxSpeed = 6.0f;
       public float reduccionVelocidad =6.0f;
       public float rotationSpeed = 10.0f;
       public float gravity = 20.0f;

       public float DashSpeed;
       private float lastDashTime = -Mathf.Infinity;

       public float dashTime;  

       PlayerMovement playerMovement;
       private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    bool canMove = true;   
    bool isLookingAtTarget = false;
    public bool isAttacking = false;
   public bool hasRotated = false; // Añade esta variable al principio de tu clase
    Vector3 targetPosition;
   public Animator animator;
  int cantidad_clik;
  bool puedo_dar_cliks;
    void Start()
    {
         controller = GetComponent<CharacterController>();
         playerMovement = GetComponent<PlayerMovement>();

         animator = GetComponent<Animator>();
         cantidad_clik = 0;
         puedo_dar_cliks = true;

         
    }

void Update()
{
    if (canMove)
    {
        MovimientoJugador();
    }   


    if (Input.GetKeyDown(KeyCode.LeftShift))
    {
        StartCoroutine(Dash());
    }



if (Input.GetMouseButtonDown(0)) 
{
    isAttacking = true;
    Iniciar_combo();    
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

          hasRotated = true; // Establece hasRotated en true después de la rotación
    }
} 
   
}

public void StartAttack()
{
    isAttacking = true;
}

public void EndAttack()
{
    isAttacking = false;
}
  public void Iniciar_combo()
  {
cantidad_clik++;

    if (cantidad_clik == 1)
    {
        animator.SetInteger("attack", 1);
        hasRotated = false;
    }
  
  }


  #region 	NoRotar
public void NoRotar()
{
     if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack0") )
      {
        hasRotated = false;
        speed = maxSpeed;
      }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") )
      {
        hasRotated = false;
        
      }
         else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") )
      {
        hasRotated = false;
      }  
      else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") )
      {
        hasRotated = false;
        
      }  
       else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") )
      {
        hasRotated = false;
      }  
}
#endregion


    public void Verificar_combo()
    {
        puedo_dar_cliks = false;

      if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack0") && cantidad_clik == 1)
      {
        animator.SetInteger("attack", 0);
         puedo_dar_cliks = true;
         cantidad_clik = 0;
      }
      else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack0") && cantidad_clik >= 2)
      {
        animator.SetInteger("attack", 2);
        puedo_dar_cliks = true;
      }
      else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && cantidad_clik == 2)
      {
        animator.SetInteger("attack", 0);
         puedo_dar_cliks = true; 
        cantidad_clik = 0;
         hasRotated = false;
        
      }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && cantidad_clik >= 3)
      {
        animator.SetInteger("attack", 3);
        puedo_dar_cliks = true;
        
      }
         else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") )
      {
        animator.SetInteger("attack", 0); 
        puedo_dar_cliks = true;      
         cantidad_clik = 0;
          hasRotated = false;
      }     
         else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") && cantidad_clik <= 4)
      {
        animator.SetInteger("attack", 0); 
        puedo_dar_cliks = true;      
         cantidad_clik = 0;
      }           
    
    }
public void MovimientoJugador()
{
   if (controller.isGrounded)
    {
       // Get the player's input
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");

    // Create a movement vector based on the player's input
    Vector3 moveInput = new Vector3(horizontal, 0, vertical);

         // Check if any movement keys are pressed
      if ((horizontal != 0 || vertical != 0) && isAttacking == false)
{
    // If any movement keys are pressed and the player is not attacking, set the "Run" parameter to true
    animator.SetBool("Run", true);
    speed = maxSpeed;
}
         else if (isAttacking == true) 
        {
            // If no movement keys are pressed, set the "Run" parameter to false
            animator.SetBool("Run", false);
        }
        else
        {
            // If the player is attacking, set the "Run" parameter to false
            animator.SetBool("Run", false);
        }

    // Transform the movement vector from the camera's local coordinates to world coordinates
    Vector3 moveDirection = Camera.main.transform.TransformDirection(moveInput);

    // Normalize the movement vector to ensure the player's speed is constant
    moveDirection.Normalize();

    // Multiply the movement vector by the player's speed to get the final velocity
    moveDirection *= speed;

    // If the player is grounded or attacking, apply the movement
    if (controller.isGrounded || isAttacking)
    {
        controller.Move(moveDirection * Time.deltaTime);
    }

if (moveDirection != Vector3.zero) // Si el jugador se está moviendo
{
           if (moveDirection != Vector3.zero && !isLookingAtTarget && !isAttacking) // Evita la rotación cuando el jugador no se está moviendo
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
    transform.rotation = targetRotation;        
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

    // Dash
    if (Input.GetKey(KeyCode.LeftShift))
    {
       
        animator.SetBool("Dash", true);
        //BERTO PON AUDIO DE INICIO DE DASH
    }
    else
    {
        animator.SetBool("Dash", false);
        
        
    }

    // Aplicar gravedad
    moveDirection.y -= gravity * Time.deltaTime;

    // Mover al personaje
    controller.Move(moveDirection * Time.deltaTime);
}
 IEnumerator Dash()
{
    if (Time.time >= lastDashTime + dashTime)
    {
        float startTime = Time.time;

        // Obtener la dirección de entrada del jugador
        Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // Obtener la dirección de la cámara
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Eliminar la inclinación de la cámara
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        // Calcular la dirección del dash relativa a la orientación de la cámara
        Vector3 dashDirection = cameraForward * playerInput.z + cameraRight * playerInput.x;

        while (Time.time < startTime + dashTime)
        {
            playerMovement.controller.Move(dashDirection * DashSpeed * Time.deltaTime);
            yield return null;
        }

        lastDashTime = Time.time;
    }
}

   
public Collider espada; // Añade esta línea
   public void ActivarEspada()
   {
       espada.enabled = true; // Cambia esta línea
       
   }

   public void DesactivarEspada()
   {
       espada.enabled = false; // Y esta línea
   }

   public void ReduccionVelocidad()
   {
       speed /=reduccionVelocidad;

       //BERTO PON AUDIO DE INICIO DE ATAQUE
   }

 
   public void VelocidadMaxima()
   {
       speed = maxSpeed;
   }
}

