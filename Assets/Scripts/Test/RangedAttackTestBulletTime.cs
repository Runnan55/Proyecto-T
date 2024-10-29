using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTestBulletTime : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab de la bala

    [Range(0.1f, 100f)]
    public float bulletSpeed = 10f; // Velocidad de la bala

    [Range(0.1f, 3f)]
    public float fireInterval = 1f; // Intervalo de disparo en segundos

    public float bulletOffset = 1f; // Distancia de la bala desde el objeto

    private float timeSinceLastFire = 0f; // Tiempo desde el último disparo

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Ajustar el tiempo acumulado usando la escala de tiempo del bullet time
        timeSinceLastFire += Time.deltaTime * MovimientoJugador.bulletTimeScale;

        // Disparar cuando se haya alcanzado el intervalo ajustado
        if (timeSinceLastFire >= fireInterval)
        {
            FireBullet();
            timeSinceLastFire = 0f; // Reiniciar el tiempo desde el último disparo
        }
    }

    void FireBullet()
    {
        // Calcular la posición de generación de la bala
        Vector3 spawnPosition = transform.position + transform.forward * bulletOffset;
        Quaternion spawnRotation = transform.rotation;

        // Instanciar la bala
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);
        TestBullet bulletScript = bullet.GetComponent<TestBullet>();
        if (bulletScript != null)
        {
            bulletScript.speed = bulletSpeed; // Ajustar la velocidad de la bala
        }
    }
}