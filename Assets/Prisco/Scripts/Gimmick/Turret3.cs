using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret3 : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform cannon;
    public float moveSpeed = 5f;
    public float fireRate = 1f;
    public float bulletSpeed = 10f;

    private Transform player; // Referencia al transform del jugador
    private bool canShoot = true;

    void Start()
    {
        // Buscar el objeto del jugador por su etiqueta
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'.");
        }

        transform.rotation = Quaternion.Euler(0, -90, 0);
        StartCoroutine(FireRateCooldown());
    }

    void Update()
    {
        if (player != null)
        {
            FollowPlayerOnZ();

            if (canShoot)
            {
                ShootStraight();
                StartCoroutine(FireRateCooldown());
            }
        }
    }

    private void FollowPlayerOnZ()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime * MovimientoJugador.bulletTimeScale);
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