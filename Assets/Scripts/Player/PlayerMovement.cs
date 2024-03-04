using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
       public float speed = 6.0f;

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

    {


     if (Input.GetMouseButtonDown(0)) 
   {
 
            Iniciar_combo();

            // Marcar que hay un ataque en curso
 
        // Convertir la posición del click del ratón a una posición en el mundo 3D
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit))
    {
        // Hacer que el objeto mire hacia la posición del click
        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        transform.LookAt(targetPosition);
     
    }


    } 


    }



   
}

  public
void Iniciar_combo()
  {
    if(puedo_dar_cliks)
    {
        cantidad_clik++;
    }

    if (cantidad_clik == 1)
    {
      animator.SetInteger("attack", 1);
    }
  
  }

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
     // Obtén la entrada del jugador
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");

    // Crea un vector de movimiento basado en la entrada del jugador
    Vector3 moveInput = new Vector3(horizontal, 0, vertical);

    // Transforma el vector de movimiento desde las coordenadas locales de la cámara a las coordenadas del mundo
    Vector3 moveDirection = Camera.main.transform.TransformDirection(moveInput);

    // Normaliza el vector de movimiento para asegurarte de que la velocidad del jugador es constante
    moveDirection.Normalize();

    // Multiplica el vector de movimiento por la velocidad del jugador para obtener la velocidad final
    moveDirection *= speed;

    // Aplica el movimiento al jugador
    controller.Move(moveDirection * Time.deltaTime);
         
if (moveDirection != Vector3.zero && !isLookingAtTarget) // Evita la rotación cuando el jugador no se está moviendo
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

    // Interpolar suavemente hacia la rotación objetivo
    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
}
    }
       
 

    // Dash
    if (Input.GetKey(KeyCode.LeftShift))
    {
        Dash();
        animator.SetBool("Dash", true);
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


    public void AplicarCarta() // Método que se ejecuta al aplicar una carta
    {
       //Dani aqui tienes el metodo
       
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
}

