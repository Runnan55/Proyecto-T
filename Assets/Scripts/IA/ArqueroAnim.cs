using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArqueroAnim : Enemy
{
    private Transform player;
    public float shootingRange = 10.0f;
    public float closeDistance = 5f;
    public float shootCooldown = 2.0f;
    public float randomMoveRadius = 5.0f;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public GameObject damageEffect;

    private NavMeshAgent agent;
    private float lastShootTime = -999;
    private Animator animator;
    private bool isAttacking;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Siempre mirar hacia el jugador
        transform.LookAt(player.position);

        // Comprobaci�n de l�nea de visi�n
        if (!Physics.Raycast(transform.position, directionToPlayer, distance, LayerMask.GetMask("Obstruction")))
        {
            if (distance <= closeDistance)
            {
                // Moverse a una posici�n aleatoria si est� muy cerca
                Vector3 randomDirection = Random.insideUnitSphere * randomMoveRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, randomMoveRadius, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    animator.SetBool("Walk", true);
                    animator.SetBool("Attack", false);
                }
            }
            else if (distance <= shootingRange)
            {
                if (Time.time > lastShootTime + shootCooldown)
                {
                    lastShootTime = Time.time;
                    isAttacking = true; // Comienza el ataque
                    animator.SetBool("Attack", true);  // Activar animaci�n de ataque
                    animator.SetBool("Walk", false);   // Asegurarse de que no camina mientras ataca
                }
            }
            else
            {
                // Ajustar posici�n manteniendo la distancia de disparo
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
        isAttacking = true; // Comienza el ataque
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        // Desactivar animaci�n de caminar si est� atacando
        animator.SetBool("Walk", false);
        // Reactivar animaci�n de ataque si es necesario (opcional dependiendo de c�mo est� configurada la animaci�n)
        animator.SetBool("Attack", true);
    }

    public override void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);
        if (isAttacking)
        {
            isAttacking = false; // Cancela el ataque
            animator.SetBool("Attack", false); // Cancela la animaci�n de ataque
        }

        if (damageEffect != null)
        {
            damageEffect.SetActive(true);

            Invoke("DisableDamageEffect", 2f);
        }
    }

    private void DisableDamageEffect()
    {
        if (damageEffect != null)
        {
            damageEffect.SetActive(false);
        }
    }

    public void ActiveNavMesh()
    {
        enabled = true;
        animator.applyRootMotion = true;
    }

    public void DesactiveNavMesh()
    {
        enabled = false;
        animator.applyRootMotion = false;
    }
}