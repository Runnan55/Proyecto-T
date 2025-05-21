using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArmController : MonoBehaviour
{
    public GameObject shadowIndicatorPrefab; // Prefab de la sombra en el suelo
    public GameObject shockwaveEffectPrefab; // Prefab visual de la onda expansiva
    public float delayBeforeSmash = 3f; // Tiempo que tarda en caer la mano
    public float shockwaveRadius = 3f; // Radio de da�o alrededor del impacto
    public float damage = 50f; // Da�o a aplicar

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
        // Guardamos la posici�n del jugador en el momento del aviso
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

        // 4. Da�o si el jugador est� en el radio
        float distance = Vector3.Distance(player.position, targetPosition);
        Debug.Log("Distancia del jugador al centro de impacto: " + distance);

        if (distance <= shockwaveRadius)
        {
            Debug.Log("Jugador dentro del �rea: aplicar da�o (" + damage + ")");
            // Aqu� deber�as aplicar el da�o real al jugador
            // player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Jugador fuera del �rea: no recibe da�o");
        }

        // 5. Eliminar sombra
        Destroy(shadow);
    }
}