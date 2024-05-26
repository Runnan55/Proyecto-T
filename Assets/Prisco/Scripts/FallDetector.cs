using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector: MonoBehaviour
{
      private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                // Respawn en el punto de respawn más cercano en lugar de en la posición inicial del nivel
                playerRespawn.RespawnAtNearestPoint();
            }
        }
    }
}
