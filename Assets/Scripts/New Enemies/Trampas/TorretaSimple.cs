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

    [Range(1, 10)]
    public int bulletsPerBurst = 1; // Número de balas por ráfaga

    public List<Transform> firePoints; // Lista de Transforms desde los cuales se disparan las balas

    private float timeSinceLastShot = 0f; // Tiempo desde el último disparo

    [Header("Movement Settings")]
    public bool moverse = false; // Variable para activar el movimiento
    public Transform puntoA; // Punto A
    public Transform puntoB; // Punto B
    public float velocidadMovimiento = 2f; // Velocidad de movimiento

    private bool moviendoHaciaB = true; // Indica si se está moviendo hacia el punto B

    [Header("Aiming Settings")]
    public bool apuntarAlJugador = false; // Variable para activar el apuntado al jugador
    public float velocidadRotacion = 2f; // Velocidad de rotación
    private Transform jugador; // Referencia al jugador

    [Header("Activation Settings")]
    public float activationDelay = 2f; // Tiempo de delay antes de activar la torreta
    private bool isActivated = false; // Indica si la torreta está activada

    void Start()
    {
        StartCoroutine(ActivateAfterDelay());
    }

    void Update()
    {
        if (!isActivated)
        {
            return; // No hacer nada si la torreta no está activada
        }

        // Ajustar el tiempo acumulado usando la escala de tiempo del bullet time
        timeSinceLastShot += Time.deltaTime * MovimientoJugador.bulletTimeScale;

        if (timeSinceLastShot >= fireInterval)
        {
            StartCoroutine(FireBurst());
            timeSinceLastShot = 0f;
        }

        // Mover la torreta si la variable moverse está activada
        if (moverse)
        {
            MoverTorreta();
        }

        // Apuntar al jugador si la variable apuntarAlJugador está activada
        if (apuntarAlJugador && jugador != null)
        {
            ApuntarAlJugador();
        }
    }

    IEnumerator FireBurst()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            FireBullet();
            yield return new WaitForSeconds(0.1f); // Pequeño retraso entre balas en la ráfaga
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
        if (puntoA == null || puntoB == null)
        {
            return;
        }

        Transform objetivo = moviendoHaciaB ? puntoB : puntoA;
        transform.position = Vector3.MoveTowards(transform.position, objetivo.position, velocidadMovimiento * Time.deltaTime);

        // Verificar si la torreta ha llegado al objetivo
        if (Vector3.Distance(transform.position, objetivo.position) < 0.1f)
        {
            moviendoHaciaB = !moviendoHaciaB;
        }
    }

    public void BuscarJugador()
    {
        StartCoroutine(BuscarJugadorCoroutine());
    }

    IEnumerator BuscarJugadorCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject jugadorObj = GameObject.FindWithTag("Player");
        if (jugadorObj != null)
        {
            jugador = jugadorObj.transform;
        }
    }

    void ApuntarAlJugador()
    {
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0; // Ignorar la diferencia en el eje Y
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
    }

    IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        isActivated = true;
        BuscarJugador();
    }
}