using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollowPlayer : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float delayTime = 0.5f;

    void Start()
    {
        StartCoroutine(FindAndAssignPlayer());
    }

    IEnumerator FindAndAssignPlayer()
    {
        yield return new WaitForSeconds(delayTime);

        if (virtualCamera.Follow == null) // Verifica si el seguimiento está desactivado
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
            {
                virtualCamera.Follow = player.transform;
                Debug.Log("Camera follow asignado al jugador.");
            }
            else
            {
                Debug.LogWarning("Camera follow: Player no encontrado");
            }
        }
        else
        {
            Debug.Log("Camera follow ya está configurado. No se reasigna.");
        }
    }
}