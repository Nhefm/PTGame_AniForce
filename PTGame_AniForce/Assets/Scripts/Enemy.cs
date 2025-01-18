using System;
using Unity.Behavior;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public Animator npcAnimator; // Animator to control NPC animations

    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public float nextAttackTime;

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    protected virtual void _Attack(float damage, GameObject player)
    {
        
    }

    public void Attack()
    {
        var player = GameObject.FindGameObjectsWithTag("Player");
        _Attack(-damage, player[0]);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        this.GetComponent<BehaviorGraphAgent>().SetVariableValue("AttackRange", attackRange);
    }
}
