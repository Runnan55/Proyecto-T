using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraRooms : MonoBehaviour
{
    public Transform target; // La posición actual hacia la que la cámara debe moverse
    public float smoothSpeed = 0.125f; // Velocidad de suavizado del movimiento de la cámara

    private void LateUpdate()
    {
        if (target != null)
        {
            // Interpola la posición de la cámara hacia la posición del target
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, target.position, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    // Método para cambiar el objetivo de la cámara
    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
