using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    static public ObjectPooling sharedInstance;
    private List<GameObject> listObj;
    [SerializeField] private float amountToPool;
    [SerializeField] private GameObject objectToPool;

    void Awake()
    {
        sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        listObj = new List<GameObject>();
        
        for(int i = 0; i < amountToPool; i++)
        {
            GameObject temp = Instantiate(objectToPool);
            temp.SetActive(false);
            listObj.Add(temp);
        }
    }

    public GameObject GetObject()
    {
        for(int i = 0; i < amountToPool; i++)
        {
            if(!listObj[i].activeInHierarchy)
            {
                return listObj[i];
            }
        }

        return null;
    }
}
