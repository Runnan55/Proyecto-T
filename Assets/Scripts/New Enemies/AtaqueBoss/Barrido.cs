using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrido : MonoBehaviour
{
    public GameObject sweepPrefab; // Prefab del cubo que simula el barrido
    public Transform[] sweepPositions; // Posiciones relativas del barrido
    public float sweepInterval = 0.2f; // Tiempo entre la aparición de los cubos
    public GameObject projectilePrefab; // Prefab de los proyectiles
    public Transform[] projectileSpawns; // Puntos de aparición relativos de los proyectiles
    public float projectileSpeed = 10f; // Velocidad de los proyectiles

    private bool isSweeping = false; // Controla si el barrido está en curso

    void Update()
    {
        // Si necesitas lógica adicional durante el barrido
    }

    public void ExecuteSweep()
    {
        if (isSweeping) return; // Evita iniciar múltiples barridos al mismo tiempo
        StartCoroutine(SweepSequence()); // Llama a la corutina del barrido
    }

    private IEnumerator SweepSequence()
    {
        isSweeping = true; // Marca que el barrido está en curso

        float[] dodgeTimers = { 0.5f, 0.7f, 1f }; // Valores para cada instancia
        int timerIndex = 0; // Índice para iterar los valores

        // Crear los cubos del barrido en las posiciones relativas, orientados con el barrido
        foreach (var position in sweepPositions)
        {
            Vector3 worldPosition = transform.TransformPoint(position.localPosition);
            Quaternion worldRotation = transform.rotation * position.localRotation;

            GameObject sweep = Instantiate(sweepPrefab, worldPosition, worldRotation);

            // Asignar el valor dodgeTimer de acuerdo al índice actual
            if (sweep.TryGetComponent(out CubeAttack cubeAttack))
            {
                cubeAttack.dodgeTimer = dodgeTimers[timerIndex];
            }

            // Aumentar el índice, reiniciar si supera los límites
            timerIndex = (timerIndex + 1) % dodgeTimers.Length;

            Destroy(sweep, dodgeTimers[timerIndex]+0.3f); // Destruir el cubo tras 1 segundo
            yield return new WaitForSeconds(0f); // Esperar un intervalo antes de crear el siguiente
        }

        // Disparar los proyectiles en las posiciones relativas, orientados con el barrido
        foreach (var spawn in projectileSpawns)
        {
            Vector3 worldPosition = transform.TransformPoint(spawn.localPosition);
            Quaternion worldRotation = transform.rotation * spawn.localRotation;

            GameObject projectile = Instantiate(projectilePrefab, worldPosition, worldRotation);

            // Dirección hacia adelante basada en la orientación global
            Vector3 direction = transform.forward;

            // Inicializar el proyectil con la dirección calculada
            projectile.GetComponent<BouncingProyectil2>().Initialize(direction);
        }

        isSweeping = false; // Marca que el barrido ha terminado
    }
}