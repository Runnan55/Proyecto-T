using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float velocidad = 10f; // Velocidad del boomerang
    public float magnetRange = 5f;
    public float magnetSpeed = 5f;
    public float serpentineFrequency = 2f; // Frecuencia del movimiento en "S"
    public float serpentineMagnitude = 0.5f; // Magnitud del movimiento en "S"
    private Vector3 direccion; // Dirección del boomerang
    private float serpentineOffset; // Desplazamiento para el movimiento en "S"

    // Método para establecer la dirección del boomerang
    public void Lanzar(Vector3 direccionInicial)
    {
        direccion = direccionInicial.normalized;
        serpentineOffset = Random.Range(0f, 2f * Mathf.PI); // Desplazamiento aleatorio para el movimiento en "S"
    }

    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector3 enemyDirection = (nearestEnemy.transform.position - transform.position).normalized;
            direccion = Vector3.Lerp(direccion, enemyDirection, magnetSpeed * Time.deltaTime);
        }

        // Movimiento en "S"
        Vector3 serpentine = transform.right * Mathf.Sin(Time.time * serpentineFrequency + serpentineOffset) * serpentineMagnitude;
        Vector3 moveDirection = direccion + serpentine;

        transform.position += moveDirection * velocidad * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance && distance <= magnetRange)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}