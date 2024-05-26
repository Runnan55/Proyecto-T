using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
        private Vector3 initialPosition; // Posición inicial del jugador
     // La última posición segura del jugador
    private List<Vector3> respawnPoints = new List<Vector3>(); // Lista de puntos de respawn disponibles
    private CharacterController characterController;

    void Start()
    {
        // Inicializa la posición inicial del jugador
        initialPosition = transform.position;
        characterController = GetComponent<CharacterController>();

        // Buscar objetos con el tag "Respawn" y añadir sus posiciones a la lista de puntos de respawn
        GameObject[] respawnObjects = GameObject.FindGameObjectsWithTag("Reaparecer");
        foreach (GameObject obj in respawnObjects)
        {
            respawnPoints.Add(obj.transform.position);
        }
    }    

    // Método para encontrar el punto de respawn más cercano al jugador
    private Vector3 FindNearestRespawnPoint()
    {
        if (respawnPoints.Count == 0)
        {
            // Si no hay puntos de respawn disponibles, regresa la posición inicial
            return initialPosition;
        }

        Vector3 closestPoint = respawnPoints[0];
        float closestDistance = Vector3.Distance(transform.position, closestPoint);

        foreach (Vector3 point in respawnPoints)
        {
            float distance = Vector3.Distance(transform.position, point);
            if (distance < closestDistance)
            {
                closestPoint = point;
                closestDistance = distance;
            }
        }

        return closestPoint;
    }

    // Método para hacer respawn en el punto de respawn más cercano al jugador
    public void RespawnAtNearestPoint()
    {
        Vector3 nearestPoint = FindNearestRespawnPoint();
        StartCoroutine(TeleportPlayer(nearestPoint));
    }

    // Método para teletransportar al jugador con desactivación temporal del CharacterController
    private IEnumerator TeleportPlayer(Vector3 newPosition)
    {
        // Desactivar CharacterController
        characterController.enabled = false;

        // Teletransportar al jugador
        transform.position = newPosition;

        // Esperar un frame para asegurarse de que la posición se actualice
        yield return null;

        // Volver a activar CharacterController
        characterController.enabled = true;
    }

}
