using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    public GameObject prefab; // The prefab to spawn
    public float spawnRate = 1f; // Time in seconds between spawns
    public float moveSpeed = 5f; // Speed at which the spawned objects move

    private float nextSpawnTime;
    private bool canSpawn = false;

    // Start is called before the first frame update
    void Start()
    {
        nextSpawnTime = Time.time + spawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn && Time.time >= nextSpawnTime)
        {
            SpawnPrefab();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnPrefab()
    {
        GameObject spawnedObject = Instantiate(prefab, transform.position, transform.rotation);
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * moveSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canSpawn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canSpawn = false;
        }
    }
}