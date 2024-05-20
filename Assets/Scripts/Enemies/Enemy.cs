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
            _hp = math.clamp(value, 0, 1000);

            if (health <= 0)
            {
                Debug.Log("Enemy defeated en enemy");
                if (doorManager != null) // Verificar si doorManager es null
                {
                    doorManager.EnemyDefeated(); 
                }
                Destroy(gameObject);
            }
        }    
    }

    public float damage = 10f;
    private Life playerLife;
    public GameObject floatingTextPrefab;
    public GameObject dropPrefab;

    private Vector3 dropspawn;
    public GameObject boss2;

    private WaveManager waveManager;
    public DoorManager doorManager;

    public float knockbackDistance = 2f;
    public float knockbackDuration = 0.1f;
       PlayerMovement playerMovement;


    public void Awake()
    {      
        doorManager = GameObject.FindGameObjectWithTag("DoorManager").GetComponent<DoorManager>();
    }

    void Start()
    {
          playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
        var waveManagerObject = GameObject.FindGameObjectWithTag("WaveManager");
        if (waveManagerObject != null)
        {
            waveManager = waveManagerObject.GetComponent<WaveManager>();
        }
        var doorManagerObject = GameObject.FindGameObjectWithTag("DoorManager");
        if (doorManagerObject != null) // Verificar si doorManagerObject es null
        {
            doorManager = doorManagerObject.GetComponent<DoorManager>();
        }
    }

    public virtual void ReceiveDamage(float amount) // (en segundos)
    {
        health -= amount;
        if (health <= 0)
        {
            DropItem();
            ActiveBoss();
            if (waveManager != null)
            {
                waveManager.EnemyDied();
            }
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

            dropspawn = new Vector3(transform.position.x, transform.position.y+ 1, transform.position.z);
            Instantiate(dropPrefab, dropspawn, Quaternion.identity);
        }
        }
    }
    void ActiveBoss()
    {
        if (boss2 != null)
        {

            boss2.SetActive(true);
          
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