using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArtilleroPrueba : EnemyLife
{
    public enum State { positioning, attacking, gethit, reposition }
    public State currentState;
    private static List<ArtilleroPrueba> allEnemies = new List<ArtilleroPrueba>();
    private State previousState; 

    private bool isPositioning = false;
    private bool isRepositioning = false; 

    public float agentSpeed = 3.5f; 
    public GameObject mortarProjectilePrefab; 
    public Transform mortarLaunchPoint; 
    private Vector3 targetPosition; 

    public GameObject impactAreaPrefab;    
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Renderer enemyRenderer;
    public float pushForce = 5f; 
    public float pushDuration = 0.5f; 
    private bool isBeingPushed = false; 
    public float pushCooldown = 0.5f; 
    private float lastPushTime = -1f; 
    private Transform player; 
    private bool isPreShooting = false;
    private float waitTimer;
    public float preShootDelay = 0.5f; // Tiempo de espera antes de disparar
    public float waitTimeBetweenShots = 2f; // Tiempo de espera entre disparos
    public float repositionDelay = 1f; // Tiempo de espera antes de reposicionar
    private float repositionTimer;
    Color color;

    private void Awake()
    {
       
        rb = GetComponent<Rigidbody>(); 

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRenderer = GetComponent<Renderer>(); 
        currentState = State.positioning;
        previousState = currentState; 
       

         if (!allEnemies.Contains(this))
        {
            allEnemies.Add(this);
        }
    }

        void OnDisable()
    {
        
        if (allEnemies.Contains(this))
        {
            allEnemies.Remove(this);
        }
    }

    public void Update()
    {
        if (currentState == State.gethit)
        {
            return; 
        }

        switch (currentState)
        {
            case State.positioning:
                Positioning();
                break;
            case State.attacking:
                Attacking();
                break;
            case State.reposition:
                if (!isRepositioning)
                {
                    StartCoroutine(Reposition());
                }
                break;
        }

        agent.speed = agentSpeed * MovimientoJugador.bulletTimeScale;
    }

       #region Damage Handling
        public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage); 

        
        if (Time.time > lastPushTime + pushCooldown)
        {
            lastPushTime = Time.time;
            StartCoroutine(PushBack());
          
        }
          currentState = State.gethit;

    }
    private IEnumerator PushBack()
    {
        if (rb == null) yield break; 

        isBeingPushed = true;
        agent.enabled = false; 
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
                rb.velocity = Vector3.zero; // Detiene el movimiento si choca con un obstáculo
                break;
            }

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

       
        rb.isKinematic = true;
        agent.enabled = true; 
        isBeingPushed = false;
        currentState = State.positioning;
        isPositioning = false; 
    }  
    #endregion

    public void Positioning()
    {
        agent.isStopped = false;
        if (!agent.enabled)
        {
            agent.enabled = true; 
        }

        if (!isPositioning)
        {
            isPositioning = true;
            StartCoroutine(MoveAwayFromPlayer());
        }
    }

    private IEnumerator MoveAwayFromPlayer()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        float distanciaAlejarse = 10f; 

        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 targetPosition = transform.position + directionAwayFromPlayer * distanciaAlejarse;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(targetPosition);
            while (agent.pathPending || (agent != null && agent.isOnNavMesh && agent.remainingDistance > agent.stoppingDistance))
            {
                if (currentState == State.gethit)
                {
                    yield break; 
                }
                yield return null;
            }
        }

        currentState = State.attacking; 
        isPositioning = false;
    }

    public void Attacking()
    {
        agent.isStopped = true;
        if (mortarProjectilePrefab == null || mortarLaunchPoint == null) return;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        targetPosition = player.position;

        if (!isPreShooting)
        {
            isPreShooting = true;
            waitTimer = preShootDelay;
            color = enemyRenderer.material.color; 
        }

        waitTimer -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
        float lerpFactor = 1 - (waitTimer / preShootDelay);
        Color darkerColor = color * 0.5f; 
        enemyRenderer.material.color = Color.Lerp(color, darkerColor, lerpFactor);

        if (waitTimer <= 0)
        {
            enemyRenderer.material.color = color; 
            GameObject projectile = Instantiate(mortarProjectilePrefab, mortarLaunchPoint.position, mortarLaunchPoint.rotation);
            ProyectilMortero proyectilMortero = projectile.GetComponent<ProyectilMortero>();
            if (proyectilMortero != null)
            {
                proyectilMortero.impactAreaPrefab = impactAreaPrefab;
                proyectilMortero.Launch(targetPosition, 1f);
            }

            waitTimer = waitTimeBetweenShots / MovimientoJugador.bulletTimeScale;
            isPreShooting = false;

           
            currentState = State.reposition;
            repositionTimer = repositionDelay; 
        }
    }

    private IEnumerator Reposition()
    {
        isRepositioning = true; 
        float elapsedTime = 0f;
        float waitTime = 1f;

        while (elapsedTime < waitTime)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= 15f)
            {
                currentState = State.positioning;
                isRepositioning = false; 
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentState = State.attacking;
        isRepositioning = false; 
    }

 
   
    public void GetHit()
    {
        
    }
    

}
