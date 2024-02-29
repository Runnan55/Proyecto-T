using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class movimientopocho : MonoBehaviour
{
       public float speed = 6.0f;
       public float rotationSpeed = 10.0f;
       public float gravity = 20.0f;

       public float DashSpeed;

       public float dashTime;  

       movimientopocho playerMovement;

       private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
   
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<movimientopocho>();
    }

    void Update()
    {
        MovimientoJugador();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());
        }
    }

    public void MovimientoJugador()
{
    if (controller.isGrounded)
    {
        // Movimiento horizontal
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection.Normalize(); // Normalizar el vector de movimiento
        moveDirection *= speed;

  // Rotar al jugador en la dirección del movimiento
        if (moveDirection != Vector3.zero) // Evita la rotación cuando el jugador no se está moviendo
        {
             // Determinar la dirección de la rotación basada en la entrada del jugador
    int horizontal = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
    int vertical = Mathf.RoundToInt(Input.GetAxis("Vertical"));

    switch (horizontal)
    {
        case 1: // D key
            switch (vertical)
            {
                case 1: // W key
                    transform.rotation = Quaternion.Euler(0, 45, 0);
                    break;
                case -1: // S key
                    transform.rotation = Quaternion.Euler(0, 135, 0);
                    break;
                default:
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
            }
            break;
        case -1: // A key
            switch (vertical)
            {
                case 1: // W key
                    transform.rotation = Quaternion.Euler(0, -45, 0);
                    break;
                case -1: // S key
                    transform.rotation = Quaternion.Euler(0, -135, 0);
                    break;
                default:
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;
            }
            break;
        default:
            switch (vertical)
            {
                case 1: // W key
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case -1: // S key
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
            break;
    }
        }
    }

    // Dash
    if (Input.GetKey(KeyCode.LeftShift))
    {
        Dash();
    }

    // Aplicar gravedad
    moveDirection.y -= gravity * Time.deltaTime;

    // Mover al personaje
    controller.Move(moveDirection * Time.deltaTime);
}
    IEnumerator Dash()
    {

      float startTime = Time.time;

      while (Time.time < startTime + dashTime)
      {
        playerMovement.controller.Move(playerMovement.transform.forward * DashSpeed * Time.deltaTime);
        yield return null;
      }

    }


    public void AplicarCarta() // Método que se ejecuta al aplicar una carta
    {
       //Dani aqui tienes el metodo
       
    }


}
