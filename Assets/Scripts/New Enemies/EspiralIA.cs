using System.Collections;
using UnityEngine;

public class EspiralIA : EnemyLife
{
    public enum EnemyState { Waiting, Shooting, Teleporting, Stunned }
    public EnemyState currentState;

    [Header("Configuración de Ataque")]
    public int projectilesToShoot = 6; // Número fijo de proyectiles
    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float shootingInterval = 0.2f;
    public float attackCooldownTime = 2f; // Tiempo que el enemigo no podrá atacar después de recibir daño

    [Header("Configuración de Teletransporte")]
    public float teleportRadius = 10f;
    public float heightOffset = 1f;
    public GameObject teleportMarkerPrefab;

    [Header("Empuje")]
    public float pushForce = 10f;
    public float pushDuration = 0.5f;

    private Transform player;
    private bool isBeingPushed = false;
    private Rigidbody rb;

    [Header("Cubo de Estado")]
    public GameObject statusCube;

    private bool isTeleporting = false;
    private bool canAttack = true; // Booleano que controla si el enemigo puede atacar

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
        if (player == null) Debug.LogError("No se encontró un objeto con la etiqueta 'Player'");
    }

    private void Update()
    {
        if (player == null || isBeingPushed) return;

        LookAtPlayer();

        switch (currentState)
        {
            case EnemyState.Waiting:
                if (canAttack) // Solo se inicia el ataque si puede atacar
                {
                    StartCoroutine(WaitAndShoot());
                }
                break;
            case EnemyState.Shooting:
                // Disparo ocurre en la corrutina.
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

        if (!isBeingPushed && canAttack)
        {
            // Si puede atacar, desactivar el ataque y comenzar el tiempo de recarga
            StartCoroutine(HandleAttackCooldown());
            StartCoroutine(PushBack());
        }
    }

    private IEnumerator HandleAttackCooldown()
    {
        canAttack = false; // Bloquear el ataque
        yield return new WaitForSeconds(attackCooldownTime); // Esperar el tiempo de recarga
        canAttack = true; // Permitir nuevamente el ataque
    }

    private IEnumerator PushBack()
    {
        if (rb == null || player == null) yield break;

        isBeingPushed = true;
        currentState = EnemyState.Stunned;
        rb.isKinematic = false;

        Vector3 pushDirection = (transform.position - player.position).normalized;
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);

        yield return new WaitForSeconds(pushDuration);

        rb.isKinematic = true;
        isBeingPushed = false;
        currentState = EnemyState.Waiting;
    }

    private IEnumerator WaitAndShoot()
    {
        currentState = EnemyState.Shooting;
        yield return new WaitForSeconds(2f); // Tiempo de espera antes de disparar

        if (projectilePrefab == null)
        {
            Debug.LogError("ProjectilePrefab no está asignado o ha sido destruido.");
            yield break; // Salir si no hay prefab
        }

        // Disparar el número fijo de proyectiles
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

        currentState = EnemyState.Teleporting;
    }

    private void TeleportAroundPlayer()
    {
        if (player == null || isTeleporting) return;

        isTeleporting = true;
        Vector3 teleportPosition = GenerateTeleportPosition();

        if (teleportMarkerPrefab != null)
        {
            GameObject marker = Instantiate(teleportMarkerPrefab, teleportPosition, Quaternion.identity);
            Destroy(marker, 1f); // Eliminar el marcador después de un segundo
        }

        StartCoroutine(TeleportAfterDelay(teleportPosition));
    }

    private Vector3 GenerateTeleportPosition()
    {
        float minDistance = 10f;
        float maxDistance = teleportRadius;

        Vector3 teleportPosition;
        do
        {
            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection.y = 0; // Mantener el teletransporte en el plano horizontal
            float randomDistance = Random.Range(minDistance, maxDistance);
            teleportPosition = player.position + randomDirection * randomDistance;
        } while (Vector3.Distance(player.position, teleportPosition) < minDistance);

        return teleportPosition;
    }

    private IEnumerator TeleportAfterDelay(Vector3 teleportPosition)
    {
        yield return new WaitForSeconds(1.8f); // Esperar antes de teletransportarse

        transform.position = teleportPosition;
        Debug.Log("Teletransportado a: " + teleportPosition);

        currentState = EnemyState.Waiting;
        isTeleporting = false;
    }

    private void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Mantener la rotación en el plano horizontal
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void UpdateStatusCubeColor()
    {
        if (statusCube != null)
        {
            Renderer cubeRenderer = statusCube.GetComponent<Renderer>();

            Color stateColor = currentState switch
            {
                EnemyState.Waiting => Color.green,
                EnemyState.Shooting => Color.red,
                EnemyState.Teleporting => Color.blue,
                EnemyState.Stunned => Color.yellow,
                _ => Color.white
            };

            cubeRenderer.material.color = stateColor;
        }
        else
        {
            Debug.LogError("El cubo de estado no está asignado en el inspector.");
        }
    }
}
