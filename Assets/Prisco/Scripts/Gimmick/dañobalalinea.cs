using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dañobalalinea : MonoBehaviour
{
    public float damage = 5f;

      [SerializeField] private MovimientoJugador movimientoJugador;
      [SerializeField] private Life playerLife;

    

    void Start()
    {
        StartCoroutine(FindPlayerWithDelay());
    }

    private IEnumerator FindPlayerWithDelay()
    {
        yield return new WaitForSeconds(1f); // Ajusta el delay según sea necesario
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLife = player.GetComponent<Life>();
            movimientoJugador = player.GetComponent<MovimientoJugador>();
        }
        else
        {
            
        }
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
                //Debug.Log("buenas tardes " + MovimientoJugador.isInDodgeArea);
            }
            else
            {
               
            }
        }
    }
}
