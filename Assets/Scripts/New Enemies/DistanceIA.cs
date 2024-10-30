using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedIA : EnemyLife
{
    public enum EnemyState { Searching, Moving, Shooting, Waiting }
    public EnemyState currentState;

    [Header("Configuraci�n de Movimiento")]
    public float chaseDistance = 15f;  // Distancia a la que comienza a perseguir
    public float attackDistance = 10f;  // Distancia a la que puede atacar
    public float waitTimeBetweenShots = 2f; // Tiempo entre disparos
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform shootingPoint; // Punto desde el cual se disparar� el proyectil

    private NavMeshAgent agent;        // Referencia al agente de NavMesh
    private Transform player;          // Referencia al transform del jugador
    private float waitTimer;           // Temporizador para gestionar el tiempo de espera entre disparos

    [Header("Cubo de Estado")]
    public GameObject statusCube;      // Referencia al cubo que cambiar� de color

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Searching;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontr� un objeto con la etiqueta 'Player'");
        }

        UpdateStatusCubeColor(); // Cambia el color del cubo al iniciar
    }

    void Update()
    {
        if (player == null) return; // Si no se encontr� el jugador, no se ejecuta el resto

        LookAtPlayer();

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
        }

        UpdateStatusCubeColor(); // Actualiza el color del cubo en cada frame

        // Gestionar el temporizador de espera
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime; // Resta el tiempo transcurrido
        }
    }

    private void LookAtPlayer()
    {
        if (player != null)
        {
            // Calcular direcci�n hacia el jugador
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Ignorar la altura para rotaci�n
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Rotaci�n suave hacia el jugador
        }
    }


    // M�todo para localizar al jugador
    private void SearchForPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= chaseDistance)
        {
            currentState = EnemyState.Moving;
        }
    }

    // M�todo para moverse a una posici�n de disparo
    private void MoveToShootingPosition()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 shootingPosition = player.position - direction * attackDistance;

        agent.SetDestination(shootingPosition); // Establece la posici�n de disparo como destino

        if (Vector3.Distance(transform.position, shootingPosition) <= 1f) // Umbral para llegar
        {
            currentState = EnemyState.Shooting; // Cambia al estado de disparo
        }
    }

    // M�todo para disparar al jugador
    private void ShootAtPlayer()
    {


        if (waitTimer <= 0)
        {
            GameObject Shoot = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
            Debug.Log("El enemigo ha disparado al jugador!");
            waitTimer = waitTimeBetweenShots; // Reinicia el temporizador de espera
            currentState = EnemyState.Waiting; // Cambia al estado de espera
        }
    }

    // M�todo para esperar antes de disparar nuevamente
    private void WaitBeforeNextShot()
    {
        if (waitTimer <= 0)
        {
            currentState = EnemyState.Moving; // Regresa al estado de movimiento despu�s de esperar
        }
    }

    /// <summary>
    /// Cambia el color del cubo seg�n el estado del enemigo.
    /// </summary>
    private void UpdateStatusCubeColor()
    {
        if (statusCube != null)
        {
            Renderer cubeRenderer = statusCube.GetComponent<Renderer>();

            switch (currentState)
            {
                case EnemyState.Searching:
                    cubeRenderer.material.color = Color.green; // Color verde para el estado de b�squeda
                    break;
                case EnemyState.Moving:
                    cubeRenderer.material.color = Color.yellow; // Color amarillo para el estado de movimiento
                    break;
                case EnemyState.Shooting:
                    cubeRenderer.material.color = Color.red; // Color rojo al disparar
                    break;
                case EnemyState.Waiting:
                    cubeRenderer.material.color = Color.magenta; // Color magenta durante la espera
                    break;
            }
        }
        else
        {
            Debug.LogError("El cubo de estado no est� asignado en el inspector.");
        }
    }
}
