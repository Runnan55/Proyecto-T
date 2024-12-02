using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoTrampaLinea : MonoBehaviour
{
    public GameObject puntoA;
    public GameObject puntoB;
    public float velocidad = 1.0f;

    private bool moviendoHaciaA = true;


    // Start is called before the first frame update
    void Start()
    {
          
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moviendoHaciaA)
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoA.transform.position, velocidad * Time.deltaTime);
            if (transform.position == puntoA.transform.position)
            {
                moviendoHaciaA = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoB.transform.position, velocidad * Time.deltaTime);
            if (transform.position == puntoB.transform.position)
            {
                moviendoHaciaA = true;
            }
        }
    }

  
}