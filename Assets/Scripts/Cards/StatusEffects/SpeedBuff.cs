using UnityEngine;

public class SpeedBuff : StatusEffect
{
    private float speedIncrease = 50;


    public SpeedBuff(string name, float duration, bool isPermanent, float speedIncrease,bool speedit)
        : base(name, duration, isPermanent)

    {
        this.speedIncrease = 50; this.IsPermanent = false;
    }

    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent(out PlayerMovement controller))
        {
            if (controller.speedbuff == false)
            {
                controller.speed += 50;
                Debug.Log("Speed Aplicado a " + target);
                controller.speedbuff = true;
            }
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        if (target.TryGetComponent(out PlayerMovement controller))
        {
            if (controller.speedbuff == true)
            controller.speed -= 50;
            Debug.Log("Speed terminado a " + target);
            controller.speedbuff = false;
        }
    }
}
