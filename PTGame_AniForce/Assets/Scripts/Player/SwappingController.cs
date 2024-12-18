using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SwappingController : MonoBehaviour
{
    // singleton
    public static SwappingController instance;

    // pool
    [SerializeField] private List<GameObject> animalsToPool;
    private List<GameObject> animalsOnPool;

    // random
    private GameObject currentCharacter;
    private int choice;
    private bool canRandom;
    [SerializeField] float randomCooldown;

    // first spawn position
    [SerializeField] Vector2 spawnPosition;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        animalsOnPool = new List<GameObject>();

        foreach(GameObject animal in animalsToPool)
        {
            GameObject newCharacter = Instantiate(animal);
            animalsOnPool.Add(newCharacter);
            newCharacter.SetActive(false);
        }

        choice = 0;
        currentCharacter = animalsOnPool[choice];
        currentCharacter.transform.position = spawnPosition;
        Debug.Log(currentCharacter.transform.localScale);
        canRandom = true;
        GetRandomAnimal();
        Debug.Log(currentCharacter.transform.localScale);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            GetRandomAnimal();
        }
    }

    public void GetRandomAnimal()
    {
        if(!canRandom)
        {
            return;
        }

        while(animalsOnPool[choice] == currentCharacter)
        {
            choice = Random.Range(0, animalsOnPool.Count);
        }

        animalsOnPool[choice].SetActive(true);


        animalsOnPool[choice].GetComponent<PlayerController>().TakeControl(currentCharacter.transform.position);
        currentCharacter.SetActive(false);
        currentCharacter = animalsOnPool[choice];
        
        StartCoroutine(RandomCooldown());
    }

    public IEnumerator RandomCooldown()
    {
        canRandom = false;
        yield return new WaitForSeconds(randomCooldown);
        canRandom = true;
    }
}
