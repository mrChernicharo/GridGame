using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;
    private string currentScene = "";
    // public static int levelIdx = 0;

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

        GameObject startButtonGO = GameObject.FindGameObjectWithTag("Button.Start");
        GameObject backButtonGO = GameObject.FindGameObjectWithTag("Button.Back");
        GameObject oldLevelButtonGO = GameObject.FindGameObjectWithTag("Button.Level");



        if (startButtonGO != null)
        {
            Button startButton = startButtonGO.GetComponent<Button>();
            startButton.onClick.AddListener(() => SceneManager.LoadScene("LevelSelectionScreen"));
        }
        if (oldLevelButtonGO != null)
        {
            Button oldLevelButton = oldLevelButtonGO.GetComponent<Button>();
            oldLevelButton.onClick.AddListener(() => SceneManager.LoadScene("LevelScreen"));
        }
        if (backButtonGO != null)
        {
            Button backButton = backButtonGO.GetComponent<Button>();
            backButton.onClick.AddListener(() =>
            {
                if (SceneManager.GetActiveScene().name == "LevelSelectionScreen")
                    SceneManager.LoadScene("MainScreen");
                else
                    SceneManager.LoadScene("LevelSelectionScreen");
            });
        }
    }
}
