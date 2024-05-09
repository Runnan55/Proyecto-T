using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArqueroAnim : Enemy
{
    public Transform player;
    public float shootingRange = 10.0f;
    public float backwardStep = 5.0f;
    public float shootCooldown = 2.0f;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;

    private NavMeshAgent agent;
    private float lastShootTime = -999;
    private Animator animator;
    

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Siempre mirar hacia el jugador
        transform.LookAt(player.position);

        // Comprobación de línea de visión
        if (!Physics.Raycast(transform.position, directionToPlayer, distance, LayerMask.GetMask("Obstruction")))
        {
            if (Mathf.Abs(distance - shootingRange) < 0.5f)  // Está en la distancia perfecta
            {
                if (Time.time > lastShootTime + shootCooldown)
                {
                    
                    lastShootTime = Time.time;
                    animator.SetBool("Attack", true);  // Activar animación de ataque
                    animator.SetBool("Walk", false);   // Asegurarse de que no camina mientras ataca
                }
            }
            else
            {
                // Ajustar posición
                Vector3 targetPosition = player.position - directionToPlayer * shootingRange;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    animator.SetBool("Walk", true);  // Activar animación de caminar
                }
                animator.SetBool("Attack", false);  // Asegurarse de que no ataca mientras camina
            }
        }
        else if (distance > shootingRange)
        {
            // Seguir al jugador si está fuera de rango
            agent.SetDestination(player.position);
            animator.SetBool("Walk", true);  // Caminar si el jugador está lejos
            animator.SetBool("Attack", false);
        }
        else
        {
            // Detenerse y esperar
            animator.SetBool("Walk", false);
            animator.SetBool("Attack", false);
        }
    }

    void Shoot()
    {
        Debug.Log("Ataque");
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        // Desactivar animación de caminar si está atacando
        animator.SetBool("Walk", false);
        // Reactivar animación de ataque si es necesario (opcional dependiendo de cómo esté configurada la animación)
        animator.SetBool("Attack", true);
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