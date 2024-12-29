using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Cinemachine;

public class SwappingController : MonoBehaviour
{
    // singleton
    public static SwappingController instance;

    // pool
    [SerializeField] private List<GameObject> animalsOnPool;
    private List<GameObject> animalsToPool;
    // random
    private int choice;
    private bool canRandom;
    [SerializeField] float randomCooldown;

    // first spawn position
    [SerializeField] Vector2 spawnPosition;

    // swap fx
    [SerializeField] private GameObject swapVFX;
    private GameObject swapVFXOnScreen;
    [SerializeField] private AudioClip swapSFX;
    private AudioSource audioSource;

    // cinemachine
    [SerializeField] private GameObject cameraObject;
    private CinemachineVirtualCamera cinemachine;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        choice = 0;
        canRandom = true;
        swapVFXOnScreen = Instantiate(swapVFX);
        audioSource = GetComponent<AudioSource>();
        animalsToPool = new List<GameObject>();
        cinemachine = cameraObject.GetComponent<CinemachineVirtualCamera>();

        if(animalsOnPool.Count == 0)
        {
            return;
        }

        foreach(var animal in animalsOnPool)
        {
            GameObject temp = Instantiate(animal);
            temp.SetActive(false);
            animalsToPool.Add(temp);
        }

        animalsToPool[choice].transform.position = spawnPosition;

        GetRandomAnimal();
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

        int previousChoice = choice;

        while(previousChoice == choice)
        {
            choice = Random.Range(0, animalsOnPool.Count);
        }

        animalsToPool[choice].SetActive(true);
        animalsToPool[choice].transform.position = animalsToPool[previousChoice].transform.position;
        animalsToPool[previousChoice].SetActive(false);

        cinemachine.m_Follow = animalsToPool[choice].transform;

        swapVFXOnScreen.transform.position = animalsToPool[choice].transform.position;
        swapVFXOnScreen.GetComponent<Animator>().SetTrigger("Swap");
        audioSource.PlayOneShot(swapSFX);

        StartCoroutine(RandomCooldown());
    }

    public IEnumerator RandomCooldown()
    {
        canRandom = false;
        yield return new WaitForSeconds(randomCooldown);
        canRandom = true;
    }
}
