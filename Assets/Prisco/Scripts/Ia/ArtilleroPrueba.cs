using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArtilleroPrueba : EnemyLife
{
    public enum State { positioning, attacking, gethit }
    public State currentState;
    private static List<ArtilleroPrueba> allEnemies = new List<ArtilleroPrueba>();
    private State previousState; // Variable to store the previous state

    private bool isPositioning = false;

    public float agentSpeed = 3.5f; // Velocidad del agente
    public GameObject mortarProjectilePrefab; // Prefab del proyectil de mortero
    public Transform mortarLaunchPoint; // Punto de lanzamiento del proyectil
    private Vector3 targetPosition; // Almacenar la posición objetivo para los Gizmos

    public GameObject impactAreaPrefab; // Prefab del área de impacto    
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Renderer enemyRenderer;
      public float pushForce = 10f; // Fuerza del empuje
    public float pushDuration = 0.5f; // Duración del empuje
    private bool isBeingPushed = false; // Estado para controlar si está en empuje
    public float pushCooldown = 0.5f; // Cooldown para evitar empujes continuos
    private float lastPushTime = -1f; // Último tiempo de empuje
    private Transform player; 

      
    

         private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Intenta obtener el Rigidbody

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
        // Remover este enemigo de la lista cuando se desactive
        if (allEnemies.Contains(this))
        {
            allEnemies.Remove(this);
        }
    }

    public void Update()
    {
        if (currentState == State.gethit)
        {
            return; // No ejecutar otros estados si está en el estado gethit
        }

        switch (currentState)
        {
            case State.positioning:
                Positioning();
                break;
            case State.attacking:
                Attacking();
                break;
        }

        agent.speed = agentSpeed * MovimientoJugador.bulletTimeScale;
    }

       #region Damage Handling
        public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage); // Llama al método base para reducir la vida y gestionar el cambio de material       

          // Verifica si puede aplicar el empuje
        if (Time.time > lastPushTime + pushCooldown)
        {
            lastPushTime = Time.time;
            StartCoroutine(PushBack());
          
        }
          currentState = State.gethit;

    }
    private IEnumerator PushBack()
    {
        if (rb == null) yield break; // Salir si no hay Rigidbody

        isBeingPushed = true;
        agent.enabled = false; // Desactiva el NavMeshAgent
        rb.isKinematic = false; // Cambia el Rigidbody a modo no-kinemático para aplicar la física
       enemyRenderer.material.color = Color.red; 

        // Aplica una fuerza de empuje en la dirección opuesta a la posición del jugador
        Vector3 pushDirection = (transform.position - player.position).normalized;

        if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            pushForce = 15f;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
        else if (MovimientoJugador.instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            pushForce = 20f;
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
                rb.velocity = Vector3.zero; // Detener el empuje
                break;
            }

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Restaurar el estado del Rigidbody y NavMeshAgent
        rb.isKinematic = true; // Cambia de nuevo a kinemático
        agent.enabled = true; // Reactiva el NavMeshAgent
        isBeingPushed = false;
        currentState = State.positioning;
        isPositioning = false; // Asegurarse de que se reinicie el posicionamiento
    }  
    #endregion

    public void Positioning()
    {
          if (!agent.enabled)
        {
            agent.enabled = true; // Asegúrate de que el NavMeshAgent esté habilitado
        }
        
        if (!isPositioning)
        {
            isPositioning = true;
            StartCoroutine(MoveToRandomPosition());
        }
    }

    private IEnumerator MoveToRandomPosition()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        float DistanciaAlejarse = 10f; // Distancia segura del jugador

        for (int i = 0; i < 3; i++)
        {
            if (currentState == State.gethit)
            {
                yield break; // Salir del bucle si el estado cambia a gethit
            }

            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;

            Vector3 targetPosition = transform.position + randomDirection.normalized * 5f;

            // Si el jugador está cerca, alejarse de él
            if (Vector3.Distance(transform.position, player.position) < DistanciaAlejarse)
            {
                Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
                targetPosition = transform.position + directionAwayFromPlayer * 5f;
            }

            if (agent != null && agent.isOnNavMesh)
            {
                float elapsedTime = 0f;
                float lerpDuration = 0.3f;
                Vector3 startPosition = transform.position;

                while (elapsedTime < lerpDuration)
                {
                    if (agent != null && agent.isOnNavMesh)
                    {
                        agent.SetDestination(Vector3.Lerp(startPosition, targetPosition, elapsedTime / lerpDuration));
                    }
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                if (agent != null && agent.isOnNavMesh)
                {
                    agent.SetDestination(targetPosition);
                    while (agent.pathPending || (agent != null && agent.isOnNavMesh && agent.remainingDistance > agent.stoppingDistance))
                    {
                        if (currentState == State.gethit)
                        {
                            yield break; // Salir del bucle si el estado cambia a gethit
                        }
                        yield return null;
                    }
                }
            }
        }
        currentState = State.attacking; // Asegúrate de que el estado se cambie a attacking
        isPositioning = false;
    }

    public void Attacking()
    {
        LaunchMortarProjectile();
        currentState = State.positioning; 
    }

    private void LaunchMortarProjectile()
    {
        if (mortarProjectilePrefab == null || mortarLaunchPoint == null) return;

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        targetPosition = player.position; 

        GameObject projectile = Instantiate(mortarProjectilePrefab, mortarLaunchPoint.position, mortarLaunchPoint.rotation);
        ProyectilMortero proyectilMortero = projectile.GetComponent<ProyectilMortero>();
        if (proyectilMortero != null)
        {
            proyectilMortero.impactAreaPrefab = impactAreaPrefab;
            proyectilMortero.Launch(targetPosition, 1f);
        }
    }
   
    public void GetHit()
    {
        
    }
    

}
