using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTp : MonoBehaviour
{
    private Transform targetPosition;

    private void Start()
    {
        if (transform.childCount > 0)
        {
            targetPosition = transform.GetChild(0);
        }
        else
        {
            Debug.LogError("El objeto FallTp no tiene un hijo asignado como destino del teletransporte.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && targetPosition != null)
        {
            other.transform.position = targetPosition.position;
            Debug.Log("Jugador teletransportado a la posición del hijo.");
        }
    }
}