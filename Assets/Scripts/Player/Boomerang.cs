using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public float velocidad = 10f; // Velocidad del boomerang
    private Vector3 direccion; // Dirección del boomerang

    // Método para establecer la dirección del boomerang
    public void Lanzar(Vector3 direccionInicial)
    {
        direccion = direccionInicial.normalized;
    }

    void Update()
    {
        // Mover el boomerang en la dirección establecida
        transform.position += direccion * velocidad * Time.deltaTime;
    }
}