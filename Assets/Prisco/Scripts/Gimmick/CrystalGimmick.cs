using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private Transform lastPosition;
    private Renderer objectRenderer;
    private Collider objectCollider;

    public int touchMax = 20;
    public int touchCount1 = 1;
    public int touchCount2 = 6;
    public int touchCount3 = 11;

    void Start()
    {
        Invoke("FindPlayer", 1f);
        lastPosition = transform;
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
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
            StartCoroutine(ChangePosition());

            if ((touchCount == touchCount1) &&  (turretPrefab1 != null))
            {
                turretPrefab1 = Instantiate(turretPrefab1, turretSpawnPosition1.position, Quaternion.identity);
            }
            else if ((touchCount == touchCount2) && (turretPrefab2 != null))
            {
                turretPrefab2 = Instantiate(turretPrefab2, turretSpawnPosition2.position, Quaternion.identity);
            }
            else if ((touchCount == touchCount3) && (turretPrefab3 != null))
            {
                turretPrefab3 = Instantiate(turretPrefab3, turretSpawnPosition3.position, Quaternion.identity);
            }
            else if (touchCount == touchMax)
            {
                if (turretPrefab1 != null) Destroy(turretPrefab1);
                if (turretPrefab2 != null) Destroy(turretPrefab2);
                if (turretPrefab3 != null) Destroy(turretPrefab3);
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator ChangePosition()
    {
        // Desactivar la visibilidad y el collider del objeto
        objectRenderer.enabled = false;
        objectCollider.enabled = false;

        yield return new WaitForSeconds(0.5f);

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, spawnPositions.Length);
        } while (spawnPositions[randomIndex] == lastPosition);

        lastPosition = spawnPositions[randomIndex];
        transform.position = spawnPositions[randomIndex].position;

        // Activar la visibilidad y el collider del objeto
        objectRenderer.enabled = true;
        objectCollider.enabled = true;
    }
}