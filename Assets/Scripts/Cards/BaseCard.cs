using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCard : MonoBehaviour, ICard
{
    public string Name { get; protected set; }
    public Sprite Image { get; protected set; }
    public abstract void Effect();
}
