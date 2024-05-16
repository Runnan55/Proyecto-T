using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTpA : Enemy
{
    public EnemyVision enemyVision;
    private NavMeshAgent agent;
    private Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float teleportRadius = 5f;  // Radio del círculo de teletransporte
    public float waitTime = 2f;        // Tiempo de espera antes de volver a teletransportarse

    private float nextFireTime;
    private bool isTeleporting;
    private Animator animator;

    void Awake()
    {
        nextFireTime = Time.time;
        isTeleporting = false;
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null)  // Si no hay jugador, no hacer nada
            return;

        if (!isTeleporting && enemyVision.PlayerEnRango() && enemyVision.DetectarAngulo() && !enemyVision.HayObstaculoEntre())
        {
            if (Time.time >= nextFireTime)
            {
                StartCoroutine(TeleportAndShoot());
            }
        }
    }

    IEnumerator TeleportAndShoot()
    {
        isTeleporting = true;

        // Teletransportarse alrededor del jugador
        Vector3 teleportPosition = GetValidTeleportPosition();
        transform.position = teleportPosition;

        // Apuntar hacia el jugador
        transform.LookAt(player);
        animator.SetBool("Attack", true);
        yield return new WaitForSeconds(1.5f);
        // Disparar
       // Shoot();

        // Esperar antes de permitir otro teletransporte
        yield return new WaitForSeconds(waitTime);

        isTeleporting = false;
        nextFireTime = Time.time + 1f / fireRate;
    }

    public override void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);        
    }


    void Shoot()
    {
        // Activar la animación de ataque

        // Instanciar el proyectil en el punto de fuego
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Restablecer la animación de ataque después de un breve retardo
        StartCoroutine(ResetAttackAnimation());
    }

    IEnumerator ResetAttackAnimation()
    {
        // Esperar el tiempo que dura la animación de ataque antes de restablecer
        yield return new WaitForSeconds(3f);  // Suponiendo que la animación dura 0.5 segundos
        animator.SetBool("Attack", false);
    }

    Vector3 GetValidTeleportPosition()
    {
        Vector3 randomDirection = Random.insideUnitCircle.normalized * teleportRadius;
        Vector3 randomPosition = new Vector3(randomDirection.x, 0f, randomDirection.y);
        Vector3 teleportPosition = player.position + randomPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(teleportPosition, out hit, teleportRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            // Si no se encuentra una posición válida, simplemente retorna la posición del jugador
            return player.position;
        }
    }

    public void ActiveNavMesh()
    {
        enabled = true;
        animator.applyRootMotion = true;

    }

    public void DesactiveNavMesh()
    {
        enabled = false;
        animator.applyRootMotion = false;

    }
}
