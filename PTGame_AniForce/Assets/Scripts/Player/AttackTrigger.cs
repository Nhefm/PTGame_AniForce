using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent<Enemy> dealDamage;

    private void OnTriggerEnter2D(Collider2D other) {
        var enemy = other.transform.GetComponent<Enemy>();

        if(enemy)
        {
            dealDamage.Invoke(enemy);
        }
    }
}
