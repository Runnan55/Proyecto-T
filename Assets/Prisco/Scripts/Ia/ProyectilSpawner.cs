using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilSpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab del enemigo a instanciar

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity); // Instanciar enemigo en la posición del proyectil
        Destroy(gameObject); // Destruir el proyectil
    }
}
