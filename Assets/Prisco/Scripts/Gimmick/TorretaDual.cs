using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TorretaDual : MonoBehaviour
{
    public GameObject bulletPrefabA; // Prefab de la bala A
    public GameObject bulletPrefabB; // Prefab de la bala B
    public GameObject firePointA; // Punto de disparo A
    public GameObject firePointB; // Punto de disparo B

    [Range(0.1f, 100f)]
    public float bulletSpeed = 10f; // Velocidad de la bala

    [Range(0.1f, 3f)]
    public float fireInterval = 1f; // Intervalo de disparo en segundos

    private float timeSinceLastFire = 0f; // Tiempo desde el último disparo

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Ajustar el tiempo acumulado usando la escala de tiempo del bullet time
        timeSinceLastFire += Time.deltaTime * MovimientoJugador.bulletTimeScale;

        // Disparar cuando se haya alcanzado el intervalo ajustado
        if (timeSinceLastFire >= fireInterval)
        {
            FireBullets();
            timeSinceLastFire = 0f; // Reiniciar el tiempo desde el último disparo
        }
    }

    void FireBullets()
    {
        // Instanciar la bala en firePointA
        GameObject bulletA = Instantiate(bulletPrefabA, firePointA.transform.position, firePointA.transform.rotation);
        TestBullet bulletScriptA = bulletA.GetComponent<TestBullet>();
        if (bulletScriptA != null)
        {
            bulletScriptA.speed = bulletSpeed; // Ajustar la velocidad de la bala
        }

        // Instanciar la bala en firePointB
        GameObject bulletB = Instantiate(bulletPrefabB, firePointB.transform.position, firePointB.transform.rotation);
        TestBullet bulletScriptB = bulletB.GetComponent<TestBullet>();
        if (bulletScriptB != null)
        {
            bulletScriptB.speed = bulletSpeed; // Ajustar la velocidad de la bala
        }
    }
}
