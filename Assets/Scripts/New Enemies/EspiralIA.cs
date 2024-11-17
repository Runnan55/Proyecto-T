using System.Collections;
using UnityEngine;

public class EspiralIA : EnemyLife
{
    public enum EnemyState { Waiting, Shooting, Teleporting, Stunned }
    public EnemyState currentState;

    [Header("Configuración de Ataque")]
    public float waitTimeBeforeShooting = 2f;
    public int minProjectiles = 6;
    public int maxProjectiles = 8;
    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float shootingInterval = 0.2f;

    [Header("Configuración de Teletransporte")]
    public float teleportRadius = 10f;
    public float heightOffset = 1f;

    [Header("Empuje")]
    public float pushForce = 10f;
    public float pushDuration = 0.5f;

    private Transform player;
    private bool isBeingPushed = false;
    private Rigidbody rb;

    [Header("Cubo de Estado")]
    public GameObject statusCube;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentState = EnemyState.Waiting;

        StartCoroutine(FindPlayerWithDelay());
        UpdateStatusCubeColor();
    }

    private IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'");
        }
    }

    private void Update()
    {
        if (player == null || isBeingPushed) return;

        // Actualizar rotación del enemigo hacia el jugador
        LookAtPlayer();

        switch (currentState)
        {
            case EnemyState.Waiting:
                StartCoroutine(WaitAndShoot());
                break;
            case EnemyState.Shooting:
                // El disparo ocurre en una corrutina separada.
                break;
            case EnemyState.Teleporting:
                TeleportAroundPlayer();
                break;
            case EnemyState.Stunned:
                // No hacer nada mientras está aturdido.
                break;
        }

        UpdateStatusCubeColor();
    }

    public override void ReceiveDamage(float damage)
    {
        base.ReceiveDamage(damage);

        if (!isBeingPushed)
        {
            StartCoroutine(PushBack());
        }
    }

    private IEnumerator PushBack()
    {
        if (rb == null || player == null) yield break;

        // Cambiar estado y deshabilitar movimiento lógico
        isBeingPushed = true;
        currentState = EnemyState.Stunned;
        rb.isKinematic = false;

        // Aplicar fuerza de empuje
        Vector3 pushDirection = (transform.position - player.position).normalized;
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);

        // Esperar la duración del empuje
        yield return new WaitForSeconds(pushDuration);

        // Restaurar control del enemigo
        rb.isKinematic = true;
        isBeingPushed = false;

        // Volver al ciclo normal
        currentState = EnemyState.Waiting;
    }

    private IEnumerator WaitAndShoot()
    {
        currentState = EnemyState.Shooting;

        // Esperar antes de disparar
        yield return new WaitForSeconds(waitTimeBeforeShooting);

        // Verificar que el prefab no sea nulo antes de instanciar
        if (projectilePrefab == null)
        {
            Debug.LogError("ProjectilePrefab no está asignado o ha sido destruido.");
            yield break; // Salir de la corrutina si no hay un prefab válido
        }

        // Disparar proyectiles
        int projectilesToShoot = Random.Range(minProjectiles, maxProjectiles + 1);
        for (int i = 0; i < projectilesToShoot; i++)
        {
            if (shootingPoint != null)
            {
                Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
            }
            else
            {
                Debug.LogError("ShootingPoint no está asignado.");
            }

            yield return new WaitForSeconds(shootingInterval);
        }

        // Cambiar al estado de teletransporte
        currentState = EnemyState.Teleporting;
    }

    private void TeleportAroundPlayer()
    {
        if (player == null) return;

        // Generar una posición aleatoria dentro de un círculo
        Vector2 randomCircle = Random.insideUnitCircle * teleportRadius;
        Vector3 teleportPosition = new Vector3(
            player.position.x + randomCircle.x,
            player.position.y + heightOffset,
            player.position.z + randomCircle.y
        );

        // Mover al enemigo
        transform.position = teleportPosition;

        // Debug para verificar teletransporte
        Debug.Log("Teletransportado a: " + teleportPosition);

        // Cambiar al estado de espera para reiniciar el ciclo
        currentState = EnemyState.Waiting;
    }

    private void LookAtPlayer()
    {
        if (player != null)
        {
            // Calcular la dirección hacia el jugador
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Mantener la rotación únicamente en el plano horizontal

            // Rotar suavemente hacia el jugador
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void UpdateStatusCubeColor()
    {
        if (statusCube != null)
        {
            Renderer cubeRenderer = statusCube.GetComponent<Renderer>();

            switch (currentState)
            {
                case EnemyState.Waiting:
                    cubeRenderer.material.color = Color.green;
                    break;
                case EnemyState.Shooting:
                    cubeRenderer.material.color = Color.red;
                    break;
                case EnemyState.Teleporting:
                    cubeRenderer.material.color = Color.blue;
                    break;
                case EnemyState.Stunned:
                    cubeRenderer.material.color = Color.yellow;
                    break;
            }
        }
        else
        {
            Debug.LogError("El cubo de estado no está asignado en el inspector.");
        }
    }
}