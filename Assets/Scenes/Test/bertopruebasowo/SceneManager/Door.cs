using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float doorDistance = 10f;
    public float doorTime = 2f;

    public IEnumerator Close()
    {
        Debug.Log("Cerrando puerta");
        
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        float timer = 0;
        while (timer < doorTime)
        {
            float moveAmount = doorDistance * (Time.deltaTime / doorTime);
            transform.Translate(0, moveAmount, 0);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator Open()
    {
        Debug.Log("Abriendo puerta");

        float timer = 0;
        while (timer < doorTime)
        {
            float moveAmount = doorDistance * (Time.deltaTime / doorTime);
            transform.Translate(0, -moveAmount, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}