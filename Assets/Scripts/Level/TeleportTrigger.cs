using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    public Teleporter teleporter;
    public bool isEntry;
    private bool playerInTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInTrigger)
        {
            playerInTrigger = true;
            Debug.Log("Player entered trigger");

            if (isEntry)
            {
                teleporter.Teleport(other.gameObject, teleporter.exitPoint);
            }
            else if (teleporter.isBidirectional)
            {
                teleporter.Teleport(other.gameObject, teleporter.entryPoint);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            Debug.Log("Player exited trigger");
        }
    }
}
