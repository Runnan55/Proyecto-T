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
    public Material defaultMaterial;
    public Material protectedMaterial;
    private Renderer rend;
    private Coroutine spawnCoroutine;

    private float destroyedTime;
    private bool pendingRegen;

    void Start()
    {
        rend = GetComponent<Renderer>();
        ResetObject();  // Restablece el estado del spawner
    }

    // Método para recibir daño
    public void TakeDamage(float damage)
    {
        Debug.Log($"{name} recibió intento de daño. Protegido: {isProtected}");

        if (isDestroyed || isProtected)
        {
            Debug.Log($"{name} no recibe daño por protección o destrucción.");
            return;
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDestroyed = true;
            destroyedTime = Time.time;
            pendingRegen = true;

            StopSpawning();
            gameObject.SetActive(false);
            OnDestroyed?.Invoke(this);
        }
    }

    // Método Update, ya no controla la regeneración
    void Update()
    {
        if (pendingRegen && Time.time >= destroyedTime + 10f)
        {
            ResetObject();
            pendingRegen = false;
            Debug.Log($"{name} regenerado automáticamente tras 10s exactos desde su destrucción.");
        }
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
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    // Método para proteger el spawner (según la interfaz IProtectable)
    public void SetProtected(bool state)
    {
        isProtected = state;
        if (rend != null)
            rend.material = isProtected ? protectedMaterial : defaultMaterial;

        Debug.Log($"{name}  Protección activa: {state}");
    }

    // Método para obtener la posición del spawner (según la interfaz IProtectable)
    public Transform GetTransform()
    {
        return transform;  // Devuelve el transform del spawner
    }
}