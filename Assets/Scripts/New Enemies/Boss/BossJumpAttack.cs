using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJumpAttack : MonoBehaviour
{
    public float jumpHeight = 5f;      // Altura del salto
    public float jumpSpeed = 2f;       // Velocidad de ascenso
    public float fallSpeed = 4f;       // Velocidad de ca�da
    public float jumpPreviewDuration = 3f; // Duraci�n de la previsualizaci�n antes de caer

    private bool isJumping = false;    // Estado del salto
    private Vector3 targetPosition;    // Posici�n de destino para el salto
    private float jumpStartY;          // Altura desde donde comienza el salto
    private float jumpTime = 0f;       // Tiempo transcurrido del salto

    private BossMovement bossMovement; // Referencia al BossMovement para controlar el estado

    void Start()
    {
        // Obt�n la referencia al BossMovement
        bossMovement = GetComponent<BossMovement>();
    }

    public void SaltoAplastante(Vector3 target)
    {
        // Inicia el salto y marca la zona de aterrizaje
        targetPosition = target;
        targetPosition.y = transform.position.y;  // Mantiene la altura actual en Y para el salto

        isJumping = true;   // Comienza el salto
        jumpStartY = transform.position.y;  // Establece la altura inicial del salto
        jumpTime = 0f;      // Reinicia el contador de tiempo

        // Previsualizaci�n del salto (puedes agregar efectos visuales aqu�)
        ShowJumpPreview(targetPosition);
    }

    void ShowJumpPreview(Vector3 target)
    {
        Debug.DrawLine(transform.position, target, Color.red, jumpPreviewDuration);  // Solo para debug
    }

    void Update()
    {
        if (isJumping)
        {
            jumpTime += Time.deltaTime * jumpSpeed;

            // Elevar al Boss
            if (jumpTime < 1f)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, jumpStartY + jumpHeight, transform.position.z), jumpTime);
            }
            else
            {
                // Mover al Boss hacia la zona de aterrizaje
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

                // Cuando llega al destino, ejecutar la onda expansiva
                if (transform.position == targetPosition)
                {
                    OndaExpansiva();
                    isJumping = false;  // Termina el salto
                }
            }
        }
    }

    void OndaExpansiva()
    {
        Debug.Log("Realizando Onda Expansiva");

        // Aqu� agregas la l�gica para la onda expansiva
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Aplica da�o al jugador
                // hitCollider.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }
    }
}