using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArqueroAnim : Enemy
{
    public float detectionRange = 10f;
    public float attackRange = 10f;
    public float optimalDistance = 5f;
    public float moveBackDistance = 3f;
    public int shotsBeforeMove = 3;
    public float attackCooldown = 2f;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;

    private NavMeshAgent agent;
    private GameObject player;
    public Transform playerT;
    private float attackTimer;
    private int shotsFired = 0;
    private Vector3 directionToPlayer;
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        directionToPlayer.y = 0; // Mantener el nivel de la mirada horizontal

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    LookAtPlayer();

                    if (distanceToPlayer <= attackRange && !isAttacking)
                    {
                        if (!animator.GetBool("Attack"))
                        {
                            StartCoroutine(PerformAttackSequence());
                        }
                    }
                    else if (distanceToPlayer > attackRange)
                    {
                        MoveToPlayer();
                    }
                }
            }
        }
        else
        {
            StopMovement();
        }
    }

    IEnumerator PerformAttackSequence()
    {
        isAttacking = true;
        animator.SetBool("Attack", true);
        animator.SetBool("Walk", false);

        while (shotsFired < shotsBeforeMove)
        {
            AttackPlayer();
            yield return new WaitForSeconds(attackCooldown);
            shotsFired++;
        }

        shotsFired = 0;
        MoveAwayFromPlayer();
        isAttacking = false;
        animator.SetBool("Attack", false);
    }

    void MoveToPlayer()
    {
        agent.SetDestination(player.transform.position - directionToPlayer.normalized * optimalDistance);
        animator.SetBool("Walk", true);
    }

    void MoveAwayFromPlayer()
    {
        Vector3 moveBackPosition = transform.position - transform.forward * moveBackDistance;
        agent.SetDestination(moveBackPosition);
        animator.SetBool("Walk", true);
    }

    void StopMovement()
    {
        agent.SetDestination(transform.position);
        animator.SetBool("Walk", false);
        animator.SetBool("Attack", false);
    }

    public override void AttackPlayer()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(directionToPlayer));
    }

    void LookAtPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
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