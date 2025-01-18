using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 3f;
    public float damage = 1f;

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    protected virtual void _Attack(float damage, GameObject player)
    {
        
    }

    public void Attack(float damage)
    {
        var player = GameObject.FindGameObjectsWithTag("Player");
        _Attack(damage, player[0]);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
