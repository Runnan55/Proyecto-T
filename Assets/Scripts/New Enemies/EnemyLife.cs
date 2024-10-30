using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

public class EnemyLife : MonoBehaviour
{
    [Header("Vida del Enemigo")]
    public float _hp = 100f;
    private float maxHp;

    public float health
    {
        get { return _hp; }
        set
        {
            _hp = math.clamp(value, 0, 200000000);

            if (health <= 0)
            {
                if (level != null)
                {
                    level.EnemyDefeated(this);
                }
                Destroy(gameObject);
            }
        }
    }

    [SerializeField] private Slider healthBar;
    public Level level; // Referencia al nivel

    [Header("Materiales")]
    public Material newMaterial; // Material para aplicar cuando reciba daño
    private Material originalMaterial; // Para restaurar el material original
    private MeshRenderer enemyMeshRenderer; // MeshRenderer del enemigo

    void Start()
    {
        maxHp = _hp;

        // Intenta obtener el MeshRenderer desde el propio objeto, sus hijos o algún objeto padre
        enemyMeshRenderer = GetComponentInChildren<MeshRenderer>();

        if (enemyMeshRenderer != null)
        {
            originalMaterial = enemyMeshRenderer.material;
        }
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;

        Debug.Log("Recibiendo daño: " + damage);

        if (newMaterial != null && enemyMeshRenderer != null)
        {
            StartCoroutine(ChangeMaterialTemporarily());
        }

        if (health <= 0)
        {
            if (level != null)
            {
                level.EnemyDefeated(this);
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator ChangeMaterialTemporarily()
    {
        // Cambia el material al nuevo
        enemyMeshRenderer.material = newMaterial;

        // Espera 0.2 segundos
        yield return new WaitForSeconds(0.2f);

        // Restaura el material original
        enemyMeshRenderer.material = originalMaterial;
    }
}
