using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{ public Transform target; // El jugador u objeto a seguir
    public float smoothSpeed = 0.125f; // La velocidad de suavizado
    private Vector3 offset; // La distancia inicial entre la cámara y el jugador

    private void Start()
    {
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    }

