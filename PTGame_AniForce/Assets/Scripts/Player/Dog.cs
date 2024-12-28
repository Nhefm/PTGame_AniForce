using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dog : SingleAnimal
{
    [SerializeField] private Vector2 attackDashForce;

    public override IEnumerator AttackTimer()
    {
        rb.AddForce(new Vector2(attackDashForce.x * direction, attackDashForce.y), ForceMode2D.Impulse);
        return base.AttackTimer();
    }

    public override IEnumerator SkillTimer()
    {
        animator.SetTrigger("Skill");
        yield return new WaitForSeconds(skillDuration);

        if(state.CompareState("Skill"))
        {
            state = StateTransition();
        }
    }

    override protected void OnCollisionEnter2D(Collision2D other) {

        //if(state == State.Attack)
        {
            // get opponent component
            // oppenent change health(atk)
        }

        base.OnCollisionEnter2D(other);
    }

    public void DealSkillDamage() // add enemy
    {
        // deal enemy atk * skillAmp
    }
}
