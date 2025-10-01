// using UnityEngine;

// public class LevelButtons : MonoBehaviour
// {
//     [SerializeField] LevelListSO levelListSO;

//     void OnEnable()
//     {
//         Debug.Log($"currentLevel: {LevelLoader.currentLevel} levelListSO: {levelListSO.levels.Length} levels");

//     }




// }

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class LevelSelectionManager : MonoBehaviour
{
    // The LevelSO type, replace with your actual class name if different
    // [System.Serializable] // Only needed if you want it to appear in the inspector as a field
    // public class LevelSO : ScriptableObject { ... } 

    [Header("UI References")]
    public Transform buttonParent; // Parent object for the level buttons (e.g., a ScrollView Content)
    public GameObject levelButtonPrefab; // Prefab of your button UI element

    [Header("Scene Management")]
    public string levelSceneName = "Level"; // Name of the scene to load the level in

    private LevelSO[] allLevels;

    void Start()
    {
        LoadAllLevels();
        CreateLevelButtons();
    }

    private void LoadAllLevels()
    {
        allLevels = Resources
            .LoadAll<LevelSO>("ScriptableObjects/Levels")
            .OrderBy(level => level.name)
            .ToArray();
    }

    private void CreateLevelButtons()
    {
        if (allLevels == null || allLevels.Length == 0)
        {
            Debug.LogError("No LevelSO assets found in Resources folders!");
            return;
        }

        int levelIdx = 0;
        foreach (LevelSO level in allLevels)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonParent);
            Button button = buttonGO.GetComponent<Button>();

            TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = level.name;

            button.onClick.AddListener(() => OnLevelButtonClicked(level, levelIdx));
            levelIdx++;
        }
    }

    private void OnLevelButtonClicked(LevelSO selectedLevel, int levelIdx)
    {
        LevelLoader.currentLevel = selectedLevel;
        ScreenManager.levelIdx = levelIdx;
        SceneManager.LoadScene(levelSceneName);
    }
}