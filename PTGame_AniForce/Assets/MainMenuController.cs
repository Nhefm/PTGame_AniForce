using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Nếu bạn muốn dừng Play Mode trong Unity Editor
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button loadBtn;
    [SerializeField] private Button newBtn;
    [SerializeField] private Button howBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button aboutBtn;
    [SerializeField] private Button quitBtn;

    [Header("About Us Panel")]
    [SerializeField] private GameObject aboutUsPanel;
    [SerializeField] private Button aboutExitBtn;
    
    [Header("How to Play Panel")]
    [SerializeField] private GameObject howPanel;
    [SerializeField] private Button howExitBtn;


    [Header("Main Menu Panel")]
    [SerializeField] private GameObject mainMenuPanel;

    private void Start()
    {
        // Kiểm tra và add listener cho từng button
        if (loadBtn)    loadBtn.onClick.AddListener(OnLoadGameClicked);
        if (newBtn)     newBtn.onClick.AddListener(OnNewGameClicked);
        if (howBtn)     howBtn.onClick.AddListener(OnHowToPlayClicked);
        if (settingBtn) settingBtn.onClick.AddListener(OnSettingsClicked);
        if (aboutBtn)   aboutBtn.onClick.AddListener(OnAboutUsClicked);
        if (quitBtn)    quitBtn.onClick.AddListener(OnQuitGameClicked);

        if (aboutExitBtn) aboutExitBtn.onClick.AddListener(OnAboutUsExitBtnClicked);
        if (howExitBtn) howExitBtn.onClick.AddListener(OnHowToPlayExitBtnClicked);

        // Thiết lập trạng thái hiển thị ban đầu
        if (aboutUsPanel)   aboutUsPanel.SetActive(false);
        if (howPanel)    howPanel.SetActive(false);
        if (mainMenuPanel)  mainMenuPanel.SetActive(true);
    }

    private void OnHowToPlayExitBtnClicked()
    {
        if (howPanel) howPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }

    private void OnAboutUsExitBtnClicked()
    {
        if (aboutUsPanel)   aboutUsPanel.SetActive(false);
        if (mainMenuPanel)  mainMenuPanel.SetActive(true);
    }

    private void OnLoadGameClicked()
    {
        Debug.Log("Load Game button clicked");
        // SceneManager.LoadScene("LoadGameScene");
    }

    private void OnNewGameClicked()
    {
        Debug.Log("New Game button clicked");
        SceneManager.LoadScene("map1");
    }

    private void OnHowToPlayClicked()
    {
        Debug.Log("How To Play button clicked");
        if (howPanel) howPanel.SetActive(true);
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings button clicked");
        // Hiển thị menu cài đặt hoặc load scene cài đặt.
    }

    private void OnAboutUsClicked()
    {
        Debug.Log("About Us button clicked");
        if (mainMenuPanel)  mainMenuPanel.SetActive(false);
        if (aboutUsPanel)   aboutUsPanel.SetActive(true);
    }

    private void OnQuitGameClicked()
    {
        Debug.Log("Quit Game button clicked.");

    #if UNITY_EDITOR
        EditorApplication.isPlaying = false;  // Dừng Play mode trong Editor
    #else
        Application.Quit();                  // Thoát hẳn ứng dụng khi build
    #endif
    }
}
