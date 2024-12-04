using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFallingTrap : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int numberOfProjectiles = 16;
    public float explosionForce = 10f;
    public float fallSpeed = 10f;
    public float fallHeight = 25f;
    public bool respawnInfinite = false;
    public float respawnDelay = 5f;

    private Rigidbody rb;
    private Vector3 initialPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        StartFalling();
    }

    private void StartFalling()
    {
        transform.position = new Vector3(initialPosition.x, fallHeight, initialPosition.z);
        rb.velocity = new Vector3(0, -fallSpeed, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Explode();
            if (respawnInfinite)
            {
                StartCoroutine(RespawnAfterDelay(respawnDelay));
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void Explode()
    {
        float angleStep = 360f / numberOfProjectiles;
        float angle = 0f;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * 0.5f;
            float projectileDirZPosition = transform.position.z + Mathf.Cos((angle * Mathf.PI) / 180) * 0.5f;

            Vector3 projectileVector = new Vector3(projectileDirXPosition, transform.position.y, projectileDirZPosition);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized;

            GameObject tempProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(projectileMoveDirection));
            tempProjectile.GetComponent<Flecha>().speed = explosionForce;

            angle += angleStep;
        }
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartFalling();
    }
}