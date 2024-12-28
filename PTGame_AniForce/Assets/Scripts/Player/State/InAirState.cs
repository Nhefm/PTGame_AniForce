using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAirState : IState
{
    public override bool CompareState(string state)
    {
        return state == "In Air";
    }

    public override void Update(PlayerController player)
    {
        player.MoveInAir();
    }
}
