using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoAlterGiratorio : MonoBehaviour
{
    // Variable pública para ajustar la velocidad de rotación
    public float velocidadRotacion = 10f;
    // El objeto alrededor del cual girar
    public GameObject objetoCentro;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (objetoCentro != null)
        {
            // Rotar el objeto alrededor del objetoCentro en sentido contrario
            transform.RotateAround(objetoCentro.transform.position, Vector3.up, -velocidadRotacion * Time.deltaTime);
        }
    }
}
