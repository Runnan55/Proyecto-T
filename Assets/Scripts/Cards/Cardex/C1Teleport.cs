using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class C1Teleport : BaseCard, ICard
{
    public C1Teleport()
    {
        Name = "Teleport";
    }

    void Awake()
    {
        Image = Resources.Load<Sprite>("Sprites/SpriteTeleport.png");
    }

    public override void Effect()
    {
        Debug.Log("Teleport effect");
    }
}
