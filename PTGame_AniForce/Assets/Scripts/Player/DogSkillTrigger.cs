using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DogSkillTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        var enemy = other.transform.GetComponent<Enemy>();

        if(enemy)
        {  
            enemy.TakeDamage(2);
        }
    }
}
