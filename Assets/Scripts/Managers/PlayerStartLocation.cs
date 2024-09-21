using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartLocation : MonoBehaviour
{
    //Simplemente dibuja en el editor una bola para localizar el spawn

    #if UNITY_EDITOR

            void OnDrawGizmos()
            {
                Color c = Color.green;
                c.a = 0.5f;
                Gizmos.color = c;

                Gizmos.DrawSphere(transform.position, 0.75f);
                Gizmos.DrawWireSphere(transform.position, 0.75f);
            }

    #endif
}
