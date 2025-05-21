using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArmProtector : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float maxHealth = 50f;
    private float currentHealth;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isProtecting = false;

    private IProtectable currentTarget;

    void Start()
    {
        originalPosition = transform.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        Vector3 destination = isProtecting ? targetPosition : originalPosition;
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        if (isProtecting && currentTarget != null && Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentTarget.SetProtected(true);
        }
    }

    public void Protect(IProtectable target)
    {
        currentTarget = target;
        targetPosition = target.GetTransform().position;
        isProtecting = true;
    }

    public void StopProtection()
    {
        if (currentTarget != null)
        {
            currentTarget.SetProtected(false);
            currentTarget = null;
        }

        isProtecting = false;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log(name + " recibió daño: " + amount); // Ver si recibe el golpe

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StopProtection();
            Debug.Log(name + " brazo destruido");
            Destroy(gameObject);
        }
    }
}