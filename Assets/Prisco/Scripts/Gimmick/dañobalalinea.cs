using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dañobalalinea : MonoBehaviour
{
    
     public float damage = 5f;

    private Life playerLife;
    void Start()
    {        
        StartCoroutine(AssignPlayerLife());
    }
     private IEnumerator AssignPlayerLife()
    {
        // Esperar 0.5 segundos
        yield return new WaitForSeconds(0.5f);

        // Asignar la referencia a playerLife
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerLife != null)
            {
                playerLife.ModifyTime(-damage);
                MovimientoJugador.isInDodgeArea = false;
                Debug.Log("buenas tardes " + MovimientoJugador.isInDodgeArea);
            }
            else
            {
                Debug.LogError("playerLife is not initialized.");
            }

            
        }
       
    }
}
