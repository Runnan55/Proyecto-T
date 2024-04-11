using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cono : MonoBehaviour
{
    [SerializeField] float detectDist;
    [SerializeField] float anguloDetect;

    Transform playerT;
    Vector3 direPlayer;
    float distPlayer;

    bool PlayerEnRango() {
        direPlayer = playerT.position - transform.position;
        distPlayer = direPlayer.magnitude;
        if (distPlayer <= detectDist) {
            return true;
        }
        return false;
    }

    bool DetectarAngulo() {
        float angle = Vector3.Angle(transform.forward, direPlayer);
        if (Mathf.Abs(angle) <= (anguloDetect / 2)) {
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (PlayerEnRango() && DetectarAngulo()) {
            print("Detecto al player");
        }
    }

}
