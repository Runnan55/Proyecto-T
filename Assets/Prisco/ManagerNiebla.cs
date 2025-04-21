using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerNiebla : MonoBehaviour
{
    public float damageAmount = 1f;
    public bool isActive = true;

    private Life playerLife;
    private float damageTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLife = player.GetComponent<Life>();
            if (playerLife != null)
            {
                Debug.Log("Referencia a Life obtenida correctamente.");
            }
            else
            {
                Debug.LogWarning("No se encontró el componente Life en el jugador.");
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con la etiqueta 'Player' en la escena.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerLife = other.GetComponent<Life>();
            if (playerLife != null)
            {
                Debug.Log("Jugador detectado dentro del área de niebla.");
            }
            else
            {
                Debug.LogWarning("El jugador no tiene el componente Life.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerLife = null;
            Debug.Log("Jugador salió del área de niebla.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive && playerLife != null)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f) // Aplica daño cada segundo
            {
                playerLife.ModifyTime(-damageAmount);
                Debug.Log($"Daño aplicado: {damageAmount} al jugador. Radio de desactivación: {damageTimer}"); // Mensaje de debug
                damageTimer = 0f;
            }
        }
    }
}
