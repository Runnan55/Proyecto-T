using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParedeDestruyePro : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     // Este método se llama cuando otro collider toca el collider de este objeto
  void OnTriggerEnter(Collider other)
{
    // Verifica si el objeto que entró en el trigger tiene el tag "projectile"
    if (other.gameObject.CompareTag("Projectile"))
    {
        // Destruye el objeto que entró en el trigger
        Destroy(other.gameObject);
    }
}
}
