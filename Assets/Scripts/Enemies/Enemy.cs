using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public Life playerLife;
    public GameObject floatingTextPrefab;
    public GameObject dropPrefab;

    void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player
    }

    void Update()
    {
        if (health <= 0)
        {
            DropItem();
            Destroy(gameObject);
        }
    }

    public void ReceiveDamage(float amount) // (en segundos)
    {
        health -= amount;
        if (health <= 0)
        {
            DropItem();
            Destroy(gameObject);
        }
        
        Debug.Log("Daño infringido: " + amount + ", Vida: " + health);

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
        Vector3 offset = new Vector3(0, 2, 0); // Ajusta el valor 2 para cambiar la altura del desplazamiento
        var go = Instantiate(floatingTextPrefab, transform.position + offset, Quaternion.identity);
        go.GetComponent<DamageText>().SetText(amount.ToString());
    }

    void DropItem()
    {
        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity); 
        }
    }

    public void Activar()
    {    
        gameObject.SetActive(true); 
    }
}