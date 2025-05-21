using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArmController : MonoBehaviour
{
    public GameObject shadowIndicatorPrefab; // Prefab de la sombra en el suelo
    public GameObject shockwaveEffectPrefab; // Prefab visual de la onda expansiva
    public float delayBeforeSmash = 3f; // Tiempo que tarda en caer la mano
    public float shockwaveRadius = 3f; // Radio de daño alrededor del impacto
    public float damage = 50f; // Daño a aplicar

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void TriggerPalmSmash()
    {
        Debug.Log("Palmada iniciada");
        StartCoroutine(DoPalmAttack());
    }

    private IEnumerator DoPalmAttack()
    {
        // Guardamos la posición del jugador en el momento del aviso
        Vector3 targetPosition = player.position;
        Debug.Log("Sombra creada en: " + targetPosition);

        // 1. Aparece la sombra
        GameObject shadow = Instantiate(shadowIndicatorPrefab, targetPosition, Quaternion.identity);

        // 2. Espera antes de la palmada
        yield return new WaitForSeconds(delayBeforeSmash);
        Debug.Log("Palmada ejecutada tras " + delayBeforeSmash + " segundos");

        // 3. Onda expansiva visual
        Instantiate(shockwaveEffectPrefab, targetPosition, Quaternion.identity);
        Debug.Log("Onda expansiva creada");

        // 4. Daño si el jugador está en el radio
        float distance = Vector3.Distance(player.position, targetPosition);
        Debug.Log("Distancia del jugador al centro de impacto: " + distance);

        if (distance <= shockwaveRadius)
        {
            Debug.Log("Jugador dentro del área: aplicar daño (" + damage + ")");
            // Aquí deberías aplicar el daño real al jugador
            // player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Jugador fuera del área: no recibe daño");
        }

        // 5. Eliminar sombra
        Destroy(shadow);
    }
}