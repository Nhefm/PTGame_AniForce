using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DogSkillTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent<Enemy> dealSkillDame;

    private void OnTriggerEnter2D(Collider2D other) {
        var enemy = other.transform.GetComponent<Enemy>();

        if(enemy)
        {
            dealSkillDame.Invoke(enemy);
        }
    }
}
