using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    public override bool CompareState(string state)
    {
        return state == "Attack";
    }

    public override void Update(PlayerController player)
    {
        player.Attack();
    }
}
