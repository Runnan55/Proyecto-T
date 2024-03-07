using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorrarProfe : MonoBehaviour
{
 [SerializeField] float detectAngle;
 [SerializeField] float detectDistance;

 [SerializeField] Vector3 playerPos;

    void Update()
    {
        float dist = (playerPos - transform.position).magnitude;
        if (dist < detectDistance)
        {
            Vector3 direction = (playerPos - transform.position).normalized;
            float angle = Vector3.Angle(direction, transform.forward);
            if (angle < detectAngle * 0.5f)
            {
                Debug.Log("Player detected");
            }
        }   

    }
}
