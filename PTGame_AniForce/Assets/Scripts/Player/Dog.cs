using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : SingleAnimal
{
    [SerializeField] private Vector2 attackDashForce;

    public override IEnumerator AttackTimer()
    {
        rb.AddForce(new Vector2(attackDashForce.x * (int)direction, attackDashForce.y), ForceMode2D.Impulse);
        return base.AttackTimer();
    }

    public override IEnumerator SkillTimer()
    {
        state = State.Skill;
        animator.SetTrigger("Skill");
        
        yield return new WaitForSeconds(skillDuration);

        if(state == State.Skill)
        {
            state = State.Default;
        } 
    }

    override protected void OnCollisionEnter2D(Collision2D other) {

        if(state == State.Attack)
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
