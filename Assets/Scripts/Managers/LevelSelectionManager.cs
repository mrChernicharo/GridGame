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

    // --- Step 3: Create a button for each level ---
    private void CreateLevelButtons()
    {
        if (allLevels == null || allLevels.Length == 0)
        {
            Debug.LogError("No LevelSO assets found in Resources folders!");
            return;
        }

        foreach (LevelSO level in allLevels)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonParent);
            Button button = buttonGO.GetComponent<Button>();

            TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = level.name;

            button.onClick.AddListener(() => OnLevelButtonClicked(level));
        }
    }

    // --- Step 4: Handle the button click ---
    private void OnLevelButtonClicked(LevelSO selectedLevel)
    {
        // Store the selected level information somewhere accessible by the next scene
        // A simple way is a static field in a persistent manager or a temporary object.
        LevelLoader.currentLevel = selectedLevel;

        // Load the main level scene
        SceneManager.LoadScene(levelSceneName);
    }
}