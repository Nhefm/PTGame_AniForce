using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pigeon : SingleAnimal
{
    // stats
    [SerializeField] private float maxMana;
    private float mana;

    // cooldown
    private bool canFlyHigh;
    [SerializeField] private float flyCooldown;

    // audio clip
    [SerializeField] private AudioClip flySound;

    // fly mode
    private bool isFlyingHigh;
    [SerializeField] private float distanceFromGround;
    [SerializeField] private float maxFlyHeight;
    [SerializeField] private float fallSpeed;
    
    // skill
    [SerializeField] private GameObject pigeons;
    private GameObject storedPigeons;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        storedPigeons = Instantiate(pigeons);
        storedPigeons.SetActive(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        mana = maxMana;
        canFlyHigh = true;
        isFlyingHigh = false;
    }

    protected override void Update()
    {
        base.Update();
        ManaChange();
        Fly();
    }

    public override void Move(InputAction.CallbackContext context)
    {
        base.Move(context);

        if(!isFlyingHigh)
        {
            return;
        }
        
        if(context.performed)
        {
            inputValue = context.ReadValue<Vector2>();
        }
    }

    public override void Jump(InputAction.CallbackContext context)
    {
        if(!canFlyHigh)
        {
            return;
        }

        if (state == State.Death)
        {
            return;
        }

        if(context.performed)
        {
            SwitchFlyMode();
        }
    }

    public void Fly()
    {
        if(state == State.Death)
        {
            return;
        }
        
        float min = hit.point.y + distanceFromGround;
        float y = transform.position.y;

        if(isFlyingHigh)
        {
            y = Mathf.Clamp(y, y, maxFlyHeight);
        }
        else if(transform.position.y != min)
        {
            float dy = min - y;
            y += Mathf.Sign(dy) * Mathf.Min(fallSpeed, Mathf.Abs(dy));
        }        

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public void ManaChange()
    {
        //Debug.Log(mana + "/" + maxMana);
        float deltaMana = Time.deltaTime;

        if (isFlyingHigh)
        {
            deltaMana = -Time.deltaTime;
        }

        mana = Mathf.Clamp(mana + deltaMana, 0, maxMana);
        
        if(mana == 0)
        {
            SwitchFlyMode();
            return;
        }
    }

    public void SwitchFlyMode()
    {
        isFlyingHigh = !isFlyingHigh;

        if(!isFlyingHigh)
        {
            StartCoroutine(FlyCooldown());
        }
        else
        {
            if(flySound)
            {
                audioSource.PlayOneShot(flySound);
            }
        }
    }

    public IEnumerator FlyCooldown()
    {
        canFlyHigh = false;
        yield return new WaitForSeconds(flyCooldown);
        canFlyHigh = true;
    }

    public override IEnumerator SkillTimer()
    {
        yield return null;

        float startPos = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        storedPigeons.SetActive(true);
        storedPigeons.transform.position = new Vector2(startPos, maxFlyHeight);
    }

    public override void Attack(InputAction.CallbackContext context)
    {
        if(!isFlyingHigh)
        {
            return;
        }

        base.Attack(context);
    }

    public override IEnumerator AttackTimer()
    {
        GameObject bullet = ObjectPooling.sharedInstance.GetObject();

        if(bullet)
        {
            bullet.SetActive(true);
            bullet.transform.position = transform.position;
        }
        
        return base.AttackTimer();
    }

    public void DealDamage() // add enemy
    {
        // deal atk damage
    }
}
