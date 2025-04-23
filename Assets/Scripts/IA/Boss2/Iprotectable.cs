using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProtectable
{
    void SetProtected(bool state);
    Transform GetTransform();
}