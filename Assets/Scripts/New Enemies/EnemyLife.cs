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

    public GameObject floatingTextPrefab;

    public float health
    {
        get { return _hp; }
        set
        {
            _hp = math.clamp(value, 0, 200000000);

            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(_hp < maxHp);
                healthBar.value = _hp / maxHp;
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
    }

    [SerializeField] private Slider healthBar;
    public Level level; // Referencia al nivel

    [Header("Materiales")]
    public Material newMaterial; // Material para aplicar cuando reciba da�o
    private Material originalMaterial; // Para restaurar el material original
    private MeshRenderer enemyMeshRenderer; // MeshRenderer del enemigo

    void Start()
    {
        maxHp = _hp;

        // Intenta obtener el MeshRenderer desde el propio objeto, sus hijos o alg�n objeto padre
        enemyMeshRenderer = GetComponentInChildren<MeshRenderer>();

        if (enemyMeshRenderer != null)
        {
            originalMaterial = enemyMeshRenderer.material;
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    public virtual void ReceiveDamage(float damage)
    {
        health -= damage;

        Debug.Log("Recibiendo da�o: " + damage);

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

        ShowFloatingText(damage);
    }

    void ShowFloatingText(float amount)
    {
        Vector3 offset = new Vector3(0, 2, 0); // la y es la altura
        var go = Instantiate(floatingTextPrefab, transform.position + offset, Quaternion.identity);
        go.GetComponentInChildren<DamageText>().SetText(amount.ToString());
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
