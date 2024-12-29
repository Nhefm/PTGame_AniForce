using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{

    float horizontal;
    float vertical;
    Rigidbody2D rigidbody2D_;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D_ = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        // press the arrow keys to move the object
        // use input.GetAxis to get the input from the arrow keys

        horizontal = Input.GetAxis("Horizontal");
        //vertical = Input.GetAxis("Vertical");

        // jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            rigidbody2D_.AddForce(Vector3.up * 5, ForceMode2D.Impulse);
        }

        
    }

    private void FixedUpdate()
    {

        // transform by world space
        transform.Translate(horizontal * 5 * Time.deltaTime, 0, vertical * 5 * Time.deltaTime, Space.World);
    }
}
