using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pruebaspawn : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        GameObject player = Instantiate(playerPrefab, null);
        PlayerStartLocation startLocation = GameObject.FindObjectOfType<PlayerStartLocation>();

        if (startLocation != null)
        {
            Vector3 startPosition = startLocation.gameObject.transform.position;
            if (startPosition.y < 1)
            {
                startPosition.y = 1;
                Debug.LogWarning("Core:: PlayerStartLocation was below safe height. Adjusting player position.");
            }

            player.transform.position = startPosition;
            Quaternion flatRotation = Quaternion.Euler(0.0f, startLocation.gameObject.transform.rotation.eulerAngles.y, 0.0f);
            player.transform.rotation = flatRotation;
            Debug.Log("Spawneando player en " + player.transform.position);
        }

        else
        {
            Debug.LogWarning("Core:: PlayerStartLocation was not found. Placing player at origin");
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
        }
    }

       void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
