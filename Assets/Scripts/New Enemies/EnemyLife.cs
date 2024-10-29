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
    [SerializeField] private float colorChangeDuration = 0.5f; // Duración del cambio de color

   
    private Rigidbody rb;

    // Dirección de golpe predeterminada
    private Vector3 hitDirection = new Vector3(1, 0, 0); // Ejemplo: hacia la derecha

    // Inicialización
    void Start()
    {
        maxHp = _hp;

        rb = GetComponent<Rigidbody>();
    }

    // Método para recibir daño
    public void ReceiveDamage(float damage)
    {
        health -= damage;
        
     


        // Empujar al enemigo usando la dirección de golpe establecida
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
    

    // Método para manejar la "muerte" del enemigo
   
}
