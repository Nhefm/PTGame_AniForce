using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    Material material;
    float distance;

    [Range(0, 0.5f)]
    public float speed = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Camera.main.transform.position.x * speed;
        material.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
