using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : IState
{
    public override bool CompareState(string state)
    {
        return state == "Hurt";
    }

    public override void Update(PlayerController player)
    {
        player.Hurt();
    }
}

