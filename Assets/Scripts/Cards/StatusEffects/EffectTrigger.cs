using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    public StatusEffect effectToApply;  // Asigna este efecto en el Inspector de Unity

    private void OnTriggerEnter(Collider other)
    {
        IEffectable effectable = other.gameObject.GetComponent<IEffectable>();
        if (effectable != null)
        {
            // Aqu� asumimos que el objeto tiene un StatusEffectManager, como parte de su implementaci�n de IEffectable
            StatusManager manager = other.gameObject.GetComponent<StatusManager>();
            if (manager != null)
            {
                manager.AddEffect(effectToApply, other.gameObject);
            }
        }
    }
}
