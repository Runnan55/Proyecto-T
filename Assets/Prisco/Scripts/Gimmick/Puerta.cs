using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Puerta : MonoBehaviour
{
    // Lista de objetos activables
    public List<GameObject> objetosActivables;

    // Número de objetos que deben estar activados para abrir la puerta
    public int numeroNecesarioActivados;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int contadorActivados = 0;

        // Contar cuántos objetos están activados
        foreach (GameObject objeto in objetosActivables)
        {
            if (objeto.GetComponent<ObjetoActivable>().Activado)
            {
                contadorActivados++;
            }
        }

        // Comprobar si el número de objetos activados es suficiente
        if (contadorActivados >= numeroNecesarioActivados)
        {
            // Destruir el objeto que lleva este script
            Destroy(gameObject);
        }
    }
}