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
    public Animator healthBarAnimator;    

    private bool isDefeated = false;

    [SerializeField] private FMODUnity.EventReference hit;


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
                    //level.EnemyDefeated(this);
                }
                StartCoroutine(DestroyHealthBar());
                Destroy(gameObject, 0.2f); // Destruye el objeto después de 0.2 segundos
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

        if (healthBar != null)
        {
            //healthBarAnimator.Play("EnemyBarDeath");
            healthBar.gameObject.SetActive(false);
        }
    }

    public virtual void ReceiveDamage(float damage)
    {
        if (isDefeated) return; // Si el enemigo ya está derrotado, no hacer nada

        health -= damage;

      //  Debug.Log("Recibiendo daño: " + damage);

        if (newMaterial != null && enemyMeshRenderer != null)
        {
            StartCoroutine(ChangeMaterialTemporarily());
        }

        if (health <= 0 && !isDefeated)
        {
            isDefeated = true; // Marca al enemigo como derrotado
            if (level != null)
            {
                level.EnemyDefeated(this);
            }
            StartCoroutine(DestroyHealthBar());
            Destroy(gameObject, 0.2f); // Destruye el objeto después de 0.2 segundos
        }
        
        FMODUnity.RuntimeManager.PlayOneShot(hit);
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
        ChangeMaterial(newMaterial);

        // Espera 0.2 segundos
        yield return new WaitForSeconds(0.2f);

        // Restaura el material original
        ChangeMaterial(originalMaterial);
    }

    private IEnumerator DestroyHealthBar()
    {
        // Desasocia la barra de salud del objeto padre
        if (healthBar != null)
        {
            Debug.Log("ola");
            healthBar.transform.SetParent(null);
        }

        // Espera 0.2 segundos
        yield return new WaitForSeconds(0.2f);

        // Desactiva la barra de salud
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
    }

    private void ChangeMaterial(Material newMaterial)
    {
        if (enemyMeshRenderer != null && newMaterial != null)
        {
            enemyMeshRenderer.material = new Material(newMaterial);
        }
    }
}