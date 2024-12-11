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

    public Transform firePoint; // Transform desde el cual se disparan las balas

    public int bulletsPerBurst = 3; // Cantidad de balas por ráfaga
    public float burstInterval = 2f; // Intervalo entre ráfagas

    private float timeSinceLastBurst = 0f; // Tiempo desde la última ráfaga 

    void Update()
    {
        // Ajustar el tiempo acumulado usando la escala de tiempo del bullet time
        timeSinceLastBurst += Time.deltaTime * MovimientoJugador.bulletTimeScale;

        if (timeSinceLastBurst >= burstInterval)
        {
            StartCoroutine(FireBurst());
            timeSinceLastBurst = 0f;
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
        // Usar el firePoint para determinar la posición de la bala
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
