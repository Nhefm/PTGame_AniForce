using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
    // stats
    [SerializeField] protected float atk;
    [SerializeField] protected float maxHP;
    protected float currentHP;
    [SerializeField] protected float speed;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float skillAmp;

    // direction
    protected float direction;

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
    protected PlayerInputAction inputActions;
    protected Vector2 inputValue;
    protected AudioSource audioSource;
    protected BoxCollider2D bc;

    // audio clip
    [SerializeField] protected AudioClip hurtSound;
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioClip skillSound;

    // state
    protected IState state;

    // slope
    [SerializeField] protected float slopeCheckDistance;
    protected RaycastHit2D hit;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        bc = GetComponent<BoxCollider2D>();

        inputValue = Vector2.zero;
        inputActions = new PlayerInputAction();
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovement;
        inputActions.Player.Move.canceled += OnMovement;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Skill.performed += OnSkill;
    }
    
    virtual protected void Update()
    {
        direction = transform.localScale.x;
        SlopeHandler();
        PreventSliding();
    }

    protected void FixedUpdate()
    {
        state.Update(this);
    }

    virtual protected void OnEnable()
    {
        currentHP = maxHP;
        state = new InAirState();
        canAttack = true;
        canSkill = true;
        isInvincible = false;

        inputActions?.Player.Enable();
    }

    virtual protected void OnDisable() {

        inputActions?.Player.Disable();
    }

    virtual public void OnMovement(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            inputValue = context.ReadValue<Vector2>();
        }

        if(context.canceled)
        {
            inputValue = Vector2.zero;
        }
    }

    virtual public void OnJump(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }

        if(!state.CompareState("Default"))
        {
            return;
        }

        state = new InAirState();
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    virtual public void OnAttack(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }

        if(!state.CompareState("Default"))
        {
            return;
        }

        if(!canAttack)
        {
            return;
        }

        state = new AttackState();
    }

    virtual public void OnSkill(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }

        if(!state.CompareState("Default"))
        {
            return;
        }

        if(!canSkill)
        {
            return;
        }

        state = new SkillState();
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
        }

        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);

        if(amount < 0)
        {
            state = new HurtState();
        }
        
        //UIHealthBar.instance.SetValue((float)currentHealth / maxHealth);
    }

    public void Hurt()
    {
        StartCoroutine(InvincibleTimer());
    }

    abstract public IEnumerator InvincibleTimer();

    virtual public void Move()
    {
        Debug.Log(inputValue.x);
        
        if(hit.normal.normalized != Vector2.up)
        {
            rb.linearVelocity = speed * inputValue.x * -Vector2.Perpendicular(hit.normal.normalized);
        }
        else
        {
            rb.linearVelocity = new Vector2(speed * inputValue.x, 0);
        }

        if(inputValue.x * direction < 0)
        {
            Flip();
        }
    }

    virtual public void MoveInAir()
    {
        rb.linearVelocity = new Vector2(speed * inputValue.x, rb.linearVelocity.y);

        if(inputValue.x * direction < 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        transform.localScale = new Vector2(direction * -1, transform.localScale.y);
    }

    public void Attack()
    {
        if(!canAttack)
        {
            return;
        }

        StartCoroutine(AttackTimer());
        StartCoroutine(AttackCooldown());
    }

    virtual public IEnumerator AttackTimer()
    {
        if(attackSound)
        {
            audioSource.PlayOneShot(attackSound);
        }

        yield return new WaitForSeconds(attackDuration);

        if(state.CompareState("Attack"))
        {
            state = StateTransition();
        }
    }

    virtual public IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void Skill()
    {
        if(!canSkill)
        {
            return;
        }

        if (skillSound && !audioSource.isPlaying)
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
        yield return new WaitForSeconds(skillDuration);
        canSkill = true;
    }

    virtual protected void OnCollisionEnter2D(Collision2D other)
    {
        if(!other.collider.CompareTag("Ground"))
        {
            return;
        }

        foreach(var contact in other.contacts)
        {
            if(Mathf.Abs(contact.normal.x) == 1)
            {
                return;
            }
        }

        if(state.CompareState("In Air"))
        {
            state = new DefaultState();
        }
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        if(!other.collider.CompareTag("Ground"))
        {
            return;
        }

        if(currentHP == 0)
        {
            rb.simulated = false;
        }
    }

    protected void OnCollisionExit2D(Collision2D other)
    {
        if(!other.collider.CompareTag("Ground"))
        {
            return;
        }

        if(!state.CompareState("Default"))
        {
            return;
        }
        
        state = new InAirState();
    }

    virtual public void SlopeHandler()
    {
        List<Vector2> listOrigins = new(3)
        {
            bc.bounds.center,
            new Vector2(bc.bounds.min.x, bc.bounds.center.y),
            new Vector2(bc.bounds.max.x, bc.bounds.center.y)
        };
        
        RaycastHit2D temp = Physics2D.Raycast(listOrigins[0], Vector2.down, slopeCheckDistance, LayerMask.GetMask("Ground"));

        if(temp)
        {
            hit = temp;
        }

        foreach(var origin in listOrigins)
        {
            temp = Physics2D.Raycast(origin, Vector2.down, slopeCheckDistance, LayerMask.GetMask("Ground"));

            if(temp && temp.normal != Vector2.up)
            {
                hit = temp;
            }
        }
    }

    public IState StateTransition()
    {
        IState transitionState = new InAirState();

        float distance = Vector2.Distance(hit.point, new Vector2(bc.bounds.max.x, bc.bounds.min.y));
        float boundMinY = bc.bounds.min.y - Physics2D.defaultContactOffset;
        float hitPointY = Mathf.Round(hit.point.y * 10) / 10;
        float sin = Mathf.Sin(Mathf.Deg2Rad * Vector2.Angle(hit.normal, Vector2.up));
        float sign = Mathf.Sign(hitPointY - boundMinY);
        boundMinY = Mathf.Round((boundMinY + distance * sin * sign) * 10) / 10;

        if(hitPointY == boundMinY)
        {
            transitionState =  new DefaultState();
        }

        return transitionState;
    }

    private void PreventSliding()
    {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        // Check if on a slope and stationary
        if (slopeAngle > 0 && inputValue.x == 0 && state.CompareState("Default"))
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    public float GetCurrentHealthPercentage()
    {
        if(maxHP == 0) return 1;
        
        return currentHP / maxHP;
    }
}
