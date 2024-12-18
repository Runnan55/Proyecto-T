using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDamageZone : MonoBehaviour
{
    public GameObject smokeAttack;
    public float damageAmount = 10f;
    public Material activeMaterial;
    public Material inactiveMaterial;
    public bool isActive = true; // La zona de daño ahora tiene un valor estático para activar/desactivar

    private Life playerLife;
    private Renderer zoneRenderer;

    void Start()
    {
        zoneRenderer = GetComponent<Renderer>();
        StartCoroutine(FindPlayerAfterDelay(0.5f));
        UpdateMaterial();
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
        smokeAttack.SetActive(true);
    }

    public void DeactivateDamage()
    {
        isActive = false;
        UpdateMaterial();
        smokeAttack.SetActive(false);
    }

    private void UpdateMaterial()
    {
        if (zoneRenderer != null)
        {
            zoneRenderer.material = isActive ? activeMaterial : inactiveMaterial;
        }
    }
}
