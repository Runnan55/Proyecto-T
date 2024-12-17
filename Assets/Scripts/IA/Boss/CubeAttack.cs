using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAttack : MonoBehaviour
{
    public GameObject dodgeChild;
    public GameObject damageChild;
    public float dodgeTimer=0.25f;
    public float damageTimer=0.05f;

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

        yield return new WaitForSeconds(dodgeTimer);

        if (dodgeChild != null)
        {
            dodgeChild.SetActive(false);
            MovimientoJugador.isInDodgeArea = false;
        }

        if (damageChild != null)
        {
            damageChild.SetActive(true);
        }

        yield return new WaitForSeconds(damageTimer);

        if (damageChild != null)
        {
            damageChild.SetActive(false);
        }
    }
}