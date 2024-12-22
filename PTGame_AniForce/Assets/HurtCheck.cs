using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other) {
        var player = other.GetComponent<PlayerController>();
        Debug.Log(player);
        if(player != null)
        {
            player.ChangeHealth(-1);
        }
    }
}
