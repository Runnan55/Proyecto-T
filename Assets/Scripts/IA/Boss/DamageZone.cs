using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
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

    void Start()
    {
        zoneRenderer = GetComponent<Renderer>();
        StartCoroutine(FindPlayerAfterDelay(0.5f));
        UpdateMaterial();

        if (autoToggleMode)
        {
            StartCoroutine(AutoToggleRoutine());
        }
    }

    private IEnumerator FindPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActive && other.CompareTag("Player"))
        {
            if (playerLife != null)
            {
                playerLife.ModifyTime(-damageAmount);
            }
        }
    }

    public void ActivateDamage()
    {
        isActive = true;
        UpdateMaterial();
    }

    public void DeactivateDamage()
    {
        isActive = false;
        UpdateMaterial();
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