using UnityEngine;

public class SpeedBuff : StatusEffect
{
    private float speedIncrease = 50;


    public SpeedBuff(string name, float duration, bool isPermanent, float speedIncrease,bool speedit)
        : base(name, duration, isPermanent)

    {
        this.speedIncrease = 50;
    }

    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent(out PlayerMovement controller))
        {
            controller.speed += 50;
            Debug.Log("Speed Aplicado a " + target);
        }
        //Falta para enemigos.
    }

    public override void RemoveEffect(GameObject target)
    {
        if (target.TryGetComponent(out PlayerMovement controller))
        {
            controller.speed -= 50;
            Debug.Log("Speed terminado a " + target);
        }
        //Falta para enemigos.
    }
}
