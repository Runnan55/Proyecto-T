using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArmController : MonoBehaviour
{
    public Transform palmTargetPoint; // Posici�n donde va a caer la mano
    public GameObject shadowIndicatorPrefab; // Prefab de la sombra en el suelo
    public GameObject shockwaveEffectPrefab; // Prefab visual de la onda expansiva
    public float delayBeforeSmash = 3f; // Tiempo que tarda en caer la mano
    public float shockwaveRadius = 3f; // Radio de da�o alrededor del impacto
    public float damage = 50f; // Da�o a aplicar (si implementas sistema de vida del jugador)

    private Transform player;

    void Start()
    {
        // Encontramos al jugador por su tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Llamar a esta funci�n desde el boss para iniciar el ataque
    public void TriggerPalmSmash()
    {
        Debug.Log(" Palmada iniciada");
        StartCoroutine(DoPalmAttack());
    }

    private IEnumerator DoPalmAttack()
    {
        // 1. Aparece una sombra para avisar d�nde va a caer la mano
        GameObject shadow = Instantiate(shadowIndicatorPrefab, palmTargetPoint.position, Quaternion.identity);
        Debug.Log(" Sombra creada en: " + palmTargetPoint.position);

        // 2. Esperamos X segundos antes de que caiga la palmada
        yield return new WaitForSeconds(delayBeforeSmash);
        Debug.Log("Palmada ejecutada tras " + delayBeforeSmash + " segundos");

        // 3. Aparece la onda expansiva visual
        Instantiate(shockwaveEffectPrefab, palmTargetPoint.position, Quaternion.identity);
        Debug.Log(" Onda expansiva creada");

        // 4. Si el jugador est� dentro del �rea, se le hace da�o
        float distance = Vector3.Distance(player.position, palmTargetPoint.position);
        Debug.Log(" Distancia del jugador al centro de impacto: " + distance);

        if (distance <= shockwaveRadius)
        {
            Debug.Log(" Jugador dentro del �rea: aplicar da�o (" + damage + ")");
            // Aqu� deber�as llamar a tu sistema de da�o real
        }
        else
        {
            Debug.Log(" Jugador fuera del �rea: no recibe da�o");
        }

        // 5. Limpiamos la sombra del suelo
        Destroy(shadow);
    }
}