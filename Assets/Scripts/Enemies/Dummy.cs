using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    void Start()
    {
        damage = 0f;
        health = 999999f;
    }

    public override void AttackPlayer()
    {
        // no pegar
    }
}