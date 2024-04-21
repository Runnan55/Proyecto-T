using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private float _hp = 50f;
    public float health {
        get { return _hp; }
        set 
        {
            _hp = math.clamp(value, 0, 100);

            if (health == 0)
            {
                DropItem();
                Destroy(gameObject);
            }
        }    
    }

    public float damage = 10f;
    public Life playerLife;
    public GameObject floatingTextPrefab;
    public GameObject dropPrefab;

    public WaveManager waveManager;

    void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
        waveManager = GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>(); 
    }

    public void ReceiveDamage(float amount) // (en segundos)
    {
        health -= amount;
        if (health <= 0)
        {
            DropItem();
            Destroy(gameObject);
            waveManager.EnemyDied();
        }
        
        Debug.Log("Daño inflingido: " + amount + ", Vida: " + health);

        if (health > 0)
        {
            ShowFloatingText(amount);
        }
    }

    public virtual void AttackPlayer()
    {
        playerLife.ModifyTime(-damage);
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
}