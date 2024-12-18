using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    // stats
    [SerializeField] protected float atk;
    [SerializeField] protected float maxHP;
    protected float currentHP;
    [SerializeField] protected float speed;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float skillAmp;

    // durations
    protected bool isInvincible;
    [SerializeField] protected float invincibleDuration;
    [SerializeField] protected float attackDuration;
    [SerializeField] protected float skillDuration;

    // cooldown
    protected bool canAttack;
    [SerializeField] protected float attackCooldown;
    protected bool canSkill;

    // components
    protected Rigidbody2D rb;
    protected float horizontal;
    protected AudioSource audioSource;

    // audio clip
    [SerializeField] protected AudioClip hurtSound;
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioClip skillSound;

    // enums
    protected State state;
    protected Direction direction;

    public enum Direction {Left = -1, Right = 1}
    public enum State {Default, Jump, Attack, Skill, Death}

    // Start is called before the first frame update
    virtual protected void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        direction = Direction.Right;
        transform.localScale = new Vector2(1, transform.localScale.y);
        state = State.Default;
        canAttack = true;
        canSkill = true;
        isInvincible = false;
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        // Debug.Log(state);
        horizontal = Input.GetAxis("Horizontal");
        Move();
        Jump(KeyCode.Space);
        Attack(KeyCode.Mouse0);
        Skill(KeyCode.E);
    }

    virtual public void ChangeHealth(float amount)
    {
        if(amount < 0)
        {
            if(isInvincible)
            {
                return;
            }

            if(hurtSound)
            {
                audioSource.PlayOneShot(hurtSound);
            }
            
            StartCoroutine(InvincibleTimer());
        }
        
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        //UIHealthBar.instance.SetValue((float)currentHealth / maxHealth);
    }

    abstract public IEnumerator InvincibleTimer();

    virtual public void Move()
    {
        if(state != State.Default && state != State.Jump)
        {
            return;
        }

        transform.Translate(speed * Time.deltaTime * horizontal * Vector2.right);

        if(horizontal * (int)direction < 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        direction = (Direction)(-(int)direction);
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    virtual public void Jump(KeyCode keyCode)
    {
        if(!Input.GetKeyDown(keyCode))
        {
            return;
        }

        if(state != State.Default)
        {
            return;
        }

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        state = State.Jump;
    }
    virtual public void Attack(KeyCode keyCode)
    {
        if(!Input.GetKeyDown(keyCode))
        {
            return;
        }

        if(state != State.Default)
        {
            return;
        }

        if(!canAttack)
        {
            return;
        }

        StartCoroutine(AttackTimer());
        StartCoroutine(AttackCooldown());
    }

    virtual public IEnumerator AttackTimer()
    {
        state = State.Attack;

        if(attackSound)
        {
            audioSource.PlayOneShot(attackSound);
        }

        yield return new WaitForSeconds(attackDuration);

        if(state == State.Attack)
        {
            state = State.Default;
        } 
    }

    virtual public IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    virtual public void Skill(KeyCode keyCode)
    {
        if(!Input.GetKeyDown(keyCode))
        {
            return;
        }

        if(state != State.Default)
        {
            return;
        }

        if(!canSkill)
        {
            return;
        }

        if(skillSound)
        {
            audioSource.PlayOneShot(skillSound);
        }
        
        StartCoroutine(SkillTimer());
        StartCoroutine(SkillCooldown());
    }

    abstract public IEnumerator SkillTimer();

    virtual public IEnumerator SkillCooldown()
    {
        canSkill = false;
        yield return null;
        // yield return new WaitForSeconds(skillCooldown);
        canSkill = true;
    }

    virtual protected void OnCollisionEnter2D(Collision2D other) {

        if(state == State.Jump & other.collider.CompareTag("Ground"))
        {
            state = State.Default;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    public float getCurrentHealthPercentage()
    {
        if(maxHP == 0) return 1;
        
        return currentHP / maxHP;
    }

    virtual public void TakeControl(Vector3 currentPosition)
    {
        if(state != State.Default)
        {
            return;
        }

        transform.position = currentPosition;

        // reset stats
        currentHP = maxHP;
        canAttack = true;
        canSkill = true;
        isInvincible = false;
        state = State.Default;
        
        if(direction != Direction.Right)
        {
            Flip();
        }
    }

}
