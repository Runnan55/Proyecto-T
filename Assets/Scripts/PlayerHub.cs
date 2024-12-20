using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHub : MonoBehaviour
{

       public float speed = 15.0f;    
       public float rotationSpeed = 10.0f;
       public float gravity = 20.0f;
       private CharacterController controller;
       private Vector3 moveDirection = Vector3.zero;
       bool canMove = true;   
       bool isLookingAtTarget = false;      
       public static bool hasRotated = false; // Añade esta variable al principio de tu clase
       Vector3 targetPosition;
       public Animator animator;   
       public static bool enterAttack = false;
    

        

    // Start is called before the first frame update
    void Start()
    {
            controller = GetComponent<CharacterController>();
        

         animator = GetComponent<Animator>();

    
    }

    // Update is called once per frame
    void Update()
    {
       if (canMove)
    {
        MovimientoJugador();
    }   
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
}
