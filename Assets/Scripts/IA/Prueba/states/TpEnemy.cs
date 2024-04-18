using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TpEnemy : Enemy
{
    public EnemyVision enemyVision;
    private NavMeshAgent agent;
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float teleportRadius = 5f; // Radio del círculo de teletransporte
    public float waitTime = 2f; // Tiempo de espera antes de volver a teletransportarse

    private float nextFireTime;
    private bool isTeleporting;

    void Start()
    {
        nextFireTime = Time.time;
        isTeleporting = false;
    }

    void Update()
    {
        if (player == null) // Si no hay jugador, no hacer nada
            return;

        if (!isTeleporting && enemyVision.PlayerEnRango() && enemyVision.DetectarAngulo() && !enemyVision.HayObstaculoEntre())
        {
            // Si el jugador está dentro del rango de visión y no estamos teletransportando, dispara
            if (Time.time >= nextFireTime)
            {
                StartCoroutine(TeleportAndShoot());
            }
        }
        else 
        {
            // Si el jugador no está dentro del rango de visión o estamos teletransportando, utiliza NavMeshAgent
            agent.SetDestination(player.position);
        }
    }

    IEnumerator TeleportAndShoot()
    {
        isTeleporting = true;

        // Teletransportarse alrededor del jugador
        Vector3 teleportPosition = GetValidTeleportPosition();
        transform.position = teleportPosition;

        // Apuntar hacia el jugador
        transform.LookAt(player);

        // Disparar
        Shoot();

        // Esperar antes de permitir otro teletransporte
        yield return new WaitForSeconds(waitTime);

        isTeleporting = false;
        nextFireTime = Time.time + 1f / fireRate;
    }

    void Shoot()
    {
        // Instantiar el proyectil en el punto de fuego
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    Vector3 GetValidTeleportPosition()
    {
        Vector3 randomDirection = Random.insideUnitCircle.normalized * teleportRadius;
        Vector3 randomPosition = new Vector3(randomDirection.x, 0f, randomDirection.y);
        Vector3 teleportPosition = player.position + randomPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(teleportPosition, out hit, teleportRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            // Si no se encuentra una posición válida, simplemente retorna la posición del jugador
            return player.position;
        }
    }

    public void ActiveNavMesh()
    {
        enabled = true;
    }

    public void DesactiveNavMesh()
    {
        enabled = false;
    }
}