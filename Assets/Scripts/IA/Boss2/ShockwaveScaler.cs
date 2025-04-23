using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveScaler : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.localScale += Vector3.one * speed * Time.deltaTime;
    }
}