using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    Transform cam;
    Vector3 camStartPosition;
    float distance;


    GameObject[] backgrounds;
    Material[] materials;
    float[] backgroundSpeed;

    float farthestBack;

    [Range(0.01f, 0.05f)]
    public float parallaxSpeed = 0.02f;
    



    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        camStartPosition = cam.position;

        int backCount = transform.childCount;
        materials = new Material[backCount];
        backgrounds = new GameObject[backCount];
        backgroundSpeed = new float[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            materials[i] = backgrounds[i].GetComponent<MeshRenderer>().material;
            //backgroundSpeed[i] = materials[i].GetFloat("_Speed");
        }
        BackSpeedCalculate(backCount);
    }


    void BackSpeedCalculate(int backCount)
    {
        for (int i = 0; i < backCount; i++)
        {
            if (backgrounds[i].transform.position.z - cam.position.z > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++)
        {
            backgroundSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        distance = cam.position.x - camStartPosition.x;
        //transform.position = new Vector3(cam.position.x, transform.position.y, transform.position.z);
        transform.position = new Vector3(cam.position.x, transform.position.y, transform.position.z);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backgroundSpeed[i] * parallaxSpeed;
            materials[i].SetTextureOffset("_MainTex", new Vector2(distance * speed, 0));
        }
    }
}
