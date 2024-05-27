using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Charge : Enemy
{
    private Transform player;
    public Collider attackCollider; // Collider de ataque
    public float detectionRadius = 10f;
    public float attackDistance = 2f;
    public float waitTime = 2f;
    public float chargeSpeed = 10f;
    public float normalSpeed = 3.5f;
    public float chargeDistance = 5f; // Distancia que se embestirá
    public float chargeDuration = 1f; // Duración de la embestida
    public Collider dano;
    public GameObject damageEffect;


    private NavMeshAgent agent;
    private bool isAttacking = false;
    private bool isWaiting = false;
    private Animator animator;
    private bool empujar = true;
    void Awake()
    {

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        animator = GetComponent<Animator>(); // Obtener el Animator
        dano.enabled = false;
        // Desactivar el collider de ataque al inicio
        attackCollider.enabled = false;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si el enemigo está en espera, el jugador está detrás del enemigo y el enemigo no está atacando
        if (isWaiting && !IsPlayerBehindEnemy() && !isAttacking)
        {
            // Activa el collider de ataque
            attackCollider.enabled = true;
        }
        else
        {
            // Desactiva el collider de ataque
            attackCollider.enabled = false;
        }
        
        // Si el jugador está dentro del radio de detección y el enemigo no está atacando ni esperando
        if (distanceToPlayer <= detectionRadius && !isAttacking && !isWaiting)
        {
            if (distanceToPlayer > attackDistance)
            {
                // Moverse hacia el jugador
                agent.SetDestination(player.position);
                animator.SetBool("Walk", true);
            }
            else
            {
                    
                // Comienza la espera antes de atacar
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
        agent.isStopped = true; // Detener al enemigo
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

        // Movimiento de embestida
        agent.SetDestination(chargeTarget);

        yield return new WaitForSeconds(chargeDuration);

        // Vuelve a la velocidad normal
        agent.speed = normalSpeed;
        agent.SetDestination(transform.position); // Detener el movimiento

        // Mirar al jugador después de la carga
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

    // Método para verificar si el jugador está detrás del enemigo
    private bool IsPlayerBehindEnemy()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0; // Ignorar la componente Y para evitar problemas en terrenos inclinados
        directionToPlayer.y = 0; // Ignorar la componente Y para evitar problemas en terrenos inclinados

        // Calcula el ángulo entre los dos vectores
        float angleToPlayer = Vector3.Dot(forwardDirection.normalized, directionToPlayer.normalized);

        // Si el ángulo es mayor que 0.5, el jugador está detrás del enemigo
        return angleToPlayer > 0.5f;
    }

    public override void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);

        if (damageEffect != null)
        {
            damageEffect.SetActive(true);

            Invoke("DisableDamageEffect", 2f);
        }

        if (empujar)
        {
            Empuje();
        }
    }
    public void Empuje()
    {
        Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            // Cambia el valor de la fuerza seg�n lo necesites
            float force = 100f;
            enemyRigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
        }
    }
    private void DisableDamageEffect()
    {
        if (damageEffect != null)
        {
            damageEffect.SetActive(false);

        }
    }

    public void DesactivarMovimientos()
    {
        attackCollider.enabled = true;
        agent.enabled = false; // Desactiva el NavMeshAgent
        animator.enabled = false;
        Debug.Log("11");
    }

    public void ReactivarMovimientos()
    {
        attackCollider.enabled = false;

        agent.enabled = true; // Desactiva el NavMeshAgent
        animator.enabled = true;
        Debug.Log("22");
    }

}