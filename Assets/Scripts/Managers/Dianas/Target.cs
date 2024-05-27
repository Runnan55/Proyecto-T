using UnityEngine;

public class Target : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            // Llama al método del manager para incrementar el contador
            TargetManager.Instance.IncrementTargetCount();
            // Destruye la diana
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
