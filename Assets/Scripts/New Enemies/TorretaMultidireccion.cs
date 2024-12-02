using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorretaMultidireccion : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab de la bala
    [Range(0.1f, 100f)]
    public float bulletSpeed = 10f; // Velocidad de la bala
    [Range(0.1f, 3f)]
    public float fireInterval = 1f; // Intervalo de disparo en segundos
    public List<Transform> fireDirections1 = new List<Transform>(); // Lista de direcciones para la primera r�faga
    public List<Transform> fireDirections2 = new List<Transform>(); // Lista de direcciones para la segunda r�faga

    public int bulletsPerBurst = 3; // Cantidad de balas por r�faga
    public float burstInterval = 2f; // Intervalo entre r�fagas
    private float timeSinceLastBurst = 0f; // Tiempo desde la �ltima r�faga 

    void Update()
    {
        timeSinceLastBurst += Time.deltaTime * MovimientoJugador.bulletTimeScale;

        if (timeSinceLastBurst >= burstInterval)
        {
            StartCoroutine(FireBurst(fireDirections1)); // Dispara en la primera r�faga
            timeSinceLastBurst = 0f;
        }
    }

    IEnumerator FireBurst(List<Transform> directions)
    {
        // Dispara en cada direcci�n de la lista 'directions'
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            foreach (Transform fireDirection in directions)
            {
                FireBullet(fireDirection);
            }
            yield return new WaitForSeconds(fireInterval);
        }

        // Cambia las direcciones para la siguiente r�faga (ejemplo: usa fireDirections2)
        if (directions == fireDirections1)
        {
            StartCoroutine(FireBurst(fireDirections2)); // Dispara en la segunda r�faga
        }
    }

    void FireBullet(Transform fireDirection)
    {
        // Usar el firePoint para determinar la posici�n de la bala
        if (fireDirection != null)
        {
            Vector3 spawnPosition = fireDirection.position; // Usamos la posici�n del fireDirection
            Quaternion spawnRotation = fireDirection.rotation; // Usamos la rotaci�n del fireDirection

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);
            TestBullet bulletScript = bullet.GetComponent<TestBullet>();
            if (bulletScript != null)
            {
                bulletScript.speed = bulletSpeed;
            }
        }
        else
        {
            Debug.LogWarning("Una de las direcciones no ha sido asignada en el Inspector.");
        }
    }
}