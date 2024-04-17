using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{

    public float delay = 1f;

    void Start()
    {

        // Invoca la función para destruir el GameObject después del retraso especificado
        Invoke("DestroyObject", delay);
    }

    void DestroyObject()
    {
        // Destruye el GameObject
        Destroy(this.gameObject);
    }
}