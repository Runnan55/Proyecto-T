using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour, IProtectable
{
    public float maxHealth = 33;
    private float currentHealth;
    private bool isDestroyed = false;
    private bool isProtected = false;

    public GameObject enemyPrefab;
    public float spawnInterval = 5f;

    public delegate void OnDestroyedHandler(BossSpawner spawner);
    public event OnDestroyedHandler OnDestroyed;

    private Coroutine spawnCoroutine;

    void Start()
    {
        ResetObject();
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed || isProtected) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDestroyed = true;
            StopSpawning();
            OnDestroyed?.Invoke(this);
            gameObject.SetActive(false);
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


            if (enemyPrefab != null)
            {
                Instantiate(enemyPrefab, transform.position + Vector3.up, Quaternion.identity);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SetProtected(bool state)
    {
        isProtected = state;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}