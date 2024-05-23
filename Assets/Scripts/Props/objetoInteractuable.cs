using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objetoInteractuable : MonoBehaviour
{
    public float pushforce = 10f;
    public Transform player;
    public float pushDistance = 5f;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if( distance < pushDistance )
        {
            Vector3 pushDirection = player.position - transform.position;
            pushDirection.y = 0;
            pushDirection.Normalize();

            Vector3 newPosition = player.position + pushDirection * pushforce * Time.deltaTime;

            player.position = newPosition;
        }
    }
}
