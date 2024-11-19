using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public bool isBidirectional = false;
    public Transform entryPoint;
    public Transform exitPoint;

    void Start()
    {
        if (entryPoint == null || exitPoint == null)
        {
            Debug.LogError("EntryPoint or ExitPoint is not assigned.");
        }
    }

    public void Teleport(GameObject player, Transform targetPoint)
    {
        Debug.Log("ola");

        if (targetPoint != null)
        {
            Debug.Log("oli");

            // Desactiva temporalmente el Rigidbody y CharacterController del jugador si existen
            Rigidbody rb = player.GetComponent<Rigidbody>();
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero; // Detén cualquier movimiento
                rb.angularVelocity = Vector3.zero; // Detén cualquier rotación
            }

            // Mueve al jugador a la posición del punto de destino
            player.transform.position = targetPoint.position;

            // Reactiva el CharacterController y Rigidbody del jugador si existen
            if (controller != null) controller.enabled = true;
            if (rb != null) rb.isKinematic = false;

            Debug.Log("Player teleported to: " + targetPoint.position);
        }
        else
        {
            Debug.LogError("TargetPoint is not assigned.");
        }
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (entryPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(entryPoint.position, 0.75f);
            Gizmos.DrawWireSphere(entryPoint.position, 0.75f);
        }

        if (exitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(exitPoint.position, 0.75f);
            Gizmos.DrawWireSphere(exitPoint.position, 0.75f);
        }
    }
    #endif
}
