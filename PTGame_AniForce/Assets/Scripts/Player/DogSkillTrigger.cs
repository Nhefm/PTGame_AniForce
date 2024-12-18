using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DogSkillTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider2D> dealSkillDame;

    private void OnTriggerEnter2D(Collider2D other) {
        // get component enemy
        // if enemy
            dealSkillDame.Invoke(other);
    }
}
