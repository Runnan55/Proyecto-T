using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffectActivator : MonoBehaviour
{
    public float areaRadius = 5f; 
    public float effectDuration = 0.3f; 
    public int damageAmount = 10; 
    public GameObject areaIndicatorPrefab; // Prefab for the area indicator
    public float activationDelay = 1f; // Time delay before activation

    void Start()
    {
        StartCoroutine(ActivateEffectAfterDelay());
    }

    private IEnumerator ActivateEffectAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay); // Wait for the specified delay

        // Instantiate the area indicator
        if (areaIndicatorPrefab != null)
        {
            GameObject indicator = Instantiate(areaIndicatorPrefab, transform.position, Quaternion.identity);
            float diameter = areaRadius * 2; // Calculate the diameter of the area
            indicator.transform.localScale = new Vector3(diameter, indicator.transform.localScale.y, diameter); // Adjust scale to match the area
            Destroy(indicator, effectDuration); // Destroy the indicator after the effect duration
        }

        foreach (Collider collider in Physics.OverlapSphere(transform.position, areaRadius))
        {
            if (collider.CompareTag("Enemy"))
            {
                EnemyLife enemy = collider.GetComponent<EnemyLife>();
                if (enemy != null)
                {
                    if (!enemy.antiRevivir) 
                    {
                        enemy.antiRevivir = true; 
                        enemy.ReceiveDamage(damageAmount); 
                    }
                }
            }
        }

        Destroy(gameObject, effectDuration);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }
}
