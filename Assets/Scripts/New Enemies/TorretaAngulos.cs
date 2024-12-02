using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorretaAngulos : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab de la bala
    [Range(0.1f, 100f)]
    public float bulletSpeed = 10f; // Velocidad de la bala
    [Range(0.1f, 3f)]
    public float fireInterval = 1f; // Intervalo entre disparos
    public Transform firePoint; // El punto desde donde se disparan las balas
    public float rotationAngle = 45f; // Ángulo de rotación por ciclo
    public float rotationSpeed = 30f; // Velocidad de rotación

    private Quaternion initialRotation; // Rotación inicial de la torreta
    private bool isRotating = false; // Para verificar si está rotando

    void Start()
    {
        initialRotation = transform.rotation; // Guardamos la rotación inicial
        StartCoroutine(RotationCycle()); // Comenzamos el ciclo de rotación
    }

    IEnumerator RotationCycle()
    {
        while (true)
        {
            // Primera parte: Rotación en una dirección
            yield return StartCoroutine(RotateAndFire(rotationAngle));

            // Segunda parte: Vuelta a la rotación inicial
            yield return StartCoroutine(RotateAndFire(-rotationAngle));
        }
    }

    IEnumerator RotateAndFire(float angle)
    {
        float targetRotation = transform.eulerAngles.y + angle;
        float startRotation = transform.eulerAngles.y;

        // Rotación hacia el ángulo objetivo
        float timeToRotate = Mathf.Abs(angle) / rotationSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < timeToRotate)
        {
            elapsedTime += Time.deltaTime;
            float currentRotation = Mathf.Lerp(startRotation, targetRotation, elapsedTime / timeToRotate);
            transform.rotation = Quaternion.Euler(0, currentRotation, 0); // Rotamos solo en el eje Y
            yield return null;
        }

        // Garantizar que la rotación es exacta
        transform.rotation = Quaternion.Euler(0, targetRotation, 0);

        // Disparar después de la rotación
        FireBullet();
    }

    void FireBullet()
    {
        if (firePoint != null)
        {
            Vector3 spawnPosition = firePoint.position; // Usamos la posición del firePoint
            Quaternion spawnRotation = firePoint.rotation; // Usamos la rotación del firePoint

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);
            TestBullet bulletScript = bullet.GetComponent<TestBullet>();
            if (bulletScript != null)
            {
                bulletScript.speed = bulletSpeed;
            }
        }
        else
        {
            Debug.LogWarning("El firePoint no ha sido asignado en el Inspector.");
        }
    }
}