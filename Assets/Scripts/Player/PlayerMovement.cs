using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
       public float speed = 6.0f;
       public float rotationSpeed = 10.0f;
       public float gravity = 20.0f;

       public float DashSpeed;

       public float dashTime;  

       PlayerMovement playerMovement;
       private CharacterController controller;

    private Vector3 moveDirection = Vector3.zero;
    
    public GameObject ataqueProvisional; // El prefab que se instanciará al hacer click
    [Header("Ataque 1")]
    public Transform ataque1Inicio;
    public Transform ataque1Final; // El objeto vacío hacia el que quieres mover el GameObject instanciado
    bool canMove = true;
    [Header("Ataque 2")]
    public Transform ataque2Inicio;
    public Transform ataque2Final; // El segundo objeto vacío hacia el que quieres mover el GameObject instanciado
   private bool isAttacking = false; 
 [Header("Ataque 3")]
   public Transform ataque3Inicio;
   public Transform ataque3Final; // El segundo objeto vacío hacia el que quieres mover el GameObject instanciado
   bool isLookingAtTarget = false;
  public int attackState = 0;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

void Update()
{
    if (canMove)
    {
        MovimientoJugador();
    }

    if (Input.GetKeyDown(KeyCode.Mouse0)) // Suponiendo que el espacio es la tecla para atacar
    {
        canMove = true;
        Atacar();
      
    }

    if (Input.GetKeyDown(KeyCode.LeftShift))
    {
        StartCoroutine(Dash());
    }

    
        AplicarCarta();
   
}


public void MovimientoJugador()
{
    if (controller.isGrounded)
    {
        // Movimiento horizontal
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDirection.Normalize(); // Normalizar el vector de movimiento
        moveDirection *= speed;

        if (moveDirection != Vector3.zero && !isLookingAtTarget) // Evita la rotación cuando el jugador no se está moviendo
        {
            // Determinar la dirección de la rotación basada en la entrada del jugador
            int horizontal = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
            int vertical = Mathf.RoundToInt(Input.GetAxis("Vertical"));

            Quaternion targetRotation = transform.rotation; // Inicializar con la rotación actual

            switch (horizontal)
            {
                case 1: // D key
                    switch (vertical)
                    {
                        case 1: // W key
                            targetRotation = Quaternion.Euler(0, 45, 0);
                            break;
                        case -1: // S key
                            targetRotation = Quaternion.Euler(0, 135, 0);
                            break;
                        default:
                            targetRotation = Quaternion.Euler(0, 90, 0);
                            break;
                    }
                    break;
                case -1: // A key
                    switch (vertical)
                    {
                        case 1: // W key
                            targetRotation = Quaternion.Euler(0, -45, 0);
                            break;
                        case -1: // S key
                            targetRotation = Quaternion.Euler(0, -135, 0);
                            break;
                        default:
                            targetRotation = Quaternion.Euler(0, -90, 0);
                            break;
                    }
                    break;
                default:
                    switch (vertical)
                    {
                        case 1: // W key
                            targetRotation = Quaternion.Euler(0, 0, 0);
                            break;
                        case -1: // S key
                            targetRotation = Quaternion.Euler(0, 180, 0);
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
    }

    // Aplicar gravedad
    moveDirection.y -= gravity * Time.deltaTime;

    // Mover al personaje
    controller.Move(moveDirection * Time.deltaTime);
}





private void StartAttack()
{
  // Marcar que hay un ataque en curso
    isAttacking = true;
    isLookingAtTarget = true;

    // Convertir la posición del click del ratón a una posición en el mundo 3D
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit))
    {
        // Hacer que el objeto mire hacia la posición del click
        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        transform.LookAt(targetPosition);

        GameObject instance;
        // Instanciar el GameObject en la posición del objeto vacío y como hijo del objeto vacío
        if (attackState == 0)
        {
            instance = Instantiate(ataqueProvisional, ataque1Inicio.position, Quaternion.identity);
            instance.transform.SetParent(ataque1Inicio, true);
        }
        else if (attackState == 1)
        {
            instance = Instantiate(ataqueProvisional, ataque2Inicio.position, Quaternion.identity);
            instance.transform.SetParent(ataque2Inicio, true);
        }
        else
        {
            instance = Instantiate(ataqueProvisional, ataque3Inicio.position, Quaternion.identity);
            instance.transform.SetParent(ataque3Inicio, true);
        }

        // Iniciar la coroutine para mover el GameObject instanciado hacia la posición del segundo objeto vacío
        StartCoroutine(MoveToTarget(instance));
    }
}

public void Atacar()
{
    // Si el botón izquierdo del ratón está presionado...
    if (Input.GetMouseButtonDown(0))
    {
        // Si no hay un ataque en curso, iniciar un nuevo ataque
        if (!isAttacking)
        {
            StartAttack();
        }
    }
}
IEnumerator MoveToTarget(GameObject instance)
{
    Vector3 targetPosition;

    while (true) // Loop infinito hasta que el objeto llegue a su destino
    {
        // Actualizar la posición final basada en el estado de ataque actual
        if (attackState == 0)
        {
            targetPosition = ataque1Final.position;
        }
        else if (attackState == 1)
        {
            targetPosition = ataque2Final.position;
        }
        else
        {
            targetPosition = ataque3Final.position;
        }

        // Si el objeto ha llegado a su destino, salir del loop
        if (Vector3.Distance(instance.transform.position, targetPosition) <= 0.1f)
        {
            break;
        }

        // Mover el objeto instanciado hacia la posición final
        instance.transform.position = Vector3.MoveTowards(instance.transform.position, targetPosition, speed * Time.deltaTime);

        // Hacer que el objeto instanciado rote junto con el personaje
        instance.transform.rotation = transform.rotation;

        yield return null; // Esperar hasta el próximo frame
    }

      // Destruir el GameObject instanciado una vez que llegue a su destino
    Destroy(instance);

    // Cambiar attackState después de que el objeto ha sido destruido
    attackState = (attackState + 1) % 3;

    // Esperar un pequeño retraso antes de marcar que el ataque ha terminado
    yield return new WaitForSeconds(0.1f);
    isAttacking = false;
    isLookingAtTarget = false;
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
