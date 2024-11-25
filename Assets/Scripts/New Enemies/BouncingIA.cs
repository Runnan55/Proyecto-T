using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BouncingIA : EnemyLife
{
    [Header("Configuración de Ataque")]
    public float chargeTime = 2f; // Tiempo de carga antes de moverse
    public float moveDuration = 5f; // Duración del movimiento antes del ataque
    public float attackRadius = 5f; // Radio del ataque en área
    public float attackDamage = 20f; // Daño del ataque en área
    public LayerMask playerLayer; // Capa para detectar jugadores

    [Header("Movimiento")]
    public float bounceSpeed = 20f; // Velocidad del enemigo

    private Rigidbody rb;
    private bool isCharging = false;
    private bool isMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Evitar rotaciones no deseadas
        StartCoroutine(AttackCycle());
    }

    private IEnumerator AttackCycle()
    {
        while (true)
        {
            // Fase de carga
            isCharging = true;
            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(chargeTime);

            // Fase de movimiento
            isCharging = false;
            isMoving = true;
            yield return StartCoroutine(MoveAndBounce());

            // Fase de ataque
            isMoving = false;
            yield return StartCoroutine(PerformAreaAttack());
        }
    }

    private IEnumerator MoveAndBounce()
    {
        // Dirección inicial aleatoria
        Vector3 direction = GetRandomDirection();
        rb.velocity = direction * bounceSpeed;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero;
    }

    private IEnumerator PerformAreaAttack()
    {
        // Simulación de ataque en área
        yield return new WaitForSeconds(1f);

        // Detecta jugadores en el radio de ataque
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            if (player.TryGetComponent(out Life playerHealth)) // Asegúrate de tener un script PlayerHealth
            {
                playerHealth.ModifyTime(-attackDamage);
            }
        }

        Debug.Log("Ataque en área realizado!");
    }

    private Vector3 GetRandomDirection()
    {
        return new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isMoving)
        {
            // Calcula la dirección de rebote inicial
            Vector3 incomingDirection = rb.velocity.normalized;
            Vector3 normal = collision.contacts[0].normal;
            Vector3 bounceDirection = Vector3.Reflect(incomingDirection, normal);

            // Introducimos una pequeña variabilidad en el ángulo de rebote
            float angleVariation = Random.Range(-15f, 15f);  // Variabilidad entre -15 y 15 grados
            Quaternion rotation = Quaternion.AngleAxis(angleVariation, Vector3.up); // Rota alrededor del eje Y (eje vertical)

            // Aplica la variabilidad a la dirección de rebote
            bounceDirection = rotation * bounceDirection;

            // Ajusta la velocidad para mantener el rebote constante
            rb.velocity = bounceDirection.normalized * bounceSpeed;

            Debug.Log($"Rebote con variación! Dirección: {bounceDirection}");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualiza el radio de ataque en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public override void ReceiveDamage(float damage)
    {
        // Evitar recibir daño durante la carga
        if (isCharging) return;

        base.ReceiveDamage(damage);
    }
}