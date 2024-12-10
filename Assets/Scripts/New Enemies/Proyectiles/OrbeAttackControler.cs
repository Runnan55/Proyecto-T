using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbeAttackControler : MonoBehaviour
{
    [Header("Prefab de Orbe y Parámetros")]
    public GameObject orbePrefab; // Prefab del orbe
    public Transform bossTransform; // Referencia al transform del boss (para generar orbes en su alrededor)
    public float radius = 5f; // Radio donde se generan los orbes
    public int orbeCount = 10; // Cantidad de orbes a generar

    // Método para realizar la descarga de orbes
    public void PerformOrbeAttack()
    {
        Debug.Log("Realizando Descarga de Orbes");

        // Calcula el ángulo entre cada orbe en radianes
        float angleStep = 360f / orbeCount;

        // Genera los orbes en posiciones distribuidas uniformemente
        for (int i = 0; i < orbeCount; i++)
        {
            // Calcula el ángulo actual en radianes
            float angle = i * angleStep * Mathf.Deg2Rad;

            // Calcula la posición del orbe en el círculo
            Vector3 orbePosition = new Vector3(
                bossTransform.position.x + Mathf.Cos(angle) * radius,
                bossTransform.position.y, // Mantén la altura constante
                bossTransform.position.z + Mathf.Sin(angle) * radius
            );

            // Instancia un solo orbe en la posición calculada
            GameObject orbe = Instantiate(orbePrefab, orbePosition, Quaternion.identity);

            // Agregar dirección o movimiento al orbe (hacia afuera del círculo)
            Vector3 direction = (orbePosition - bossTransform.position).normalized;
            orbe.GetComponent<Rigidbody>().velocity = direction * 5f; // Velocidad del orbe

            // Ajustar la rotación del orbe para que mire hacia afuera
            orbe.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}