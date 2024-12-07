using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrido : MonoBehaviour
{
    public GameObject sweepPrefab; // Prefab del cubo que simula el barrido
    public Transform[] sweepPositions; // Posiciones donde aparecerán los cubos del barrido
    public float sweepInterval = 0.2f; // Tiempo entre la aparición de los cubos
    public GameObject projectilePrefab; // Prefab de los proyectiles
    public Transform[] projectileSpawns; // Puntos de aparición de los proyectiles
    public float projectileSpeed = 10f; // Velocidad de los proyectiles

    private bool isSweeping = false; // Controla si el barrido está en curso

    // Actualización por cuadro
    void Update()
    {
        // Detecta cuando se presiona la tecla "B" para ejecutar el barrido
        if (Input.GetKeyDown(KeyCode.B) && !isSweeping)
        {
            ExecuteSweep(); // Ejecuta el barrido cuando se presiona la tecla "B"
        }
    }

    // Método para ejecutar el barrido
    public void ExecuteSweep()
    {
        if (isSweeping) return; // Evita iniciar múltiples barridos al mismo tiempo
        StartCoroutine(SweepSequence()); // Llama a la corutina del barrido
    }

    // Corutina que gestiona el barrido
    private IEnumerator SweepSequence()
    {
        isSweeping = true; // Marca que el barrido está en curso

        // Crear los cubos del barrido uno tras otro
        foreach (var position in sweepPositions)
        {
            GameObject sweep = Instantiate(sweepPrefab, position.position, position.rotation);
            Destroy(sweep, 1f); // Destruir el cubo tras 1 segundo
            yield return new WaitForSeconds(sweepInterval); // Esperar un intervalo antes de crear el siguiente
        }

        // Disparar los proyectiles al finalizar el barrido
        foreach (var spawn in projectileSpawns)
        {
            GameObject projectile = Instantiate(projectilePrefab, spawn.position, spawn.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = spawn.forward * projectileSpeed; // Aplicar movimiento al proyectil
            Destroy(projectile, 5f); // Destruir el proyectil tras 5 segundos
        }

        isSweeping = false; // Marca que el barrido ha terminado
    }
}