using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    public StatusEffect effectToApply;

    private void OnTriggerEnter(Collider other)
    {
        IEffectable effectable = other.gameObject.GetComponent<IEffectable>();
        if (effectable != null)
        {
            StatusManager manager = other.gameObject.GetComponent<StatusManager>();
            if (manager != null)
            {
                manager.AddEffect(effectToApply, other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IEffectable effectable = other.gameObject.GetComponent<IEffectable>();
        if (effectable != null)
        {
            StatusManager manager = other.gameObject.GetComponent<StatusManager>();
            if (manager != null)
            {
                manager.RemoveEffectTriggers(effectToApply, other.gameObject);
                Debug.Log("removiendo");
            }
        }
    }
}
