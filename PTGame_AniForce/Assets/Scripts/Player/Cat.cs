using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : SingleAnimal
{
    [SerializeField] private Vector2 skillDashForce;
    [SerializeField] private Vector2 attackDashForce;

    public override IEnumerator AttackTimer()
    {
        rb.AddForce(new Vector2(attackDashForce.x * (int)direction, attackDashForce.y), ForceMode2D.Impulse);
        return base.AttackTimer();
    }

    public override IEnumerator SkillTimer()
    {
        state = State.Skill;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(skillDashForce.x * (int)direction, skillDashForce.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(skillDuration / 4);

        rb.velocity = Vector2.zero;
        Flip();
        yield return new WaitForSeconds(skillDuration / 4);

        if(skillSound)
        {
            audioSource.PlayOneShot(skillSound);
        }
        
        rb.AddForce(new Vector2(skillDashForce.x * (int)direction, skillDashForce.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(skillDuration / 4);
        
        rb.velocity = Vector2.zero;
        Flip();
        state = State.Default;
    }

    public override void ChangeHealth(float amount)
    {
        if(amount < 0 && state == State.Skill)
        {
            return;
        }

        base.ChangeHealth(amount);
    }

    protected void OnCollisionEnter2D(Collision2D other) {

        if(state == State.Attack || state == State.Skill)
        {
            // get opponent component   
            float dmgDeal = 0;

            if(state == State.Attack)
            {
                dmgDeal = atk;
            }
            else
            {
                dmgDeal = atk * skillAmp;
            }

            // oppenent change health(dmgDeal)
        }
    }
}
