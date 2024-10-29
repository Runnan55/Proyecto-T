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
                if (level != null)
                {
                    level.EnemyDefeated(this);
                }
                Destroy(gameObject);
            }
        }
    }
    [SerializeField] private Slider healthBar;
    [SerializeField] private float pushForce = 5f;
    [SerializeField] private float colorChangeDuration = 0.5f;

    private Rigidbody rb;
    public Level level; // Referencia al nivel

    private Vector3 hitDirection = new Vector3(1, 0, 0);

    void Start()
    {
        maxHp = _hp;
        rb = GetComponent<Rigidbody>();
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;

        if (rb != null)
        {
            rb.AddForce(hitDirection.normalized * pushForce, ForceMode.Impulse);
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