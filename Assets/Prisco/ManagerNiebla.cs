using System.Collections;
using System.Collections.Generic;
using System.Linq; // Para usar LINQ
using UnityEngine;

public class ManagerNiebla : MonoBehaviour
{
    public float damageAmount = 1f;
    public bool isActive = true;

    private Life playerLife;
    private float damageTimer = 0f;
    private ObjDNiebla[] objDNieblaArray; // Array para almacenar referencias a los objetos ObjDNiebla

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FindPlayerWithDelay(0.5f)); // Llama a la corrutina con un retraso de 0.5 segundos

        // Buscar todos los objetos ObjDNiebla en la escena
        objDNieblaArray = FindObjectsOfType<ObjDNiebla>();
    }

    private IEnumerator FindPlayerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera el tiempo especificado

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
        // Verificar si el jugador está dentro del rango de visión de algún ObjDNiebla
        isActive = objDNieblaArray.Any(obj =>
        {
            if (obj.fogRevealerIndex != -1 && obj.fogWar != null)
            {
                Vector3 playerPosition = playerLife != null ? playerLife.transform.position : Vector3.zero;
                Vector3 objPosition = obj.transform.position;
                float distance = Vector3.Distance(playerPosition, objPosition);
                return distance <= obj.sightRange * obj.fogWar._UnitScale;
            }
            return false;
        });

        // Si no está activo, aplicar daño al jugador
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