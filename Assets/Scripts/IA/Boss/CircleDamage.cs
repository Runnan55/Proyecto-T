using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDamage : MonoBehaviour
{
    public int damageAmount = 30; // Cantidad de daño que inflige el círculo
    private Vector3 direction; // Dirección de movimiento del robot
    public float moveSpeed = 2f; // Velocidad del robot
    public float lifetime = 8;
    public bool moveLeft = true; // Si es true, el robot se mueve a la izquierda, si es false a la derecha
    private bool canMove = true; // Controla si el objeto puede moverse

    private MeshRenderer mesh; // Componente VisualEffect
    private Collider parentCollider; // Collider del GameObject padre
    public float toggleInterval = 0.3f; // Intervalo en segundos para alternar el estado de los componentes

    private Transform parentTransform; // Transform del padre

    private void Start()
    {
        // Obtener los componentes
        mesh = GetComponent<MeshRenderer>();
        parentCollider = GetComponent<Collider>();
        parentTransform = transform.parent;

        // Llamar a la corrutina para alternar encender y apagar los componentes
        StartCoroutine(ToggleVisualEffectAndCollider());

        Destroy(gameObject, lifetime);

        // Definir la dirección de movimiento según moveLeft
        direction = moveLeft ? Vector3.left : Vector3.right;
    }

    private void Update()
    {
        if (canMove) // Solo se mueve si `canMove` es true
        {
            Move();
        }
    }

    private void Move()
    {
        // Mover el robot en la dirección X (izquierda o derecha) en relación con el sistema de coordenadas del padre
        transform.Translate(direction * moveSpeed * Time.deltaTime, parentTransform);
    }

    private void OnTriggerStay(Collider other)
    {
        // Verifica si el objeto que colisionó es el jugador
        if (other.CompareTag("Player"))
        {
            Life playerHealth = other.GetComponent<Life>();
            if (playerHealth != null)
            {
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
            canMove = true; // Permitir movimiento

            yield return new WaitForSeconds(toggleInterval);

            // Prender el VisualEffect y el Collider
            mesh.enabled = true;
            parentCollider.enabled = true;
            canMove = false; // Detener movimiento

            yield return new WaitForSeconds(toggleInterval);
        }
    }

    // Método para cambiar la dirección desde otro script
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }
}
