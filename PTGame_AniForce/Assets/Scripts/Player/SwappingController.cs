using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Cinemachine;
using System.IO;
using UnityEngine.SceneManagement;

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

    // swap fx
    [SerializeField] private GameObject swapVFX;
    private GameObject swapVFXOnScreen;
    [SerializeField] private AudioClip swapSFX;
    private AudioSource audioSource;

    // cinemachine
    [SerializeField] private GameObject cameraObject;
    private CinemachineVirtualCamera cinemachine;

    // checkpoint
    [SerializeField] Vector2 spawnPosition;
    static public Vector2 CurrentSpawnPosition {get; set;}
    [SerializeField] private int maxLife;
    static private int currentLife;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        choice = 0;
        canRandom = true;
        CurrentSpawnPosition = spawnPosition;
        currentLife = maxLife;
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

        GetRandomAnimal();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(currentLife);
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
        animalsToPool[choice].transform.position = CurrentSpawnPosition;
        animalsToPool[previousChoice].SetActive(false);

        cinemachine.m_Follow = animalsToPool[choice].transform;

        swapVFXOnScreen.transform.position = animalsToPool[choice].transform.position;
        swapVFXOnScreen.GetComponent<Animator>().SetTrigger("Swap");
        audioSource.PlayOneShot(swapSFX);

        // StartCoroutine(RandomCooldown());
    }

    public IEnumerator RandomCooldown()
    {
        canRandom = false;
        yield return new WaitForSeconds(randomCooldown);
        canRandom = true;
    }

    public void Swap()
    {
        if(currentLife == 0)
        {
            // game over
            return;
        }

        --currentLife;
        GetRandomAnimal();
    }

    public void SaveData()
    {
        SaveData data = new()
        {
            savedHP = animalsToPool[choice].GetComponent<PlayerController>().GetCurrentHealthPercentage(),
            savedLife = currentLife,
            savedCheckpoint = CurrentSpawnPosition,
            savedChoice = choice,
            savedMap = SceneManager.GetActiveScene().name
        };

        string json = JsonUtility.ToJson(data);
  
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            SceneManager.LoadScene(data.savedMap);
            CurrentSpawnPosition = data.savedCheckpoint;
            currentLife = data.savedLife;
            SetCurrentAnimal(data.savedChoice);
            animalsToPool[choice].GetComponent<PlayerController>().LoadData(data.savedHP);
        }
    }

    public void SetCurrentAnimal(int idx)
    {
        animalsToPool[idx].SetActive(true);
        animalsToPool[idx].transform.position = CurrentSpawnPosition;
        animalsToPool[choice].SetActive(false);
        choice = idx;
        cinemachine.m_Follow = animalsToPool[choice].transform;
    }
}
