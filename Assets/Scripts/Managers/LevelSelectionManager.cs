using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using System.Threading.Tasks;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform buttonParent; // Parent object for the level buttons (e.g., a ScrollView Content)
    public GameObject levelButtonPrefab;

    [Header("Scene Management")]
    public string levelSceneName = "Level";
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

        for (int idx = 0; idx < allLevels.Length; idx++)
        {
            int levelIdx = idx;

            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonParent);
            Button button = buttonGO.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = $"Level {levelIdx + 1}";

            button.onClick.AddListener(() => OnLevelButtonClicked(levelIdx));
        }
    }

    private async void OnLevelButtonClicked(int levelIdx)
    {
        Debug.Log($"currentLevelIdx {levelIdx}");
        PlayerPrefs.SetInt("currentLevelIdx", levelIdx);

        await Task.Delay(100);

        SceneManager.LoadScene(levelSceneName);
    }
}