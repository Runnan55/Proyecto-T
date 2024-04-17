using UnityEngine;

public class EffectActivator : MonoBehaviour
{
    public StatusEffect effectToApply;
    private StatusManager manager;
    private GameObject target;

    // Este m�todo podr�a ser llamado, por ejemplo, al interactuar con un objeto.
    public void ActivateEffect()
    {
        if (target == null)
        {
        }

        if (manager == null)
        {
            manager = target.GetComponent<StatusManager>();
        }

        if (manager != null)
        {
            manager.AddEffect(effectToApply, target);
        }
    }

    // Este m�todo podr�a ser llamado para desactivar el efecto manualmente.
    public void DeactivateEffect()
    {
        if (manager != null)
        {
            manager.RemoveEffectImmediately(effectToApply, target);
        }
    }
}
