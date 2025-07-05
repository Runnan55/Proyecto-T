using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLIfe : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public EnemyVision enemyVision; // Referencia al script de visi�n

    public float frontDamageMultiplier = 0.25f;
    public float normalDamageMultiplier = 1f;
    
    public int contador=0;
  
    public int contador2 = 0;


    public Slider healthBar;            // Barra principal (vida actual)
    public Slider previewHealthBar;     // Barra secundaria (preview amarilla)
    public float previewSpeed = 1f;     // Velocidad con la que la barra amarilla baja

    public Text damageText; // Referencia al Text en el Canvas para mostrar el da�o
    public float damageDisplayTime = 1f; // Tiempo en segundos que el da�o es visible

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (previewHealthBar != null)
        {
            previewHealthBar.maxValue = maxHealth;
            previewHealthBar.value = currentHealth;
        }
        if (damageText != null)
        {
            damageText.gameObject.SetActive(false); // Aseg�rate de que el texto no se vea al principio
        }
    }
    protected virtual void Update()
    {
        if (previewHealthBar != null && previewHealthBar.value > currentHealth)
        {
            previewHealthBar.value = Mathf.Lerp(previewHealthBar.value, currentHealth, previewSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        if (enemyVision.PlayerEnRango() && enemyVision.DetectarAngulo())
        {
            damage *= frontDamageMultiplier;
            contador++;
            contador2++;
        }
        else
        {
            damage *= normalDamageMultiplier;
            contador++;
            contador2++;
        }

        currentHealth -= damage;
        Debug.Log($"Da�o recibido: {damage}. Vida restante: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
        int redondeado = Mathf.RoundToInt(damage);
        
        if (damageText != null)
        {
            // Mostrar el da�o en el texto
            damageText.text = $"-{redondeado}";
            damageText.gameObject.SetActive(true); // Activar el texto para mostrarlo

            // Desactivar el texto despu�s de un tiempo
            StartCoroutine(HideDamageTextAfterDelay());
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HideDamageTextAfterDelay()
    {
        yield return new WaitForSeconds(damageDisplayTime); // Espera el tiempo configurado
        if (damageText != null)
        {
            damageText.gameObject.SetActive(false); // Desactivar el texto
        }
    }

    private void Die()
    {
        Debug.Log("Enemigo derrotado.");
        BossDeath();
        Destroy(gameObject);
    }

    private void BossDeath()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var movimientoJugador = player.GetComponent<MovimientoJugador>();
            if (movimientoJugador != null)
            {
                movimientoJugador.nivelActual = 3;
                movimientoJugador.disparoDesbloqueado = true;
            }
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("HubNuevo");
    }
}