using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider2D> dealDamage;

    private void OnTriggerEnter2D(Collider2D other) {
        // get component enemy
        // if enemy
            dealDamage.Invoke(other);
    }
}
