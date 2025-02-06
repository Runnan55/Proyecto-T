using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GearMovement : MonoBehaviour
{
    [SerializeField, Tooltip("Velocidad de rotación en grados por segundo")]
    private float rotationSpeed = 10f;

    [SerializeField, Tooltip("Rotar hacia la derecha (true) o hacia la izquierda (false)")]
    private bool rotateRight = true;

    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private Vector3 lastGearPosition; // Para calcular desplazamiento del engranaje

    void Start()
    {
        // Registrar la posición inicial del engranaje
        lastGearPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Rotar el engranaje
        float direction = rotateRight ? 1f : -1f;
        transform.Rotate(0, direction * rotationSpeed * Time.fixedDeltaTime, 0);

        // Si el jugador está sobre el engranaje, moverlo con este
        if (playerTransform != null)
{
        // Calcular la rotación del jugador alrededor del centro del engranaje
        Vector3 pivot = transform.position; // Centro del engranaje
        Vector3 offset = playerTransform.position - pivot; // Posición relativa del jugador al engranaje

        float angle = direction * rotationSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = Quaternion.Euler(0, angle, 0) * offset + pivot;

        // Calcular el desplazamiento total (rotación + traslación si el engranaje se mueve)
        Vector3 gearMovement = transform.position - lastGearPosition;
        Vector3 finalMovement = newPosition - playerTransform.position + gearMovement;

        // Mover al jugador con el Rigidbody
        playerRigidbody.MovePosition(playerRigidbody.position + finalMovement);
}
        // Actualizar la última posición del engranaje
        lastGearPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detectar si es el jugador
        {
            playerTransform = other.transform;
            playerRigidbody = other.GetComponent<Rigidbody>(); // Obtener el CharacterController
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
            playerRigidbody = null; // Limpiar la referencia al salir del engranaje
        }
    }

    private void StopMovement()
    {
        rotationSpeed = 0;
    }

    private void ResumeMovement()
    {
        rotationSpeed = 10;
    }
}