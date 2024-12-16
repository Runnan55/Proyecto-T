using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedIA : EnemyLife
{
    public enum EnemyState { Searching, Moving, Shooting, Waiting, Stunned }
    public EnemyState currentState;

    [Header("Configuración de Movimiento")]
    public float chaseDistance = 15f;
    public float attackDistance = 10f;
    public float waitTimeBetweenShots = 2f;
    public GameObject projectilePrefab;
    public Transform shootingPoint;

    private NavMeshAgent agent;
    private Transform player;
    public float waitTimer;
    private float originalAgentSpeed;  // Almacena la velocidad original del agente

    [Header("Cubo de Estado")]
    public GameObject statusCube;

    [Header("Empuje")]
    public float pushForce = 10f;
    public float pushDuration = 0.5f;

    private bool isBeingPushed = false;
    private Rigidbody rb;

    [Header("Sounds")]
    [SerializeField] private FMODUnity.EventReference crossbow;

    private MovimientoJugador movimientoJugador;
    [SerializeField] private bool entro = false;
    private float guardarVelocidadTiempo = 0;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        originalAgentSpeed = agent.speed; // Almacena la velocidad original del agente
        rb = GetComponent<Rigidbody>();
        currentState = EnemyState.Searching;

        StartCoroutine(FindPlayerWithDelay());

        UpdateStatusCubeColor();
    }

    void OnEnable()
    {
        // Aquí no necesitamos un evento, solo verificamos el valor de bulletTimeScale
    }

    void OnDisable()
    {
        // Limpiar si fuera necesario
    }

    private IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(1f);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        movimientoJugador = player.GetComponent<MovimientoJugador>();

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'");
        }
    }

    void Update()
    {
        if (player == null || isBeingPushed) return; // No hacer nada si el jugador no está o el enemigo está siendo empujado

        LookAtPlayer();
        actualizarTiempo();
        // Verificar si el Bullet Time ha terminado (bulletTimeScale vuelve a 1)
       

        switch (currentState)
        {
            case EnemyState.Searching:
                SearchForPlayer();
                break;
            case EnemyState.Moving:
                MoveToShootingPosition();
                break;
            case EnemyState.Shooting:
                ShootAtPlayer();
                break;
            case EnemyState.Waiting:
                WaitBeforeNextShot();
                break;
            case EnemyState.Stunned:
                // No hacer nada mientras está aturdido
                break;
        }

        UpdateStatusCubeColor();

        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime * MovimientoJugador.bulletTimeScale;
        }

        // Ajustar la velocidad del agente en función de bulletTimeScale
        agent.speed = originalAgentSpeed * MovimientoJugador.bulletTimeScale;
    }

    public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage);

        if (!isBeingPushed)
        {
            StartCoroutine(PushBack());
        }

        if (currentState == EnemyState.Shooting || currentState == EnemyState.Waiting)
        {
            currentState = EnemyState.Stunned;
            waitTimer = 0;  // Reinicia el temporizador para asegurar que espere antes de otro ataque
        }
    }

    private IEnumerator PushBack()
    {
        if (rb == null) yield break;

        // Cambia el estado a "Stunned" y desactiva el agente para que no se mueva
        isBeingPushed = true;
        agent.enabled = false;
        rb.isKinematic = false;

        currentState = EnemyState.Stunned;

        // Aplica la fuerza de empuje
        Vector3 pushDirection = (transform.position - player.position).normalized;
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);

        yield return new WaitForSeconds(pushDuration * MovimientoJugador.bulletTimeScale);

        rb.isKinematic = true;
        agent.enabled = true;

        isBeingPushed = false;

        // Restaura el estado dependiendo de la distancia al jugador
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            currentState = EnemyState.Shooting;
        }
        else if (Vector3.Distance(transform.position, player.position) <= chaseDistance)
        {
            currentState = EnemyState.Moving;
        }
        else
        {
            currentState = EnemyState.Searching;
        }
    }

    private void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f * MovimientoJugador.bulletTimeScale);
        }
    }

    private void SearchForPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= chaseDistance)
        {
            currentState = EnemyState.Moving;
        }
    }

    private void MoveToShootingPosition()
    {
        if (!agent.enabled) return;

        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 shootingPosition = player.position - direction * attackDistance;

        agent.SetDestination(shootingPosition);

        if (Vector3.Distance(transform.position, shootingPosition) <= 1f)
        {
            currentState = EnemyState.Shooting;
        }
    }

    private void ShootAtPlayer()
    {
        if (waitTimer <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot(crossbow);
            Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
            Debug.Log("El enemigo ha disparado al jugador!");
            waitTimer = waitTimeBetweenShots / MovimientoJugador.bulletTimeScale;
            currentState = EnemyState.Waiting;
        }
    }

    private void WaitBeforeNextShot()
    {
        if (waitTimer <= 0)
        {
            currentState = EnemyState.Moving;
        }
    }

    private void UpdateStatusCubeColor()
    {
        if (statusCube != null)
        {
            Renderer cubeRenderer = statusCube.GetComponent<Renderer>();

            switch (currentState)
            {
                case EnemyState.Searching:
                    cubeRenderer.material.color = Color.green;
                    break;
                case EnemyState.Moving:
                    cubeRenderer.material.color = Color.yellow;
                    break;
                case EnemyState.Shooting:
                    cubeRenderer.material.color = Color.red;
                    break;
                case EnemyState.Waiting:
                    cubeRenderer.material.color = Color.magenta;
                    break;
                case EnemyState.Stunned:
                    cubeRenderer.material.color = Color.blue;
                    break;
            }
        }
        else
        {
            Debug.LogError("El cubo de estado no está asignado en el inspector.");
        }
    }

    private void actualizarTiempo()
    {
        if (movimientoJugador.IsBulletTimeActive() == true)
        {
            Debug.Log("intento division");
            if (entro == false)
            {
                waitTimer = waitTimer / (MovimientoJugador.bulletTimeScale);
                guardarVelocidadTiempo = MovimientoJugador.bulletTimeScale;
                Debug.Log("dividido");
                entro = true;
            }
        }
        if (movimientoJugador.IsBulletTimeActive() == false)
        {
            Debug.Log("intento multi");

            if (entro == true)
            {

                waitTimer = waitTimer * guardarVelocidadTiempo;
                Debug.Log("multiplica");

                entro = false;
            }
        }
    }
}
