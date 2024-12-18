using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mice : PlayerController
{
    // stats
    private float maxAtk;

    // mice component
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int amountToPool;
    [SerializeField] private float distanceScale;

    // component
    private BoxCollider2D collider2d;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        GameObject temp;
        collider2d = GetComponent<BoxCollider2D>();
        int order = 0;

        for(int i = amountToPool; i > 0; i--)
        {
            float position;

            if (i % 2 == 0)
            {
                position = -1;
            }
            else
            {
                position = 1;
                ++order;
            }

            temp = Instantiate(objectToPool);
            temp.transform.localPosition = new Vector2( i / 2 * position * distanceScale + transform.position.x,  transform.position.y);
            temp.GetComponent<SpriteRenderer>().sortingOrder = order;
            temp.transform.parent = transform;
        }

        transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = ++order;
        maxAtk = atk;
        ResizeCollider(amountToPool);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        for(int i = transform.childCount - amountToPool; i < transform.childCount; i++)
        {
            Animator childAnimator = transform.GetChild(i).GetComponent<Animator>();
            childAnimator.SetFloat("speed", Mathf.Abs(horizontal));
            childAnimator.SetFloat("yVelocity", rb.velocity.y);
        }
    }

    public override IEnumerator InvincibleTimer()
    {
        for(int i = transform.childCount - amountToPool; i < transform.childCount; i++)
        {
            Animator childAnimator = transform.GetChild(i).GetComponent<Animator>();
            childAnimator.SetTrigger("Hurt");
        }

        isInvincible = true;

        if(currentHP == 0)
        {
            rb.simulated = false;
            state = State.Death;
            transform.GetChild(transform.childCount - 1).GetComponent<Animator>().SetBool("isDeath", true);
        }
        else
        {
            yield return new WaitForSeconds(invincibleDuration);
            isInvincible = false;
        }

        ResizeCollider((int)Mathf.Ceil(getCurrentHealthPercentage() * amountToPool));
        yield return null;
    }

    public override void Attack(KeyCode keyCode)
    {
        if(!canAttack)
        {
            return;
        }

        if(state != State.Default)
        {
            return;
        }

        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Poison");
        StartCoroutine(AttackCooldown());
    }

    public override IEnumerator SkillTimer()
    {
        state = State.Skill;
        transform.GetChild(1).GetComponent<Animator>().SetTrigger("Heal");
        currentHP = Mathf.Clamp(currentHP + maxHP / amountToPool, 0, maxHP);

        yield return new WaitForSeconds(skillDuration);
        ResizeCollider((int)Mathf.Ceil(getCurrentHealthPercentage() * amountToPool));
        state = State.Default;
    }

    public void ResizeCollider(int mouseNumber)
    {
        if(mouseNumber == 0)
        {
            mouseNumber = 1;
        }
        
        for(int i = transform.childCount - amountToPool; i < transform.childCount - mouseNumber; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for(int i = transform.childCount - mouseNumber; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        Vector2 mouseSize = objectToPool.GetComponent<SpriteRenderer>().bounds.size;
        collider2d.size = new Vector2(mouseSize.x + (mouseNumber - 1) * distanceScale, mouseSize.y);

        atk = maxAtk * mouseNumber / amountToPool;
    }

    public override void ChangeHealth(float amount)
    {
        if(amount < 0 && state == State.Skill)
        {
            return;
        }

        base.ChangeHealth(amount);
    }

    public void DealAttackDamage() // add enemy
    {
        // deal enemy atk
    }
}
