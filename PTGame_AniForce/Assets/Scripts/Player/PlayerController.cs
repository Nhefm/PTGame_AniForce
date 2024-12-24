using System.Collections;
using System.Collections.Generic;
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

    // enums
    protected State state;
    protected Direction direction;

    public enum Direction {Left = -1, Right = 1}
    public enum State {Default, Jump, Attack, Skill, Death}

    // slope
    [SerializeField] protected float slopeCheckDistance;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        bc = GetComponent<BoxCollider2D>();
        direction = Direction.Right;
        inputActions = new PlayerInputAction();
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += Move;
        inputActions.Player.Move.canceled += Move;
        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Attack.performed += Attack;
        inputActions.Player.Skill.performed += Skill;
    }
    
    virtual protected void Update()
    {
        SlopeHandler();
        Move();
    }

    virtual protected void OnEnable()
    {
        if(state == State.Death)
        {
            return;
        }

        currentHP = maxHP;
        state = State.Default;
        canAttack = true;
        canSkill = true;
        isInvincible = false;

        if(inputActions != null)
        {
            inputActions.Player.Enable();
        }
    }

    virtual protected void OnDisable() {

        if(inputActions != null)
        {
            inputActions.Player.Disable();
        }
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
            StartCoroutine(InvincibleTimer());
        }
        
        //UIHealthBar.instance.SetValue((float)currentHealth / maxHealth);
    }

    abstract public IEnumerator InvincibleTimer();

    virtual public void Move(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            inputValue = Vector2.right * context.ReadValue<Vector2>();
        }

        if(context.canceled)
        {
            inputValue = Vector2.zero;
        }
    }

    virtual public void Move()
    {
        if(state != State.Default && state != State.Jump)
        {
            return;
        }

        transform.Translate(speed * Time.deltaTime * inputValue);

        if(inputValue.x * (int)direction < 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        direction = (Direction)(-(int)direction);
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    virtual public void Jump(InputAction.CallbackContext context)
    {
        if(!context.performed)
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
    virtual public void Attack(InputAction.CallbackContext context)
    {
        if(!context.performed)
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

    virtual public void Skill(InputAction.CallbackContext context)
    {
        if(!context.performed)
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

        if(!other.collider.CompareTag("Ground"))
        {
            return;
        }

        rb.velocity = Vector2.zero;
        
        if(state == State.Jump)
        {
            state = State.Default;
        }
        else if(state == State.Death)
        {
            rb.simulated = false;
        }
    }

    public float getCurrentHealthPercentage()
    {
        if(maxHP == 0) return 1;
        
        return currentHP / maxHP;
    }

    virtual public void SlopeHandler()
    {
        Vector2 checkPos = transform.position + Vector3.right * bc.size.x / 2 * (int)direction;
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, LayerMask.GetMask("Ground"));

        if(!hit)
        {
            return;
        }

        Debug.DrawRay(hit.point, hit.normal, Color.green);

        if(Vector2.Angle(hit.normal, Vector2.up) == 0)
        {
            return;
        }

        if(state != State.Default)
        {
            return;
        }

        rb.velocity = Vector2.zero;
    }
}
