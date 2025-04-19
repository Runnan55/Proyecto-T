using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public float maxHealth = 33;
    private float currentHealth;
    private bool isDestroyed = false;

    public GameObject enemyPrefab;  // Enemigo que va a invocar
    public float spawnInterval = 1f;

    public delegate void OnDestroyedHandler(BossSpawner obj);
    public event OnDestroyedHandler OnDestroyed;

    private Coroutine spawnCoroutine;
    public Transform spawnPoint;

    void Start()
    {
        ResetObject();
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDestroyed = true;
            Debug.Log($"{gameObject.name} ha sido destruido");
            StopSpawning(); // Detenemos la invocación
            OnDestroyed?.Invoke(this);
            gameObject.SetActive(false); // ocultamos el objeto en vez de destruirlo
        }
    }

    public bool IsDestroyed()
    {
        return isDestroyed;
    }

    public void ResetObject()
    {
        currentHealth = maxHealth;
        isDestroyed = false;
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
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}