using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour, IProtectable
{
    public float maxHealth = 33;
    private float currentHealth;
    private bool isDestroyed = false;
    private bool isProtected = false;
    public float spawnInterval = 10f;
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    public delegate void OnDestroyedHandler(BossSpawner spawner);
    public event OnDestroyedHandler OnDestroyed;

    public Material defaultMaterial;
    public Material protectedMaterial;
    private Renderer rend;
    private Coroutine spawnCoroutine;

    void Start()
    {
        rend = GetComponent<Renderer>();
        ResetObject();  // Inicializa el spawner
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"{name} recibi� intento de da�o. Protegido: {isProtected}");

        if (isDestroyed || isProtected)
        {
            Debug.Log($"{name} no recibe da�o por protecci�n o destrucci�n.");
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDestroyed = true;

            StopSpawning();
            gameObject.SetActive(false);
            OnDestroyed?.Invoke(this);

            StartCoroutine(RespawnDelay());  // Inicia espera para reaparecer
        }
    }

    private IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(10f);  // Espera tras destrucci�n
        ResetObject();
        Debug.Log($"{name} regenerado autom�ticamente tras 10s de estar destruido.");
    }

    public bool IsDestroyed()
    {
        return isDestroyed;
    }

    public void ResetObject()
    {
        currentHealth = maxHealth;
        isDestroyed = false;
        isProtected = false;
        gameObject.SetActive(true);
        StartSpawning();
    }

    private void StartSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    private void StopSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }

    private IEnumerator SpawnLoop()
    {
        while (!isDestroyed)
        {
            yield return new WaitForSeconds(2f);

            if (enemyPrefabs != null && enemyPrefabs.Count > 0)
            {
                int index = Random.Range(0, enemyPrefabs.Count);
                GameObject prefab = enemyPrefabs[index];
                Instantiate(prefab, transform.position + Vector3.up, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetProtected(bool state)
    {
        isProtected = state;
        if (rend != null)
            rend.material = isProtected ? protectedMaterial : defaultMaterial;

        Debug.Log($"{name} Protecci�n activa: {state}");
    }

    public Transform GetTransform()
    {
        return transform;
    }
}