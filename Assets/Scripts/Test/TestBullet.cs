using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public float speed = 10f; // Velocidad de la bala
    public float lifetime = 20f; // Tiempo de vida de la bala en segundos

    // Start is called before the first frame update
    void Start()
    {
        // Llamar al método DestroyBullet después de 'lifetime' segundos
        Invoke("DestroyBullet", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        // Ajustar la velocidad de la bala usando la escala de tiempo global
        transform.position += transform.forward * speed * PlayerMovement.bulletTimeScale * Time.deltaTime;
    }

    // Método que se llama cuando la bala colisiona con otro objeto
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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