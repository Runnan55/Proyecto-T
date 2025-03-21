using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiroManecillasReloj : MonoBehaviour
{
    
    public float intervaloSegundos = 1f; 
    public Vector3 rotacionDeseada = new Vector3(0, 90, 0);
    public float velocidadRotacion = 1f; 
    public bool invertirRotacion = false; 

    private float tiempoAcumulado = 0f; 
    private bool estaRotando = false; 


    void Update()
    {
      
        if (!estaRotando)
        {
          
            tiempoAcumulado += Time.deltaTime;

           
            if (tiempoAcumulado >= intervaloSegundos)
            {
               
                StartCoroutine(RotarObjeto());
            }
        }
    }

   
    private IEnumerator RotarObjeto()
    {
        estaRotando = true;
        Vector3 rotacionInicial = transform.eulerAngles;

      
        Vector3 rotacionFinal = rotacionInicial + (invertirRotacion ? -rotacionDeseada : rotacionDeseada);

        float tiempo = 0f;

        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadRotacion;
            transform.eulerAngles = Vector3.Lerp(rotacionInicial, rotacionFinal, tiempo);
            yield return null;
        }

        transform.eulerAngles = rotacionFinal; 
        tiempoAcumulado = 0f; 
        estaRotando = false; 
    }
}
