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

    private void OnTriggerStay2D(Collider2D other) {
        var enemy = other.transform.GetComponent<Enemy>();

        if(enemy && state.CompareState("Attack"))
        {
            enemy.TakeDamage(atk);
        }   
    }

    public void DealSkillDamage(Enemy enemy)
    {
        enemy.TakeDamage(atk * skillAmp);
    }
}
