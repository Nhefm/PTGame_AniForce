using UnityEngine;

public class ShootingPinwheel : Enemy
{
    protected override void _Attack(float damage, GameObject player)
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= 10)
        {
            player.GetComponent<PlayerController>().ChangeHealth(damage);
        }
    }
}