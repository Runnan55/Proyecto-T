using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    string Name { get; }
    Sprite Image { get; }
    void Effect();
}

