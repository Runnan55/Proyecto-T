using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Charge : Enemy
{
    private Transform player;
    public Collider attackCollider;
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public float waitTime = 2f;
    public float chargeSpeed = 10f;
    public float normalSpeed = 3.5f;
    public float chargeDistance = 5f; 
    public float chargeDuration = 1f; 
    public Collider dano;


    private NavMeshAgent agent;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private Animator animator;
    void Awake()
    {

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        animator = GetComponent<Animator>(); 
        dano.enabled = false;
        
        attackCollider.enabled = false;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        
        if (isWaiting && !IsPlayerBehindEnemy() && !isAttacking)
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
        }
        
        if (distanceToPlayer <= detectionRadius && !isAttacking && !isWaiting)
        {
            if (distanceToPlayer > attackDistance)
            {
                agent.SetDestination(player.position);
                animator.SetBool("Walk", true);
            }
            else
            {
                    
                StartCoroutine(WaitAndAttack());
            }
        }
        else
        {
            animator.SetBool("Walk", false);

        }
        
    }

    private IEnumerator WaitAndAttack()
    {
        isWaiting = true;
        agent.isStopped = true; 
        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
        StartCoroutine(ChargeAttack());
    }

    private IEnumerator ChargeAttack()
    {
        isAttacking = true;
        agent.isStopped = false;
            dano.enabled = true;
        agent.speed = chargeSpeed;
        animator.SetBool("Walk", true);
        Vector3 chargeDirection = (player.position - transform.position).normalized;
        Vector3 chargeTarget = transform.position + chargeDirection * chargeDistance;

        agent.SetDestination(chargeTarget);

        yield return new WaitForSeconds(chargeDuration);

        agent.speed = normalSpeed;
        agent.SetDestination(transform.position);

        transform.LookAt(player.position);
        Debug.Log("Enemy charge completed and looking at player.");
            dano.enabled = false;

            isAttacking = false;
        animator.SetBool("Walk", false);
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    private bool IsPlayerBehindEnemy()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0; 
        directionToPlayer.y = 0; 

        float angleToPlayer = Vector3.Dot(forwardDirection.normalized, directionToPlayer.normalized);

        return angleToPlayer > 0.5f;
    }

  
 
 

    public void DesactivarMovimientos()
    {

        animator.applyRootMotion = false;

        attackCollider.enabled = true;
        agent.enabled = false; // Desactiva el NavMeshAgent
        Debug.Log("11");
    }

    public void ReactivarMovimientos()
    {
        attackCollider.enabled = false;

        agent.enabled = true; // Desactiva el NavMeshAgent
        animator.applyRootMotion = true;

        Debug.Log("22");
    }

}