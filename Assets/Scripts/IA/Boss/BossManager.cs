using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;

    private void Start()
    {
       
    }

    public void EnemyDied()
    {
        // Comprueba qué enemigo ha muerto y activa al otro
        if (enemy1.activeSelf==false)
        {
            enemy2.SetActive(true);
        }
       
       
    }
}