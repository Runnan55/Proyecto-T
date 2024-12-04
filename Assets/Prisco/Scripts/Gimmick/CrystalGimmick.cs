using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGimmick : MonoBehaviour
{
    public Transform[] spawnPositions;
    public GameObject turretPrefab1;
    public GameObject turretPrefab2;
    public GameObject turretPrefab3;
    public Transform turretSpawnPosition1;
    public Transform turretSpawnPosition2;
    public Transform turretSpawnPosition3;
    private GameObject player;
    private int touchCount = 0;

    void Start()
    {
        Invoke("FindPlayer", 1f);
    }

    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            touchCount++;
            ChangePosition();

            if (touchCount == 1)
            {
                Instantiate(turretPrefab1, turretSpawnPosition1.position, Quaternion.identity);
            }
            else if (touchCount == 6)
            {
                Instantiate(turretPrefab2, turretSpawnPosition2.position, Quaternion.identity);
            }
            else if (touchCount == 11)
            {
                Instantiate(turretPrefab3, turretSpawnPosition3.position, Quaternion.identity);
            }
            else if (touchCount == 16)
            {
                Destroy(gameObject);
            }
        }
    }

    void ChangePosition()
    {
        int randomIndex = Random.Range(0, spawnPositions.Length);
        transform.position = spawnPositions[randomIndex].position;
    }
}