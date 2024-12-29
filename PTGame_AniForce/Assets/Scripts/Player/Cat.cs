using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : SingleAnimal
{
    [SerializeField] private Vector2 skillDashForce;
    [SerializeField] private Vector2 attackDashForce;

    public override IEnumerator AttackTimer()
    {
        rb.AddForce(new Vector2(attackDashForce.x * direction, attackDashForce.y), ForceMode2D.Impulse);
        return base.AttackTimer();
    }

    public override IEnumerator SkillTimer()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(skillDashForce.x * direction, skillDashForce.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(skillDuration / 4);

        rb.linearVelocity = Vector2.zero;
        Flip();
        yield return new WaitForSeconds(skillDuration / 4);

        if(skillSound)
        {
            audioSource.PlayOneShot(skillSound);
        }
        
        rb.AddForce(new Vector2(skillDashForce.x * direction, skillDashForce.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(skillDuration / 4);
        
        rb.linearVelocity = Vector2.zero;
        Flip();

        if(state.CompareState("Skill"))
        {
            state = StateTransition();
        }
    }

    public override void ChangeHealth(float amount)
    {
        if(amount < 0 && state.CompareState("Skill"))
        {
            return;
        }

        base.ChangeHealth(amount);
    }

    override protected void OnCollisionEnter2D(Collision2D other) {

        //if(state == State.Attack || state == State.Skill)
        {
            // get opponent component   
            float dmgDeal = 0;

            //if(state == State.Attack)
            {
                dmgDeal = atk;
            }
            //else
            {
                dmgDeal = atk * skillAmp;
            }

            // oppenent change health(dmgDeal)
        }

        base.OnCollisionEnter2D(other);
    }
}
