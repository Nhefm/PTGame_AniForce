using UnityEngine;

public class Pinwheel : Enemy
{
    public Animator npcAnimator; // Animator to control NPC animations

    public float attackRange = 10f;
    public float attackDamage = 10f;
    public float attackCooldown = 3f;

    private float nextAttackTime;
    private bool isDead = false;


    protected override void _Attack(float damage, GameObject player)
    {
        // Check if the player is within attack range
        if (Vector3.Distance(player.transform.position, transform.position) <= attackRange && Time.time >= nextAttackTime && !isDead)
        {
            // Set attack animation bool
            if (npcAnimator != null)
            {
                npcAnimator.SetBool("Attack", true);
            }

            // Attempt to get the PlayerController component and apply damage
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ChangeHealth(damage);
                Debug.Log($"NPC attacked the player for {damage} damage.");

                // Set the next attack time
                nextAttackTime = Time.time + attackCooldown;
            }

            if (npcAnimator != null)
            {
                npcAnimator.SetBool("Attack", false);
            }
        }
    }
}
