using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDamage : MonoBehaviour
{
    public int damageAmount = 30; // Cantidad de da�o que inflige el c�rculo
    private Vector3 direction; // Direcci�n de movimiento del robot
    public float moveSpeed = 2f; // Velocidad del robot
    public float lifetime = 8;
    public bool moveLeft = true; // Si es true, el robot se mueve a la izquierda, si es false a la derecha

    private MeshRenderer mesh; // Componente VisualEffect
    private Collider parentCollider; // Collider del GameObject padre
    public float toggleInterval = 0.3f; // Intervalo en segundos para alternar el estado de los componentes (ajustado a 0.3 segundos)

    private Transform parentTransform; // Transform del padre

    private void Start()
    {
        // Obtener los componentes
        mesh = GetComponent<MeshRenderer>();  // Asumiendo que el hijo es el primero
        parentCollider = GetComponent<Collider>(); // Collider del GameObject padre
        parentTransform = transform.parent; // Obtener la transformaci�n del padre

        // Llamar a la corrutina para alternar encender y apagar los componentes
        StartCoroutine(ToggleVisualEffectAndCollider());

        Destroy(gameObject, lifetime);

        // Definir la direcci�n de movimiento seg�n moveLeft
        direction = moveLeft ? Vector3.left : Vector3.right;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // Mover el robot en la direcci�n X (izquierda o derecha) en relaci�n con el sistema de coordenadas del padre
        transform.Translate(direction * moveSpeed * Time.deltaTime, parentTransform);
    }

    private void OnTriggerStay(Collider other)
    {
        // Verifica si el objeto que colision� es el jugador
        if (other.CompareTag("Player"))
        {
            // Obt�n el componente de salud del jugador (supongamos que tiene un script llamado Life)
            Life playerHealth = other.GetComponent<Life>();

            // Si el jugador tiene el componente de salud
            if (playerHealth != null)
            {
                // Aplica el da�o al jugador
                playerHealth.ModifyTime(-damageAmount);
            }
        }
    }

    // Corrutina para alternar el estado de los componentes
    private IEnumerator ToggleVisualEffectAndCollider()
    {
        while (true)
        {
            // Apagar el VisualEffect y el Collider
            mesh.enabled = false;
            parentCollider.enabled = false;

            // Esperar 0.3 segundos
            yield return new WaitForSeconds(toggleInterval);

            // Prender el VisualEffect y el Collider
            mesh.enabled = true;
            parentCollider.enabled = true;

            // Esperar 0.3 segundos
            yield return new WaitForSeconds(toggleInterval);
        }
    }

    // M�todo para cambiar la direcci�n desde otro script
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }
}