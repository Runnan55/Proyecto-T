using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public float maxHealth = 33;
    private float currentHealth;
    private bool isDestroyed = false;

    public delegate void OnDestroyedHandler(BossSpawner obj);
    public event OnDestroyedHandler OnDestroyed;

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
    }
}