using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultState : IState
{
    public override bool CompareState(string state)
    {
        return state == "Default";
    }

    public override void Update(PlayerController player)
    {
        player.Move();
    }
}
