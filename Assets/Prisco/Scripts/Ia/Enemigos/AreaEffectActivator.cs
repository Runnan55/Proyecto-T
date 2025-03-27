using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffectActivator : MonoBehaviour
{
    public float areaRadius = 5f; 
    public float effectDuration = 0.3f; 
    public int damageAmount = 10; 

    void Start()
    {
        
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
