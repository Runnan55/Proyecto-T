using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    public string Name;
    public float Duration;
    public bool IsPermanent;

    public abstract void ApplyEffect(GameObject target);
    public abstract void RemoveEffect(GameObject target);

    protected StatusEffect(string name, float duration, bool isPermanent)
    {
        Name = name;
        Duration = duration;
        IsPermanent = isPermanent;
    }
}
