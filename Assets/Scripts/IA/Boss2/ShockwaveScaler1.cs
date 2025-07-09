using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveScaler1 : MonoBehaviour
{
    public float scaleSpeed = 2f;
    public float maxScale = 5f;

    void Update()
    {
        if (transform.localScale.x < maxScale)
        {
            float newScale = transform.localScale.x + scaleSpeed * Time.deltaTime;
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}