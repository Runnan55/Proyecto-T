using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float _hp = 20f;
    public float health {
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

    public WaveManager waveManager;
    public DoorManager doorManager;

    public float knockbackDistance = 2f;
    public float knockbackDuration = 0.1f;
    public Level level;
       PlayerMovement playerMovement;


    

    void Start()
    { 
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
    }

    public virtual void ReceiveDamage(float amount) // (en segundos)
    {
        health -= amount;
        if (health <= 0)
        {
            DropItem();
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

            dropspawn = new Vector3(transform.position.x, transform.position.y+ 1, transform.position.z);
            Instantiate(dropPrefab, dropspawn, Quaternion.identity);
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