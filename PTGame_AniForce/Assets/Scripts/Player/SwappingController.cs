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
    // pool
    [SerializeField] private List<GameObject> animalsOnPool;
    private static List<GameObject> animalsToPool;
    // random
    private static int choice;

    // swap fx
    [SerializeField] private GameObject swapVFX;
    private static GameObject swapVFXOnScreen;
    [SerializeField] private AudioClip swapSFX;
    private static AudioSource audioSource;

    // cinemachine
    [SerializeField] private GameObject cameraObject;
    private static CinemachineVirtualCamera cinemachine;

    // checkpoint
    [SerializeField] Vector2 spawnPosition;
    static public Vector2 CurrentSpawnPosition {get; set;}
    [SerializeField] private int maxLife;
    static private int currentLife;

    // Start is called before the first frame update
    void Start()
    {
        choice = 0;

        if(CurrentSpawnPosition == Vector2.zero)
        {
            CurrentSpawnPosition = spawnPosition;
        }

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

        if(Input.GetKeyDown(KeyCode.G))
        {
            SaveData();
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            LoadData();
        }
    }

    public void GetRandomAnimal()
    {
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
            savedMap = SceneManager.GetActiveScene().name
        };

        string json = JsonUtility.ToJson(data);
  
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public static void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            SceneManager.LoadScene(data.savedMap);
            CurrentSpawnPosition = data.savedCheckpoint;
            currentLife = data.savedLife;
            animalsToPool[choice].GetComponent<PlayerController>().LoadData(data.savedHP);
        }
    }
}
