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
                Destroy(gameObject);
            }
        }
    }
    [SerializeField] private Slider healthBar;
    [SerializeField] private float pushForce = 5f; // Fuerza del empuje
    [SerializeField] private float colorChangeDuration = 0.5f; // Duraci�n del cambio de color

   
    private Rigidbody rb;

    // Direcci�n de golpe predeterminada
    private Vector3 hitDirection = new Vector3(1, 0, 0); // Ejemplo: hacia la derecha

    // Inicializaci�n
    void Start()
    {
        maxHp = _hp;

        rb = GetComponent<Rigidbody>();
    }

    // M�todo para recibir da�o
    public void ReceiveDamage(float damage)
    {
        health -= damage;
        
     


        // Empujar al enemigo usando la direcci�n de golpe establecida
        if (rb != null)
        {
            rb.AddForce(hitDirection.normalized * pushForce, ForceMode.Impulse);
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Coroutine para cambiar el color del enemigo
    

    // M�todo para manejar la "muerte" del enemigo
   
}
