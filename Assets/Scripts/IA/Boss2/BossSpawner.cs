using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour, IProtectable
{
     public float maxHealth = 33;  // Salud máxima del spawner
    private float currentHealth;
    private bool isDestroyed = false;
    private bool isProtected = false;
    public float spawnInterval = 10f;  // Tiempo entre cada spawn de enemigos
    public List<GameObject> enemyPrefabs = new List<GameObject>();  // Lista de enemigos a spawn

    public delegate void OnDestroyedHandler(BossSpawner spawner);
    public event OnDestroyedHandler OnDestroyed;

    private Coroutine spawnCoroutine;

    void Start()
    {
        ResetObject();  // Restablece el estado del spawner
    }

    // Método para recibir daño
    public void TakeDamage(float damage)
    {
        if (isDestroyed || isProtected) return;  // No recibe daño si está destruido o protegido

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDestroyed = true;  // El spawner ha sido destruido
            StopSpawning();  // Detiene la spawn de enemigos
            gameObject.SetActive(false);  // Desactiva el spawner
            OnDestroyed?.Invoke(this);  // Notifica que el spawner ha sido destruido
        }
    }

    // Método Update, ya no controla la regeneración
    void Update()
    {
        // Ahora solo controlamos si el spawner está destruido y si su evento de regeneración es gestionado por el Boss
    }

    public bool IsDestroyed()
    {
        return isDestroyed;  // Retorna si el spawner está destruido
    }

    // Método para restablecer el spawner a su estado inicial
    public void ResetObject()
    {
        currentHealth = maxHealth;  // Restablece la salud del spawner
        isDestroyed = false;  // Marca que el spawner no está destruido
        isProtected = false;  // Asegura que no esté protegido
        gameObject.SetActive(true);  // Reactiva el spawner
        StartSpawning();  // Inicia la spawn de enemigos
    }

    // Método para comenzar a generar enemigos
    private void StartSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop());  // Inicia la generación de enemigos en un loop
    }

    // Método para detener la generación de enemigos
    private void StopSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);  // Detiene el coroutine
    }

    // Método para generar enemigos
    private IEnumerator SpawnLoop()
    {
        while (!isDestroyed)
        {
            // Solo se invoca un enemigo de manera aleatoria
            if (enemyPrefabs != null && enemyPrefabs.Count > 0)
            {
                int index = Random.Range(0, enemyPrefabs.Count);  // Elige un enemigo aleatorio de la lista
                GameObject prefab = enemyPrefabs[index];
                Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity);  // Crea el enemigo en el spawner
            }

            // Espera el intervalo antes de generar el siguiente enemigo
            yield return new WaitForSeconds(spawnInterval);  // Tiempo entre cada spawn
        }
    }

    // Método para proteger el spawner (según la interfaz IProtectable)
    public void SetProtected(bool state)
    {
        isProtected = state;  // Marca el spawner como protegido o no
    }

    // Método para obtener la posición del spawner (según la interfaz IProtectable)
    public Transform GetTransform()
    {
        return transform;  // Devuelve el transform del spawner
    }
}