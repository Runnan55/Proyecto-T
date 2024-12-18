using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public GameObject smokeAttack;
    public GameObject smokeCD;
    public float damageAmount = 10f;
    public Material activeMaterial;
    public Material preActiveMaterial;
    public Material inactiveMaterial;
    public bool autoToggleMode = false;
    public float activeDuration = 5f;
    public float inactiveDuration = 5f;

    private Life playerLife;
    private bool isActive = true;
    private Renderer zoneRenderer;

    void Awake()
    {
        zoneRenderer = GetComponent<Renderer>();
        UpdateMaterial();

        if (autoToggleMode)
        {
            StartCoroutine(AutoToggleRoutine());
        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (isActive && other.CompareTag("Player"))
        {
            playerLife = other.GetComponent<Life>();
            playerLife.ModifyTime(-damageAmount);
            //Debug.Log("danoplayer");
            
        }
    }

    public void ActivateDamage()
    {
        isActive = true;
        UpdateMaterial();
        if (smokeAttack != null && smokeCD != null)
        {
        smokeAttack.SetActive(true);
        smokeCD.SetActive(false);
        }
    }

    public void DeactivateDamage()
    {
        isActive = false;
        UpdateMaterial();
        if (smokeAttack != null && smokeCD != null)
        {
        smokeCD.SetActive(true);
        smokeAttack.SetActive(false);
        }

    }

    private void UpdateMaterial()
    {
        if (zoneRenderer != null)
        {
            zoneRenderer.material = isActive ? activeMaterial : inactiveMaterial;
        }
    }

    private IEnumerator AutoToggleRoutine()
    {
        while (true)
        {
            StartCoroutine(PreActivateRoutine());
            yield return new WaitForSeconds(activeDuration + inactiveDuration);
        }
    }

    private IEnumerator PreActivateRoutine()
    {
        yield return new WaitForSeconds(inactiveDuration - 1f);
        zoneRenderer.material = preActiveMaterial;
        yield return new WaitForSeconds(1f);
        ActivateDamage();
        yield return new WaitForSeconds(activeDuration);
        DeactivateDamage();
    }
}