using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class dOMMY : EnemyLife
{
    #region Variables
    public float tiempoRevivir = 3f;
    public Mesh revivirMesh; 
    private Mesh originalMesh;
    private MeshFilter meshFilter; 
    public GameObject areaEffectPrefab;
    #endregion

    #region Unity Methods
    void Start()
    {
        meshFilter = GetComponentInChildren<MeshFilter>(); 
        if (meshFilter != null)
        {
            originalMesh = meshFilter.mesh; 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DisparoCargado>() != null)
        {
            antiRevivir = true; 
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
                ApplyAreaEffect();

                if (level != null)
                {
                    level.EnemyDefeated(this);
                }

                Destroy(gameObject, 0.2f); 
            }
            else
            {
                if (!antiRevivir)
                {
                    StartCoroutine(DelayedRevivir()); 
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
        
        if (areaEffectPrefab != null)
        {
            Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    #endregion

    #region Reviving Mechanic
    private IEnumerator DelayedRevivir()
    {
        yield return null; 
        StartCoroutine(Revivir()); 
    }

    IEnumerator Revivir()
    {
        if (antiRevivir) yield break; 

        Debug.Log("reviviendo");
        if (revivirMesh != null && meshFilter != null)
        {
            meshFilter.mesh = revivirMesh; 
            yield return new WaitForSeconds(tiempoRevivir);
            meshFilter.mesh = originalMesh; 
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

