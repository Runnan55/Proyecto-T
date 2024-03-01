using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
       public float speed = 6.0f;
       public float atackSpeed = 6.0f;
       public float rotationSpeed = 10.0f;
       public float gravity = 20.0f;

       public float DashSpeed;
       private float lastDashTime = -Mathf.Infinity;

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

speed /= 2;
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

        
        StartCoroutine(MoveToTarget(instance));
    }
}

public void Atacar()
{
    
    if (Input.GetMouseButtonDown(0))
    {
       
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
        instance.transform.position = Vector3.MoveTowards(instance.transform.position, targetPosition, atackSpeed * Time.deltaTime);

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

    speed *= 2;
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


}

