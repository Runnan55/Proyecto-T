using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float velocidad = 10f; // Velocidad del boomerang
    public float magnetRange = 5f;
    public float magnetSpeed = 5f;
    private Vector3 direccion; // Dirección del boomerang
    
    // Método para establecer la dirección del boomerang
    public void Lanzar(Vector3 direccionInicial)
    {
        direccion = direccionInicial.normalized;
    }

    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector3 enemyDirection = (nearestEnemy.transform.position - transform.position).normalized;
            direccion = Vector3.Lerp(direccion, enemyDirection, magnetSpeed * Time.deltaTime);
        }

        transform.position += direccion * velocidad * Time.deltaTime;
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