using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
    public abstract bool CompareState(string state);
    public abstract void Update(PlayerController player);
}
