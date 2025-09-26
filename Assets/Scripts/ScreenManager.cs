using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    static ScreenManager instance;
    string currentScene;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        currentScene = SceneManager.GetActiveScene().name;
        // Debug.Log($"OnSceneLoaded ::: {currentScene}");

        switch (currentScene)
        {
            case "MainScreen":
                GameObject startButtonGO = GameObject.FindGameObjectWithTag("Button.Start");
                Button startButton = startButtonGO.GetComponent<Button>();
                startButton.onClick.AddListener(OnStartButtonClick);
                break;
            case "LevelSelectionScreen":
                GameObject backButtonGO = GameObject.FindGameObjectWithTag("Button.Back");
                GameObject levelButtonGO = GameObject.FindGameObjectWithTag("Button.Level");
                GameObject level01ButtonGO = GameObject.FindGameObjectWithTag("Button.Level.01");
                Button backButton = backButtonGO.GetComponent<Button>();
                Button levelButton = levelButtonGO.GetComponent<Button>();
                Button level01Button = level01ButtonGO.GetComponent<Button>();

                backButton.onClick.AddListener(OnBackButtonClick);
                levelButton.onClick.AddListener(OnLevelButtonClick);
                level01Button.onClick.AddListener(OnLevel01ButtonClick);
                break;
            default:
                GameObject backButtonGOD = GameObject.FindGameObjectWithTag("Button.Back");
                Button backButtonD = backButtonGOD.GetComponent<Button>();
                backButtonD.onClick.AddListener(OnBackButtonClick);
                break;
        }


    }


    private void OnBackButtonClick()
    {
        // Debug.Log("OnBackButtonClick::");
        if (SceneManager.GetActiveScene().name == "LevelSelectionScreen")
        {
            instance.LoadScreen("MainScreen");
        }
        else
        {
            instance.LoadScreen("LevelSelectionScreen");
        }
    }

    private void OnLevelButtonClick()
    {
        // Debug.Log("OnSLevelButtonClick::");
        instance.LoadScreen("LevelScreen");
    }
    private void OnLevel01ButtonClick()
    {
        instance.LoadScreen("Level");
    }
    private void OnStartButtonClick()
    {
        // Debug.Log("OnStartButtonClick::");
        instance.LoadScreen("LevelSelectionScreen");
    }



    public void LoadScreen(string screenName)
    {
        SceneManager.LoadScene(screenName);
    }
}
