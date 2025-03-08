using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NinjaPruebas : EnemyLife
{
    public enum State { Chasing, Shooting, GetHit, Blink }

    [Header("Components")]
    private NavMeshAgent agent;
    private Transform player;    
    private Renderer enemyRenderer; 
    private Rigidbody rb;    
    private State currentState;
    private State previousState;    
    public float attackRange = 20f;
    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float preShootDelay = 1f;
    private float waitTimer;
    private bool isPreShooting = false;
    public float blinkDistance = 10f;
    private int shotsFired = 0;
    public int shotsBeforeBlink = 2;
    public float shootSpeedMultiplier = 2f; // Multiplicador de velocidad de disparo
    public float pushForce = 10f; // Fuerza del empuje
    public float pushDuration = 0.5f; // Duración del empuje
    private bool isBeingPushed = false; // Estado para controlar si está en empuje     
    private float originalAgentSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();       
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRenderer = GetComponent<Renderer>();
        currentState = State.Chasing;
        previousState = currentState; 
        originalAgentSpeed = agent.speed;    

        
    }

    public void Update()
    {
        switch (currentState)
        {
            case State.Chasing:
                Chasing();
                break;         
            case State.Shooting:
                Shooting();
                break;
            case State.Blink:
                Blink();
                break;
            case State.GetHit:
                GetHit();
                break;
          
        }

      
        agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale;
    }

    private void Chasing()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Shooting;
        }
    }

    private void Shooting()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true; 
        }

        if (!isPreShooting)
        {
            isPreShooting = true;
            waitTimer = preShootDelay;
        }

        waitTimer -= Time.deltaTime * shootSpeedMultiplier * MovimientoJugador.bulletTimeScale; // Aplicar el multiplicador de velocidad y el tiempo bala
        float lerpFactor = 1 - (waitTimer / preShootDelay);
        enemyRenderer.material.color = Color.Lerp(Color.magenta, Color.blue, lerpFactor);

        LookAtPlayer(); 

        if (waitTimer <= 0)
        {
            enemyRenderer.material.color = Color.magenta;
            InstantiateProjectile(shootingPoint.forward);
            InstantiateProjectile(Quaternion.Euler(0, 30, 0) * shootingPoint.forward);
            InstantiateProjectile(Quaternion.Euler(0, -30, 0) * shootingPoint.forward);

            shotsFired++;
            isPreShooting = false;

            if (shotsFired >= shotsBeforeBlink)
            {
                shotsFired = 0;
                currentState = State.Blink; 
            }
            else
            {
                waitTimer = preShootDelay; 
            }
        }
    }

    private void Blink()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 blinkPosition = transform.position + directionAwayFromPlayer * blinkDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(blinkPosition, out hit, blinkDistance, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            Vector3 alternativeBlinkPosition = player.position + directionAwayFromPlayer * 20f;
            if (NavMesh.SamplePosition(alternativeBlinkPosition, out hit, 20f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
        }

        agent.isStopped = false; // Permitir que el enemigo se mueva nuevamente
        currentState = State.Chasing; // Cambiar al estado Chasing después de teletransportarse
    }

    private void InstantiateProjectile(Vector3 direction)
    {
        Instantiate(projectilePrefab, shootingPoint.position, Quaternion.LookRotation(direction));
    }

     private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20f);
    }   

    private void GetHit()
    {
        currentState = State.Blink; 
    }

     #region Damage Handling 

      public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage);       

        
        if (MovimientoJugador.bulletTimeScale < 1)
        {
            StartCoroutine(PushBack());
        }
        else
        {
            currentState = State.GetHit; 
        }
    }

    private IEnumerator PushBack()
    {
        if (rb == null) yield break; 
        isBeingPushed = true;
        if (agent != null)
        {
            agent.enabled = false; 
        }
        rb.isKinematic = false; 
        enemyRenderer.material.color = Color.red; 

       
        Vector3 pushDirection = (transform.position - player.position).normalized;

           if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            pushForce = 10f;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            pushForce = 12.5f;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            pushForce = 15;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }

        float elapsedTime = 0f;
        while (elapsedTime < pushDuration)
        {
            float frameDistance = rb.velocity.magnitude * Time.fixedDeltaTime;

            if (Physics.Raycast(rb.position, pushDirection, out RaycastHit hit, frameDistance, LayerMask.GetMask("obstacleLayers")))
            {
                rb.velocity = Vector3.zero; 
                break;
            }

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        
        rb.isKinematic = true; 
        if (agent != null)
        {
            agent.enabled = true; 
            if (agent.isOnNavMesh)
            {
                agent.Warp(transform.position); 
            }
        }
        isBeingPushed = false;
        currentState = State.Chasing;
    }
    #endregion
}
