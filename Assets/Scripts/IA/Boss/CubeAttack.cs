using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAttack : MonoBehaviour
{
    public GameObject dodgeChild;
    public GameObject damageChild;

    void Start()
    {
        StartCoroutine(ActivateChildren());
    }

    private void OnTriggerExit(Collider other)
    {
        MovimientoJugador.isInDodgeArea = false;
    }

    private IEnumerator ActivateChildren()
    {
        if (dodgeChild != null)
        {
            dodgeChild.SetActive(true);
        }

        yield return new WaitForSeconds(0.25f);

        if (dodgeChild != null)
        {
            dodgeChild.SetActive(false);
            MovimientoJugador.isInDodgeArea = false;
        }

        if (damageChild != null)
        {
            damageChild.SetActive(true);
        }

        yield return new WaitForSeconds(0.05f);

        if (damageChild != null)
        {
            damageChild.SetActive(false);
        }
    }
}