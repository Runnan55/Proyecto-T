using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float _hp = 20f;
    private float maxHp;
    public float health
    {
        get { return _hp; }
        set
        {
            _hp = math.clamp(value, 0, 200000000);

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public float damage = 10f;
    private Life playerLife;
    public GameObject floatingTextPrefab;
    public GameObject dropPrefab;

    private Vector3 dropspawn;

    public float knockbackDistance = 2f;
    public float knockbackDuration = 0.1f;
    public Level level;
    PlayerMovement playerMovement;

    public GameObject damageEffect;

    [Range(-100f, 100f)]
    public float empujeF;

    public bool empujar = true;

    public float DropRate = 0.3f;

    // Configuración para el cambio de material
    public float materialChangeDuration = 0.2f; // Tiempo que el enemigo se mantendrá con otro material
    public Material damageMaterial;             // Material que se aplicará cuando reciba daño
    private SkinnedMeshRenderer childSkinnedMeshRenderer;  // Referencia al SkinnedMeshRenderer del GameObject hijo
    private Material originalMaterial;           // Para guardar el material original del enemigo


    void Start()
    {
        maxHp = _hp;
        StartCoroutine(InitializePlayerComponents());
        // Obtener el MeshRenderer del GameObject hijo "OriginalMesh"
        Transform originalMeshTransform = transform.Find("OriginalMesh");
        if (originalMeshTransform != null)
        {
            childSkinnedMeshRenderer = originalMeshTransform.GetComponent<SkinnedMeshRenderer>();
            if (childSkinnedMeshRenderer != null)
            {
                // Guardar el material original
                originalMaterial = childSkinnedMeshRenderer.material;
            }
            else
            {
                Debug.LogError("No se encontró el SkinnedMeshRenderer en el GameObject hijo 'OriginalMesh'.");
            }
        }
    }

    public IEnumerator InitializePlayerComponents()
    {
        yield return new WaitForSeconds(0.25f);
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
    }

    public virtual void ReceiveDamage(float amount) // (en segundos)
    {
        health -= amount;

        // Si la vida está en 25% o menos, desactivar empuje
        if (health <= maxHp * 0.25f)
        {
            empujar = false;
        }

        if (health <= 0)
        {
            ShowFloatingText(amount);
            StartCoroutine(DestroyAfterDelay(1f));

            if (level != null)
            {
                level.EnemyDefeated(this);
            }
        }
        else
        {
            FloatUltimate();
            //Debug.Log("Daño inflingido: " + amount + ", Vida: " + health);
            ShowFloatingText(amount);
        }

        if (damageEffect != null)
        {
            damageEffect.SetActive(true);

            Invoke("DisableDamageEffect", 2f);
        }

        if (empujar)
        {
            Empuje();
        }


        StartCoroutine(ChangeMaterialOnDamage());
    }

    // Corutina que gestiona el cambio temporal de material en el hijo "OriginalMesh"
    private IEnumerator ChangeMaterialOnDamage()
    {
        if (childSkinnedMeshRenderer != null && damageMaterial != null)
        {
            // Cambiar el material del SkinnedMeshRenderer al material de daño
            childSkinnedMeshRenderer.material = damageMaterial;

            // Esperar el tiempo definido
            yield return new WaitForSeconds(materialChangeDuration);

            // Volver al material original
            childSkinnedMeshRenderer.material = originalMaterial;
        }
    }
    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

   

    public void Empuje()
    {
        Rigidbody enemyRigidbody = GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            float force = empujeF;
            enemyRigidbody.AddForce(-transform.forward * force, ForceMode.Impulse);
        }
    }


    public virtual void AttackPlayer()
    {
        if (!playerLife.isInvincible)
        {
            Vector3 knockbackDirection = (playerLife.transform.position - transform.position).normalized;
            float knockbackSpeed = knockbackDistance / knockbackDuration;
            playerLife.StartCoroutine(playerLife.Knockback(knockbackDirection, knockbackDuration, knockbackSpeed));
            playerLife.ModifyTime(-damage);
        }
    }

    //* TEXTO
    void ShowFloatingText(float amount)
    {
        //Debug.Log("ShowFloatingText");
        Vector3 offset = new Vector3(0, 2, 0); // la y es es la altura
        var go = Instantiate(floatingTextPrefab, transform.position + offset, Quaternion.identity);
        go.GetComponentInChildren<DamageText>().SetText(amount.ToString());
    }

  

    public void Activar()
    {
        gameObject.SetActive(true);
    }


 

    public void FloatUltimate()
    {
        playerMovement.IncrementFloatVariable();
    }


    
  
}