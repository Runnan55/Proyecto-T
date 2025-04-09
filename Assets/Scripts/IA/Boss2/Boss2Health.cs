using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Health : MonoBehaviour
{
    public BossSpawner[] bossSpawner;
    private int currentPhase = 1;
    private int totalHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = totalHealth;
        SubscribeToObjects();
    }

    void SubscribeToObjects()
    {
        foreach (var obj in bossSpawner)
        {
            obj.OnDestroyed += HandleObjectDestroyed;
        }
    }

    private void HandleObjectDestroyed(BossSpawner destroyedObj)
    {
        if (AllObjectsDestroyed())
        {
            if (currentPhase == 1)
            {
                StartPhaseTwo();
            }
            else
            {
                Die();
            }
        }
    }

    private bool AllObjectsDestroyed()
    {
        foreach (var obj in bossSpawner)
        {
            if (!obj.IsDestroyed())
                return false;
        }
        return true;
    }

    private void StartPhaseTwo()
    {
        Debug.Log(" Fase 2 activada");
        currentPhase = 2;
        currentHealth = totalHealth / 2;

        foreach (var obj in bossSpawner)
        {
            obj.ResetObject(); // restauramos la vida y visibilidad
        }
    }

    private void Die()
    {
        Debug.Log(" ¡El boss ha muerto definitivamente!");
        Destroy(gameObject);
    }
}