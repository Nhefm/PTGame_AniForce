using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Only needed if you want to stop Play Mode in the Editor
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    private Button loadBtn;
    private Button newBtn;
    private Button howBtn;
    private Button settingBtn;
    private Button aboutBtn;
    private Button quitBtn;

    private void Start()
    {
        // Find each Button by name in the scene hierarchy.
        // Make sure the names match exactly what you see in the Hierarchy.
        var loadBtnGO = GameObject.Find("MainMenu/LoadBtn");
        var newBtnGO = GameObject.Find("MainMenu/NewBtn");
        var howBtnGO = GameObject.Find("MainMenu/HowBtn");
        var settingBtnGO = GameObject.Find("MainMenu/SettingBtn");
        var aboutBtnGO = GameObject.Find("MainMenu/AboutBtn");
        var quitBtnGO = GameObject.Find("MainMenu/QuitBtn");

        // Get the Button component from each game object
        if (loadBtnGO != null)    loadBtn = loadBtnGO.GetComponent<Button>();
        if (newBtnGO != null)     newBtn = newBtnGO.GetComponent<Button>();
        if (howBtnGO != null)     howBtn = howBtnGO.GetComponent<Button>();
        if (settingBtnGO != null) settingBtn = settingBtnGO.GetComponent<Button>();
        if (aboutBtnGO != null)   aboutBtn = aboutBtnGO.GetComponent<Button>();
        if (quitBtnGO != null)    quitBtn = quitBtnGO.GetComponent<Button>();

        // Add listeners
        if (loadBtn)    loadBtn.onClick.AddListener(OnLoadGameClicked);
        if (newBtn)     newBtn.onClick.AddListener(OnNewGameClicked);
        if (howBtn)     howBtn.onClick.AddListener(OnHowToPlayClicked);
        if (settingBtn) settingBtn.onClick.AddListener(OnSettingsClicked);
        if (aboutBtn)   aboutBtn.onClick.AddListener(OnAboutUsClicked);
        if (quitBtn)    quitBtn.onClick.AddListener(OnQuitGameClicked);
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
        // Show how-to-play UI or load a scene, etc.
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings button clicked");
        // Show a settings menu or scene.
    }

    private void OnAboutUsClicked()
    {
        Debug.Log("About Us button clicked");
        // Show an About Us popup or load a scene, etc.
    }

    private void OnQuitGameClicked()
    {
        Debug.Log("Quit Game button clicked.");

        // If running in the Unity Editor, stop Play mode.
        // Otherwise, quit the application.
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
