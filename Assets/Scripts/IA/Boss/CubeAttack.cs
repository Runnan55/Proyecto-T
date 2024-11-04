using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAttack : MonoBehaviour
{
    public float damageAmount = 20f; // Cantidad de daño que se aplica al jugador
    private Life playerLife;

    public GameObject dodgeChild;
    public GameObject damageChild;

    void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>(); // referencia vida player

        // Iniciar la corrutina para activar los hijos
        StartCoroutine(ActivateChildren());
    }

    private IEnumerator ActivateChildren()
    {
        // Activar el hijo de dodge
        if (dodgeChild != null)
        {
            dodgeChild.SetActive(true);
        }

        // Esperar 0.2 segundos
        yield return new WaitForSeconds(0.2f);

        // Desactivar el hijo de dodge y activar el hijo de damage
        if (dodgeChild != null)
        {
            dodgeChild.SetActive(false);
        }

        if (damageChild != null)
        {
            damageChild.SetActive(true);
        }

        // Esperar 0.1 segundos
        yield return new WaitForSeconds(0.1f);

        // Desactivar el hijo de damage
        if (damageChild != null)
        {
            damageChild.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el trigger colisionó con el jugador y el tag del objeto es "damage"
        if (other.CompareTag("Player") && CompareTag("damage"))
        {
            if (playerLife != null)
            {
                // Aplicar daño al jugador
                playerLife.ModifyTime(-damageAmount);
            }
        }
    }
}