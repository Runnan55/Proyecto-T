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

    private IEnumerator ActivateChildren()
    {
        if (dodgeChild != null)
        {
            dodgeChild.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);

        if (dodgeChild != null)
        {
            dodgeChild.SetActive(false);
        }

        if (damageChild != null)
        {
            damageChild.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);

        if (damageChild != null)
        {
            damageChild.SetActive(false);
        }
    }
}