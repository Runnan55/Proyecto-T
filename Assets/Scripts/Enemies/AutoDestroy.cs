using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Tiempo en segundos que el objeto existirá antes de destruirse.
    public float destructionTime = 5f;

    // Método que se llama cuando el objeto es instanciado
    void Start()
    {
        // Inicia la destrucción del objeto después del tiempo definido
        Destroy(gameObject, destructionTime);
    }
}