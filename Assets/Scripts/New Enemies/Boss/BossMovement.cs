using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public Transform[] zonePoints; // Array de puntos fijos para las zonas
    private Transform player;       // Referencia al jugador
    public float moveSpeed = 5f;   // Velocidad de movimiento del Boss
    public float rotationSpeed = 5f; // Velocidad para ajustar la rotación

    private Vector3 currentTarget; // Punto al que se mueve el Boss

    void Start()
    {
        // Inicializa el objetivo al punto de la primera zona por defecto (opcional)
        currentTarget = zonePoints[0].position;
        StartCoroutine(FindPlayerWithDelay());

    }

    private IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'");
        }
    }
    // Este método es llamado cuando el jugador entra en una zona
    public void OnPlayerEnterZone(int zoneID)
    {
        if (zoneID >= 0 && zoneID < zonePoints.Length)
        {
            currentTarget = zonePoints[zoneID].position; // Actualiza el destino del Boss
        }
    }

    void Update()
    {
        MoveTowardsTarget(); // Mueve al Boss hacia el destino actual
        RotateTowardsPlayer(); // Ajusta la rotación del Boss para mirar al jugador
    }

    // Mueve al Boss hacia el punto objetivo
    private void MoveTowardsTarget()
    {
        // Mueve al Boss hacia el objetivo (sin NavMesh)
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);
    }

    // Ajusta la rotación del Boss para mirar hacia el jugador
    void RotateTowardsPlayer()
    {
        // Calcula la dirección hacia el jugador
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Ignora la componente Y para que no se incline

        // Calcula la rotación deseada para mirar al jugador
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Aplica la rotación de manera suave
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}