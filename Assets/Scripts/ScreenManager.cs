using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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

        Debug.Log($"OnSceneLoaded ::: {currentScene}");

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
                Button backButton = backButtonGO.GetComponent<Button>();
                Button levelButton = levelButtonGO.GetComponent<Button>();

                backButton.onClick.AddListener(OnBackButtonClick);
                levelButton.onClick.AddListener(OnSLevelButtonClick);
                break;
            case "LevelScreen":
                GameObject backButtonGO2 = GameObject.FindGameObjectWithTag("Button.Back");
                Button backButton2 = backButtonGO2.GetComponent<Button>();

                backButton2.onClick.AddListener(OnBackButtonClick);
                break;
        }


    }


    private void OnBackButtonClick()
    {
        Debug.Log("OnBackButtonClick::");
        if (SceneManager.GetActiveScene().name == "LevelScreen")
        {
            instance.LoadScreen("LevelSelectionScreen");
        }
        if (SceneManager.GetActiveScene().name == "LevelSelectionScreen")
        {
            instance.LoadScreen("MainScreen");
        }

    }

    private void OnSLevelButtonClick()
    {
        Debug.Log("OnSLevelButtonClick::");
        instance.LoadScreen("LevelScreen");
    }

    private void OnStartButtonClick()
    {
        Debug.Log("OnStartButtonClick::");
        instance.LoadScreen("LevelSelectionScreen");
    }

    public void LoadScreen(string screenName)
    {
        SceneManager.LoadScene(screenName);
    }
}
