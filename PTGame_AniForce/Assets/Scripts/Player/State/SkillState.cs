using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : IState
{
    public override bool CompareState(string state)
    {
        return state == "Skill";
    }

    public override void Update(PlayerController player)
    {
        player.Skill();
    }
}
