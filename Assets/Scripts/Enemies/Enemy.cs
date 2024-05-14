using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public DoorManager doorManager;
    public float _hp = 20f;
    public float health {
        get { return _hp; }
        set 
        {
            _hp = math.clamp(value, 0, 100);

            if (health == 0)
            {
                DropItem();
               // doorManager.EnemyDefeated(); 
                Destroy(gameObject);
            }
        }    
    }

    public float damage = 10f;
    public Life playerLife;
    public GameObject floatingTextPrefab;
    public GameObject dropPrefab;

    private WaveManager waveManager;

    public float knockbackDistance = 2f;
    public float knockbackDuration = 0.1f;
       PlayerMovement playerMovement;


public void Awake()
{
 playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
}
    void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
        waveManager = GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>(); 
        doorManager = GameObject.FindGameObjectWithTag("DoorManager").GetComponent<DoorManager>();
        
    }

       public virtual void ReceiveDamage(float amount) // (en segundos)
    {
        health -= amount;
        if (health <= 0)
        {
            DropItem();
            //waveManager.EnemyDied();
            ShowFloatingText(amount);
            StartCoroutine(DestroyAfterDelay(1f));
        }

        else
        {
           FloatUltimate();
            Debug.Log("Daño inflingido: " + amount + ", Vida: " + health);
            ShowFloatingText(amount);
        
        }
    }

        IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
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
        Debug.Log("ShowFloatingText");
        Vector3 offset = new Vector3(0, 2, 0); // la y es es la altura
        var go = Instantiate(floatingTextPrefab, transform.position + offset, Quaternion.identity);
        go.GetComponentInChildren<DamageText>().SetText(amount.ToString());
    }

    void DropItem()
    {
        if (dropPrefab != null)
        {
            float chance = UnityEngine.Random.Range(0.0f, 1.0f);
        
        // Comprueba si el número generado es menor o igual a 0.3 (30%)
        if (chance <= 0.3f)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }
        }
    }

    public void Activar()
    {    
        gameObject.SetActive(true); 
    }


    #region DEBUG //    ***** DEBUG ***** 
/*     void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("moricion");
            //ReceiveDamage(health);
            //Destroy(gameObject);
            waveManager.EnemyDied();
            Debug.Log("moricion fin");
        }    
    } */
    #endregion DEBUG

public void FloatUltimate()
{
  playerMovement.IncrementFloatVariable();
}

}