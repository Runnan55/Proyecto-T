using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public bool isBidirectional = false;
    public Transform entryPoint;
    public Transform exitPoint;
    public float teleportCooldown = 1.0f;
    private float lastTeleportTime = -Mathf.Infinity;
    public bool hasTransition = true;

    void Start()
    {
        if (entryPoint == null || exitPoint == null)
        {
            Debug.LogError("EntryPoint or ExitPoint is not assigned.");
        }
    }

    public void Teleport(GameObject player, Transform targetPoint)
    {
        if (Time.time - lastTeleportTime < teleportCooldown)
        {
            return;
        }

        lastTeleportTime = Time.time;

        if (hasTransition)
        {
            StartCoroutine(TeleportWithTransition(player, targetPoint));
        }

        else
        {
            PerformTeleport(player, targetPoint);
        }
    }

    private IEnumerator TeleportWithTransition(GameObject player, Transform targetPoint)
    {
        DefaultHUD.Instance.EnableTransition();
        yield return new WaitForSeconds(0.45f);
        PerformTeleport(player, targetPoint);
        yield return new WaitForSeconds(0.55f);
        DefaultHUD.Instance.DisableTransition();
    }

    private void PerformTeleport(GameObject player, Transform targetPoint)
    {
        if (targetPoint != null)
        {
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

            player.transform.position = targetPoint.position;

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