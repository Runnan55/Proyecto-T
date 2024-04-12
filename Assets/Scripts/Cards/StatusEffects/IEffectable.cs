using UnityEngine;

public interface IEffectable
{
    void ApplyEffect(StatusEffect effect);
    void RemoveEffect(StatusEffect effect);
}
