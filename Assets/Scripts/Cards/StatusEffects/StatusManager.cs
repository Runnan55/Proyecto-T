using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    private List<StatusEffect> activeEffects = new List<StatusEffect>();

    public void AddEffect(StatusEffect effect, GameObject target)
    {
        effect.ApplyEffect(target);
        if (!effect.IsPermanent)
        {
            StartCoroutine(RemoveEffectAfterDelay(effect, target));
        }
        activeEffects.Add(effect);
    }

    private IEnumerator RemoveEffectAfterDelay(StatusEffect effect, GameObject target)
    {
        yield return new WaitForSeconds(effect.Duration);
        effect.RemoveEffect(target);
        activeEffects.Remove(effect);
    }
}
