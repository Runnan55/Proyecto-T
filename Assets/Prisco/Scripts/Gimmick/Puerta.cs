using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Puerta : MonoBehaviour
{
    // Referencias a los tres objetos
    public GameObject objeto1;
    public GameObject objeto2;
    public GameObject objeto3;

    // Variables booleanas en los otros objetos
    private bool bool1;
    private bool bool2;
    private bool bool3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Obtener las variables booleanas de los otros objetos
        bool1 = objeto1.GetComponent<ObjetoActivable>().Activado;
        bool2 = objeto2.GetComponent<ObjetoActivable>().Activado;
        bool3 = objeto3.GetComponent<ObjetoActivable>().Activado;

        // Comprobar si todas las variables booleanas están en true
        if (bool1 && bool2 && bool3)
        {
            // Destruir el objeto que lleva este script
            Destroy(gameObject);
        }
    }
}