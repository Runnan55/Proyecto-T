using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    private Dictionary<StatusEffect, Coroutine> effectCoroutines = new Dictionary<StatusEffect, Coroutine>();

    public void AddEffect(StatusEffect effect, GameObject target)
    {
        effect.ApplyEffect(target);
        if (!effect.IsPermanent)
        {
            if (effectCoroutines.ContainsKey(effect))
            {
                // Reiniciar la corutina si ya est� en ejecuci�n
                StopCoroutine(effectCoroutines[effect]);
                effectCoroutines[effect] = StartCoroutine(RemoveEffectAfterDelay(effect, target));
            }
            else
            {
                // Iniciar una nueva corutina si no est� en ejecuci�n
                var coroutine = StartCoroutine(RemoveEffectAfterDelay(effect, target));
                effectCoroutines.Add(effect, coroutine);
            }
        }
    }

    public void RemoveEffectImmediately(StatusEffect effect, GameObject target)
    {
        if (effectCoroutines.ContainsKey(effect))
        {
            StopCoroutine(effectCoroutines[effect]);
            effectCoroutines.Remove(effect);
        }
        effect.RemoveEffect(target);
    }
    public void RemoveEffectTriggers(StatusEffect effect, GameObject target)
    {
        //Metodo para remover en triggers
        if (effectCoroutines.ContainsKey(effect))
        {
            // Si la corutina est� activa, detenerla primero
            StopCoroutine(effectCoroutines[effect]);
            effectCoroutines.Remove(effect);
        }
        // Iniciar la corutina nuevamente
        Coroutine coroutine = StartCoroutine(RemoveEffectAfterDelay(effect, target));
        effectCoroutines[effect] = coroutine;
    }

    private IEnumerator RemoveEffectAfterDelay(StatusEffect effect, GameObject target)
    {
        Debug.Log("enumerator");
        yield return new WaitForSeconds(effect.Duration);
        effect.RemoveEffect(target);
        effectCoroutines.Remove(effect);
    }
}
