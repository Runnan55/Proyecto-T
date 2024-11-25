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

    // Cambiar OnCollisionEnter por OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        if (isMoving && other.CompareTag("Walls"))
        {
            // Calcula la dirección de rebote reflejando la velocidad actual en el plano XZ
            Vector3 incomingDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z).normalized; 
            Vector3 normal = other.transform.forward; 
            Vector3 bounceDirection = Vector3.Reflect(incomingDirection, normal);

            float angleVariation = Random.Range(-30f, 30f);  
            Quaternion rotation = Quaternion.AngleAxis(angleVariation, Vector3.up); 

            // Aplica la variabilidad a la dirección de rebote
            bounceDirection = rotation * bounceDirection;

            // Aseguramos que la velocidad en Y sea 0 para que el enemigo no se eleve
            bounceDirection.y = 0f;

            // Verificamos si la dirección de rebote es muy pequeña y forzamos un ángulo de rebote mínimo
            if (Mathf.Abs(bounceDirection.x) < 0.1f && Mathf.Abs(bounceDirection.z) < 0.1f)
            {
                bounceDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            }

            // Añadir un pequeño impulso para evitar quedarse pegado a la pared, solo en XZ
            float extraForce = Random.Range(0.5f, 2f);  // Impulso aleatorio para asegurar que no se quede pegado
            bounceDirection += bounceDirection.normalized * extraForce;

            // Ajusta la velocidad para mantener el rebote constante en el plano XZ
            rb.velocity = bounceDirection.normalized * bounceSpeed * Random.Range(1f, 1.5f); // Agregar un factor extra de velocidad

            // Debugging para verificar la dirección y la velocidad del rebote
            Debug.Log($"Rebote con variación! Dirección: {bounceDirection}, Velocidad: {rb.velocity}");
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
