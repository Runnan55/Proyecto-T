using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret3 : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public GameObject bulletPrefab;
    public Transform cannon;
    public float moveSpeed = 5f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;

    private bool canShoot = true;
    private bool movingToB = true;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, -90, 0);
        StartCoroutine(FireRateCooldown());
    }

    void Update()
    {
        MoveBetweenPoints();

        if (canShoot)
        {
            ShootStraight();
            StartCoroutine(FireRateCooldown());
        }
    }

    private void MoveBetweenPoints()
    {
        if (movingToB)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, moveSpeed * Time.deltaTime * MovimientoJugador.bulletTimeScale);
            if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
            {
                movingToB = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pointA.position, moveSpeed * Time.deltaTime * MovimientoJugador.bulletTimeScale);
            if (Vector3.Distance(transform.position, pointA.position) < 0.1f)
            {
                movingToB = true;
            }
        }
    }

    private void ShootStraight()
    {
        GameObject bullet = Instantiate(bulletPrefab, cannon.position, cannon.rotation);
        bullet.GetComponent<Flecha>().speed = bulletSpeed * MovimientoJugador.bulletTimeScale;
    }

    private IEnumerator FireRateCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate / MovimientoJugador.bulletTimeScale);
        canShoot = true;
    }
}