using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearMovement : MonoBehaviour
{
    [SerializeField, Tooltip("Velocidad de rotación en grados por segundo")]
    private float rotationSpeed = 10f;

    [SerializeField, Tooltip("Rotar hacia la derecha (true) o hacia la izquierda (false)")]
    private bool rotateRight = true;

    private Transform playerTransform;

    void Update()
    {
        float direction = rotateRight ? 1f : -1f;
        transform.Rotate(0, direction * rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerTransform.SetParent(transform);
            Debug.Log("ola");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform.SetParent(null);
            playerTransform = null;
            Debug.Log("adios");
        }
    }
}