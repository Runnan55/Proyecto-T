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

    private bool canShoot = true;

    void Start()
    {
        StartCoroutine(FireRateCooldown());
    }

    void Update()
    {
        if (rotationCenter != null)
        {
            float rotationDirection = rotateCounterClockwise ? -1f : 1f;
            transform.RotateAround(rotationCenter.position, Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime * MovimientoJugador.bulletTimeScale);
            LookAtCenter();
        }

        if (canShoot)
        {
            ShootAtCenter();
            StartCoroutine(FireRateCooldown());
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
        bullet.GetComponent<Flecha>().speed = bulletSpeed * MovimientoJugador.bulletTimeScale;
    }

    private IEnumerator FireRateCooldown()
    {
        canShoot = false;
        float adjustedFireRate = fireRate / MovimientoJugador.bulletTimeScale;
        float elapsedTime = 0f;

        while (elapsedTime < adjustedFireRate)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canShoot = true;
    }
}