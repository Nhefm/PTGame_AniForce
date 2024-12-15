using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float atk;
    [SerializeField] private float maxHP;
    private float currentHP;
    [SerializeField] private float speed;

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontal;
    private Vector2 lookDirection;
    private State state;
    [SerializeField] float invincibleDuration;



    public enum State {Default, Jump, Attack, Skill, Hurt, Death}

        public void ChangeHealth(int amount)
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

    public IEnumerator InvincibleTimer()
    {
        animator.SetTrigger("Hit");
        //hitSFX.Play();
        //hitParticle.Play();
        state = State.Hurt;
        yield return new WaitForSeconds(invincibleDuration);
        //hitParticle.Stop();
        //hitParticle.Clear();

        if(state != State.Death)
        {
            state = State.Default;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
