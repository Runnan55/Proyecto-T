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

        // Comprobaci�n de l�nea de visi�n
        if (!Physics.Raycast(transform.position, directionToPlayer, distance, LayerMask.GetMask("Obstruction")))
        {
            if (Mathf.Abs(distance - shootingRange) < 0.5f)  // Est� en la distancia perfecta
            {
                if (Time.time > lastShootTime + shootCooldown)
                {
                    
                    lastShootTime = Time.time;
                    animator.SetBool("Attack", true);  // Activar animaci�n de ataque
                    animator.SetBool("Walk", false);   // Asegurarse de que no camina mientras ataca
                }
            }
            else
            {
                // Ajustar posici�n
                Vector3 targetPosition = player.position - directionToPlayer * shootingRange;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(targetPosition, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    animator.SetBool("Walk", true);  // Activar animaci�n de caminar
                }
                animator.SetBool("Attack", false);  // Asegurarse de que no ataca mientras camina
            }
        }
        else if (distance > shootingRange)
        {
            // Seguir al jugador si est� fuera de rango
            agent.SetDestination(player.position);
            animator.SetBool("Walk", true);  // Caminar si el jugador est� lejos
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
        // Desactivar animaci�n de caminar si est� atacando
        animator.SetBool("Walk", false);
        // Reactivar animaci�n de ataque si es necesario (opcional dependiendo de c�mo est� configurada la animaci�n)
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