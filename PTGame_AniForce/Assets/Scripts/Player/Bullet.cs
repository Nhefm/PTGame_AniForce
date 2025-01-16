using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    [SerializeField] private UnityEvent<Enemy> dealDame;

    void Update()
    {
        
        if(transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        
        if(other.collider.CompareTag("Player"))
        {
            return;
        }

        var enemy = other.transform.GetComponent<Enemy>();

        if(enemy)
        {
            dealDame.Invoke(enemy);
        }
        
        gameObject.SetActive(false);
    }

    private void OnBecameInvisible() {
        gameObject.SetActive(false);
    }
}
