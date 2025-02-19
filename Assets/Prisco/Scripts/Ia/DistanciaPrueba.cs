using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DistanciaPrueba : EnemyLife
{
  public enum State { Chasing, Reposition, Shooting, IsOnRange, GetHit }

    public State currentState;
    private State previousState; 
  
    [Header("Components")]
    private NavMeshAgent agent;
    private Transform player;    
    private Renderer enemyRenderer; 
     private Rigidbody rb; 
    private float originalAgentSpeed;

    public float distanciaDetecion = 30f;
     public float ReposicionDetecion = 10f;

      public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float waitTimer;
      [SerializeField] private FMODUnity.EventReference crossbow;
        public float waitTimeBetweenShots = 2f;
    public float repositionDelay = 1f; 
    private float repositionTimer; 

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();       
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRenderer = GetComponent<Renderer>();
        currentState = State.Chasing;
        previousState = currentState; 
        originalAgentSpeed = agent.speed; // Almacena la velocidad original para editarla mejor 
    }
    void Update()
  {
      if (currentState != previousState)
        {
            Debug.Log("Entering " + currentState + " state");
            previousState = currentState;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > distanciaDetecion)
        {
            currentState = State.Chasing;
        }

        switch (currentState)
        {  
        case State.Chasing:       
            HandleChasing();
            break;
        case State.Reposition:
            repositionTimer -= Time.deltaTime;
            if (repositionTimer <= 0)
            {
                HandleReposition();
            }
            break;
        case State.Shooting:
            HandleShooting();
            break;
        case State.IsOnRange:
            HandleIsOnRange();
            break;
        case State.GetHit:
            HandleGetHit();
            break; 
                             
        }

        LookAtPlayer();
        agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale;
  }

  

  private void HandleChasing() 
  { 
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    if (distanceToPlayer > distanciaDetecion)
    {
        agent.SetDestination(player.position);
    }
    else
    {
        agent.ResetPath();
        currentState = State.Shooting; 
    }
    LookAtPlayer(); 
  }
  void HandleReposition() 
  {  
    Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
    Vector3 newPosition = transform.position + directionAwayFromPlayer * 10f; 
    NavMeshHit hit;
    NavMesh.SamplePosition(newPosition, out hit, 10f, 1);
    Vector3 finalPosition = hit.position;

    transform.position = finalPosition; 
    currentState = State.Shooting; 
  }
  void HandleShooting() 
  {        
    LookAtPlayer();
    waitTimer -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
    if (waitTimer <= 0)
    {
        FMODUnity.RuntimeManager.PlayOneShot(crossbow);
        Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
       
        waitTimer = waitTimeBetweenShots / MovimientoJugador.bulletTimeScale;
    }
    currentState = State.IsOnRange;
  }
  void HandleIsOnRange() 
  {  
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    waitTimer -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
    if (distanceToPlayer <= ReposicionDetecion)
    {
        repositionTimer = repositionDelay; // Set the reposition timer
    }
    else if (waitTimer <= 0)
    {
        currentState = State.Shooting; 
    }
  }
  void HandleGetHit() 
  { 

  }

  private void LookAtPlayer()
  {
    Vector3 direction = (player.position - transform.position).normalized;
    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20f);
  }
}
