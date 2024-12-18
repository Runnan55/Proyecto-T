using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorretaSimple : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab de la bala

    [Range(0.1f, 100f)]
    public float bulletSpeed = 10f; // Velocidad de la bala

    [Range(0.1f, 3f)]
    public float fireInterval = 1f; // Intervalo de disparo en segundos

    public List<Transform> firePoints; // Lista de Transforms desde los cuales se disparan las balas

    public int bulletsPerBurst = 3; // Cantidad de balas por ráfaga
    public float burstInterval = 2f; // Intervalo entre ráfagas

    private float timeSinceLastBurst = 0f; // Tiempo desde la última ráfaga 

    [Header("Movement Settings")]
    public bool moverse = false; // Variable para activar el movimiento
    public Transform puntoA; // Punto A
    public Transform puntoB; // Punto B
    public float velocidadMovimiento = 2f; // Velocidad de movimiento

    private bool moviendoHaciaB = true; // Indica si se está moviendo hacia el punto B

    void Update()
    {
        // Ajustar el tiempo acumulado usando la escala de tiempo del bullet time
        timeSinceLastBurst += Time.deltaTime * MovimientoJugador.bulletTimeScale;

        if (timeSinceLastBurst >= burstInterval)
        {
            StartCoroutine(FireBurst());
            timeSinceLastBurst = 0f;
        }

        // Mover la torreta si la variable moverse está activada
        if (moverse)
        {
            MoverTorreta();
        }
    }

    IEnumerator FireBurst()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            FireBullet();
            yield return new WaitForSeconds(fireInterval * MovimientoJugador.bulletTimeScale);
        }
    }

    void FireBullet()
    {
        // Usar los firePoints para determinar la posición de las balas
        foreach (Transform firePoint in firePoints)
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
            
        }
    }

    void MoverTorreta()
    {
        Transform objetivo = moviendoHaciaB ? puntoB : puntoA;
        transform.position = Vector3.MoveTowards(transform.position, objetivo.position, velocidadMovimiento * Time.deltaTime);

        if (transform.position == objetivo.position)
        {
            moviendoHaciaB = !moviendoHaciaB;
        }
    }
}