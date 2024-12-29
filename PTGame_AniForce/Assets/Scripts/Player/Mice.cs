using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mice : PlayerController
{
    // stats
    private float maxAtk;

    // mice component
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int amountToPool;
    [SerializeField] private float distanceScale;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        GameObject temp;
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

    protected override void OnEnable()
    {
        base.OnEnable();
        ResizeCollider(amountToPool);
        atk = maxAtk;
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        for(int i = transform.childCount - amountToPool; i < transform.childCount; i++)
        {
            Animator childAnimator = transform.GetChild(i).GetComponent<Animator>();
            childAnimator.SetFloat("speed", Mathf.Abs(inputValue.x));
            childAnimator.SetFloat("yVelocity", rb.linearVelocity.y);
        }

        PassiveAttack();
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
            transform.GetChild(transform.childCount - 1).GetComponent<Animator>().SetBool("isDeath", true);
        }
        else
        {
            yield return new WaitForSeconds(invincibleDuration);
            isInvincible = false;
        }

        ResizeCollider((int)Mathf.Ceil(GetCurrentHealthPercentage() * amountToPool));
        yield return null;
    }

    public override void OnAttack(InputAction.CallbackContext context)
    {

    }

    public void PassiveAttack()
    {
        if(!canAttack)
        {
            return;
        }

        if(currentHP == 0)
        {
            return;
        }

        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Poison");
        StartCoroutine(AttackCooldown());
    }

    public override IEnumerator SkillTimer()
    {
        transform.GetChild(1).GetComponent<Animator>().SetTrigger("Heal");
        currentHP = Mathf.Clamp(currentHP + maxHP / amountToPool, 0, maxHP);

        yield return new WaitForSeconds(skillDuration);
        ResizeCollider((int)Mathf.Ceil(GetCurrentHealthPercentage() * amountToPool));

        if(!state.CompareState("Hurt"))
        {
            state = StateTransition();
        }
    }

    public void ResizeCollider(int mouseNumber)
    {
        if(mouseNumber == 0)
        {
            mouseNumber = 1;
        }
        
        if(transform.childCount < mouseNumber)
        {
            return;
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
        bc.size = new Vector2(mouseSize.x + (mouseNumber - 1) * distanceScale, mouseSize.y);

        atk = maxAtk * mouseNumber / amountToPool;
    }

    public override void ChangeHealth(float amount)
    {
        if(amount < 0)// && state == State.Skill)
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
