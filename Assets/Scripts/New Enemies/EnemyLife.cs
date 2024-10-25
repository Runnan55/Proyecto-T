using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLife : MonoBehaviour
{
    [Header("Vida del Enemigo")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthBar;
    [SerializeField] private float pushForce = 5f; // Fuerza del empuje
    [SerializeField] private Color damageColor = Color.red; // Color al recibir daño
    [SerializeField] private float colorChangeDuration = 0.5f; // Duración del cambio de color

    private float currentHealth;
    private Renderer enemyRenderer;
    private Color originalColor;
    private Rigidbody rb;

    // Inicialización
    void Start()
    {
        // Configura la vida actual al máximo
        currentHealth = maxHealth;

        // Configura la barra de vida
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Obtiene el componente Renderer para cambiar el color
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;

        // Obtiene el Rigidbody para el empuje
        rb = GetComponent<Rigidbody>();
    }

    // Método para recibir daño
    public void ReceiveDamage(float damage, Vector3 hitDirection)
    {
        currentHealth -= damage;

        // Asegura que la vida no baje de 0
        if (currentHealth < 0)
            currentHealth = 0;

        // Actualiza la barra de vida
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Cambiar el color a rojo
        StartCoroutine(ChangeColor(damageColor));

        // Empujar al enemigo
        if (rb != null)
        {
            rb.AddForce(hitDirection.normalized * pushForce, ForceMode.Impulse);
        }

        // Si la vida llega a 0, destruye al enemigo
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Coroutine para cambiar el color del enemigo
    private IEnumerator ChangeColor(Color newColor)
    {
        enemyRenderer.material.color = newColor; // Cambia al nuevo color
        yield return new WaitForSeconds(colorChangeDuration); // Espera un tiempo
        enemyRenderer.material.color = originalColor; // Restaura el color original
    }

    // Método para manejar la "muerte" del enemigo
    private void Die()
    {
        Debug.Log("El enemigo ha sido destruido.");
        Destroy(gameObject); // Destruye al enemigo en la escena
    }
}
