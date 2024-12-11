using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public float speed = 10f; // Velocidad de la bala
    public float lifetime = 20f; // Tiempo de vida de la bala en segundos

    public float startTime; // Tiempo de inicio para el tiempo de vida

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time; // Guardar el tiempo de inicio
    }

    // Update is called once per frame
    void Update()
    {
        // Ajustar la velocidad de la bala usando la escala de tiempo global
        transform.position += transform.forward * speed * MovimientoJugador.bulletTimeScale * Time.deltaTime;

        // Calcular el tiempo transcurrido desde la instanciación
        float elapsedTime = (Time.time - startTime) * MovimientoJugador.bulletTimeScale;

        // Destruir la bala después de que haya pasado su tiempo de vida
        if (elapsedTime >= lifetime)
        {
            DestroyBullet();
        }
    }

    // Método que se llama cuando la bala colisiona con otro objeto
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !MovimientoJugador.isDashing)
        {
            DestroyBullet();
        }
    }

    // Método para destruir la bala
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}