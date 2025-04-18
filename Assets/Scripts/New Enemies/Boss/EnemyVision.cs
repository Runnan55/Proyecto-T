using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] float detectDist;
    [SerializeField] float anguloDetect;
    [SerializeField] LayerMask obstacleMask;

    private Transform player;
    Vector3 direPlayer;
    float distPlayer;

    private void Start()
    {
        StartCoroutine(FindPlayerWithDelay());
    }

    private IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(1f);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'");
        }
    }
    public bool PlayerEnRango()
    {
        if (player == null)
        {
            return false;
        }
        direPlayer = player.position - transform.position;
        distPlayer = direPlayer.magnitude;
        if (distPlayer <= detectDist)
        {
            return true;
        }
        return false;
    }

    public bool DetectarAngulo()
    {
        float angle = Vector3.Angle(transform.forward, direPlayer);
        if (Mathf.Abs(angle) <= (anguloDetect / 2))
        {
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (PlayerEnRango() && DetectarAngulo())
        {
            //print("Detecto al player");
        }
    }

    public bool HayObstaculoEntre()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direPlayer, out hit, distPlayer, obstacleMask))
        {
            // Si el rayo golpea un objeto en la capa de obst�culos,
            // significa que hay un obst�culo entre el enemigo y el jugador
            return true;
        }
        return false;
    }
}

