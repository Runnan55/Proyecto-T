using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class dOMMY : EnemyLife
{
    #region Variables
    public float tiempoRevivir = 3f;
    public Mesh revivirMesh; // Mesh to use while reviving
    private Mesh originalMesh;
    private MeshFilter meshFilter;
 
    public GameObject areaEffectPrefab; // Prefab for the area effect visual
    #endregion

    #region Unity Methods
    void Start()
    {
        meshFilter = GetComponentInChildren<MeshFilter>(); // Get the MeshFilter component
        if (meshFilter != null)
        {
            originalMesh = meshFilter.mesh; // Store the original mesh
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DisparoCargado>() != null)
        {
            antiRevivir = true; // Set antiRevivir immediately

            if (health <= 0)
            {
                StopAllCoroutines();
                ApplyAreaEffect();

                if (level != null)
                {
                    level.EnemyDefeated(this);
                }

                Destroy(gameObject, 0.2f);
            }
        }
    }
    #endregion

    #region Damage and Effects
    public override void CalcularDamage()
    {
        if (health <= 0)
        {
            if (antiRevivir)
            {
                ApplyAreaEffect(); // Apply area effect if antiRevivir is true

                if (level != null)
                {
                    level.EnemyDefeated(this);
                }

                Destroy(gameObject, 0.2f); // Destroy the enemy
            }
            else
            {
                if (!antiRevivir) // Double-check antiRevivir before reviving
                {
                    StartCoroutine(DelayedRevivir()); // Use a delayed coroutine
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void ApplyAreaEffect()
    {
        // Instantiate the area effect prefab without applying any effect
        if (areaEffectPrefab != null)
        {
            Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    #endregion

    #region Reviving Mechanic
    private IEnumerator DelayedRevivir()
    {
        yield return null; // Wait for the next frame to ensure antiRevivir is set
        StartCoroutine(Revivir()); // Start the Revivir coroutine
    }

    IEnumerator Revivir()
    {
        if (antiRevivir) yield break; // Prevent execution if antiRevivir is true

        Debug.Log("reviviendo");
        if (revivirMesh != null && meshFilter != null)
        {
            meshFilter.mesh = revivirMesh; // Change to the reviving mesh
            yield return new WaitForSeconds(tiempoRevivir);
            meshFilter.mesh = originalMesh; // Restore the original mesh
        }
        else
        {
            yield return new WaitForSeconds(tiempoRevivir);
        }

        _hp = 100;
        antiRevivir = false;
    }
    #endregion
}

