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
    [SerializeField] protected float invincibleDuration;
    [SerializeField] protected float attackDuration;
    [SerializeField] protected float skillDuration;

    // components
    protected Rigidbody2D rb;
    protected float horizontal;

    // enums
    protected State state;
    protected Direction direction;

    public enum Direction {Left = -1, Right = 1}
    public enum State {Default, Jump, Attack, Skill, Hurt, Death}

    virtual public void ChangeHealth(float amount)
    {
        if(amount < 0)
        {
            if(state == State.Hurt)
            {
                return;
            }

            StartCoroutine(InvincibleTimer());
        }
        
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        //UIHealthBar.instance.SetValue((float)currentHealth / maxHealth);
    }

    abstract public IEnumerator InvincibleTimer();



    // Start is called before the first frame update
    virtual protected void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        direction = Direction.Right;
        state = State.Default;
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        Debug.Log(state);
        horizontal = Input.GetAxis("Horizontal");
        Move();
        Jump(KeyCode.Space);
        Attack(KeyCode.Mouse0);
        Skill(KeyCode.E);

        if(horizontal * (int)direction < 0)
        {
            Flip();
        }
    }

    virtual public void Move()
    {
        if(state != State.Default && state != State.Jump)
        {
            return;
        }

        transform.Translate(speed * Time.deltaTime * horizontal * Vector2.right);
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

        StartCoroutine(AttackTimer());
    }

    virtual public IEnumerator AttackTimer()
    {
        state = State.Attack;

        yield return new WaitForSeconds(attackDuration);

        if(state == State.Attack)
        {
            state = State.Default;
        } 
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

        StartCoroutine(SkillTimer());
    }

    abstract public IEnumerator SkillTimer();

    virtual protected void OnCollisionEnter2D(Collision2D other) {

        if(state != State.Jump)
        {
            return;
        }

        if (other.collider.CompareTag("Ground"))
        {
            state = State.Default;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }
}
