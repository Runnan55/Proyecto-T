using UnityEngine;

public class SpeedBuff : StatusEffect
{
    private float speedIncrease = 0.3f;  // Incremento de 30% en la velocidad

    public SpeedBuff(string name, float duration, bool isPermanent)
        : base(name, duration, isPermanent)
    {
    }

    public override void ApplyEffect(GameObject target)
    {
        if (target.TryGetComponent(out PlayerMovement playerMovement))
        {
            if (!playerMovement.speedbuff)  // Asegúrate de no aplicar el efecto múltiples veces
            {
                speedIncrease = 0.5f;  // Incremento de 30% en la velocidad
                Debug.Log("Speed Aplicado a " + speedIncrease);
                playerMovement.BuffSpeed += speedIncrease;  // Aumenta el BuffSpeed
                playerMovement.speedbuff = true;  // Marca el estado como activo
                Debug.Log("Speed Aplicado a " + target);
                playerMovement.SpeedChange();  // Actualiza la velocidad del jugador
                target.GetComponent<StatusManager>()?.AddEffect(this, target);  // Añade el efecto para su manejo en StatusManager
            }
        }
    }

    public override void RemoveEffect(GameObject target)
    {
        if (target.TryGetComponent(out PlayerMovement playerMovement))
        {
            if (playerMovement.speedbuff)
            {
                speedIncrease = 0.5f;  // Incremento de 30% en la velocidad
                playerMovement.BuffSpeed -= speedIncrease;  // Revierte el BuffSpeed al estado original
                playerMovement.speedbuff = false;
                playerMovement.SpeedChange();  // Actualiza la velocidad del jugador
                Debug.Log("Speed terminado a " + target);
            }
        }
    }
}
