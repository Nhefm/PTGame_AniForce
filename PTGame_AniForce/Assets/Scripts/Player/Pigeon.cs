using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float low;
    [SerializeField] private float high;
    [SerializeField] private float rangeCanAttack;
    private float counter;
    
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
        counter = 0;
        canFlyHigh = true;
        isFlyingHigh = false;
    }


    public override void Jump(KeyCode keyCode)
    {
        // Debug.Log(mana + "/" + maxMana);

        if (state == State.Death)
        {
            return;
        }

        float deltaCounter = -Time.deltaTime;
        float deltaMana = Time.deltaTime;

        if (isFlyingHigh)
        {
            deltaCounter = Time.deltaTime;
            deltaMana = -Time.deltaTime;
        }

        counter = Mathf.Clamp(counter + deltaCounter, 0, 1);
        mana = Mathf.Clamp(mana + deltaMana, 0, maxMana);
        
        if(mana == 0)
        {
            SwitchFlyMode();
            return;
        }

        float y = Mathf.Lerp(low, high, counter);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
        rb.velocity = new Vector2(rb.velocity.x, 0);

        if(!canFlyHigh)
        {
            return;
        }

        if(Input.GetKeyDown(keyCode))
        {
            SwitchFlyMode();
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
        storedPigeons.transform.position = new Vector2(startPos, high);
    }

    public override void Attack(KeyCode keyCode)
    {
        if(counter < Mathf.Lerp(0, 1, rangeCanAttack))
        {
            return;
        }

        base.Attack(keyCode);
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
