using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleAnimal : PlayerController
{
    // components
    protected Animator animator;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        animator.SetFloat("speed", Mathf.Abs(horizontal));
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    public override IEnumerator InvincibleTimer()
    {
        animator.SetTrigger("Hurt");
        state = State.Hurt;

        if(currentHP == 0)
        {
            animator.SetBool("isDeath", true);
            //rb.simulated = false;
            state = State.Death;
        }
        else
        {
            yield return new WaitForSeconds(invincibleDuration);
            state = State.Default;
        }

       yield return null;
    }
}
