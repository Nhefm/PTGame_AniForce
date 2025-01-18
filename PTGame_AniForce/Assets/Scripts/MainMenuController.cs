using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayerPrefs = UnityEngine.PlayerPrefs;

#if UNITY_EDITOR
using UnityEditor;  // Needed for EditorApplication.isPlaying
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button quitButton;

    [Header("Panels & Exit Buttons")]
    [SerializeField] private GameObject aboutUsPanel;
    [SerializeField] private Button aboutExitButton;

    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private Button howExitButton;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button settingsExitButton;

    [Header("Main Menu Panel")]
    [SerializeField] private GameObject mainMenuPanel;

    private void Start()
    {
        // Attach event listeners to all buttons if they exist
        if (loadGameButton)   loadGameButton.onClick.AddListener(HandleLoadGame);
        if (newGameButton)    newGameButton.onClick.AddListener(HandleNewGame);
        if (howToPlayButton)  howToPlayButton.onClick.AddListener(HandleHowToPlay);
        if (settingsButton)   settingsButton.onClick.AddListener(HandleSettings);
        if (aboutButton)      aboutButton.onClick.AddListener(HandleAboutUs);
        if (quitButton)       quitButton.onClick.AddListener(HandleQuitGame);

        if (aboutExitButton)  aboutExitButton.onClick.AddListener(HandleCloseAboutUs);
        if (howExitButton)    howExitButton.onClick.AddListener(HandleCloseHowToPlay);
        if (settingsExitButton) settingsExitButton.onClick.AddListener(HandleCloseSettings);

        // Initial panel states
        if (aboutUsPanel)     aboutUsPanel.SetActive(false);
        if (howToPlayPanel)   howToPlayPanel.SetActive(false);
        if (settingsPanel)    settingsPanel.SetActive(false);
        if (mainMenuPanel)    mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Called when the "Load Game" button is clicked.
    /// </summary>
    private void HandleLoadGame()
    {
        Debug.Log("Load Game button clicked.");
        PlayerPrefs.SetInt("isLoad", 1);
        SceneManager.LoadScene("map1");
    }

    /// <summary>
    /// Called when the "New Game" button is clicked.
    /// </summary>
    private void HandleNewGame()
    {
        Debug.Log("New Game button clicked.");
        PlayerPrefs.SetInt("isLoad", 0);
        SceneManager.LoadScene("map1");
    }

    /// <summary>
    /// Called when the "How to Play" button is clicked.
    /// </summary>
    private void HandleHowToPlay()
    {
        Debug.Log("How To Play button clicked.");
        if (howToPlayPanel) howToPlayPanel.SetActive(true);
        if (mainMenuPanel)  mainMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Called when the "Settings" button is clicked.
    /// </summary>
    private void HandleSettings()
    {
        Debug.Log("Settings button clicked.");
        if (settingsPanel)  settingsPanel.SetActive(true);
        if (mainMenuPanel)  mainMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Called when the "About Us" button is clicked.
    /// </summary>
    private void HandleAboutUs()
    {
        Debug.Log("About Us button clicked.");
        if (aboutUsPanel)   aboutUsPanel.SetActive(true);
        if (mainMenuPanel)  mainMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Called when the "Quit" button is clicked.
    /// </summary>
    private void HandleQuitGame()
    {
        Debug.Log("Quit Game button clicked.");

        #if UNITY_EDITOR
            // Stop play mode in the Editor
            EditorApplication.isPlaying = false;
        #else
            // Quit the application in a build
            Application.Quit();
        #endif
    }

    // ---------------------- Panel Close Handlers ----------------------

    /// <summary>
    /// Closes the How to Play panel and returns to the main menu.
    /// </summary>
    private void HandleCloseHowToPlay()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
        if (mainMenuPanel)  mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the About Us panel and returns to the main menu.
    /// </summary>
    private void HandleCloseAboutUs()
    {
        if (aboutUsPanel)   aboutUsPanel.SetActive(false);
        if (mainMenuPanel)  mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the Settings panel and returns to the main menu.
    /// </summary>
    private void HandleCloseSettings()
    {
        if (settingsPanel)  settingsPanel.SetActive(false);
        if (mainMenuPanel)  mainMenuPanel.SetActive(true);
    }
}
