using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret1 : MonoBehaviour
{
    public Transform rotationCenter;
    public GameObject bulletPrefab;
    public Transform cannon;
    public float rotationSpeed = 30f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;
    public bool rotateCounterClockwise = false;

    // private bool canShoot = true;
    private float shootCooldown = 0f;

    void Start()
    {
        shootCooldown = 0f;
    }

    void Update()
    {
        if (rotationCenter != null)
        {
            float rotationDirection = rotateCounterClockwise ? -1f : 1f;
            transform.RotateAround(rotationCenter.position, Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime * MovimientoJugador.bulletTimeScale);
            LookAtCenter();
        }

        if (shootCooldown > 0f)
        {
            shootCooldown -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
        }

        if (shootCooldown <= 0f)
        {
            ShootAtCenter();
            shootCooldown = fireRate;
        }
    }

    private void LookAtCenter()
    {
        Vector3 direction = (rotationCenter.position - transform.position).normalized;
        direction.y = 0; // Mantener la rotación solo en el eje Y
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed * MovimientoJugador.bulletTimeScale);
    }

    private void ShootAtCenter()
    {
        Vector3 direction = (rotationCenter.position - cannon.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, cannon.position, Quaternion.LookRotation(direction));
        // Solo asigna la velocidad base, el script de la bala se encarga del time scale
        bullet.GetComponent<FlechaTest>().speed = bulletSpeed;
    }

    private IEnumerator FireRateCooldown()
    {
        yield break;
    }
}