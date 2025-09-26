using UnityEngine;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    private LevelSO levelToLoad;

    [SerializeField] private LevelListSO levelList;
    [SerializeField] private TextMeshProUGUI levelTextMesh;
    void Start()
    {

        int playerLevel = GameData.LoadPlayerLevel();
        int lvlIdx = playerLevel % levelList.levels.Length;

        levelToLoad = levelList.levels[lvlIdx];

        // Check if a level has been assigned
        if (levelToLoad != null)
        {
            LoadLevel();
        }
        else
        {
            Debug.LogError("No Level Scriptable Object assigned to LevelLoader!");
        }
    }

    void LoadLevel()
    {
        // Access the data from the Scriptable Object
        string levelName = levelToLoad.name;
        int numRows = levelToLoad.rows;
        int numCols = levelToLoad.columns;

        Debug.Log($"Loading level with {numRows} rows and {numCols} columns.");
        levelTextMesh.text = levelName;

        // Here's where you'd add your level generation logic.
        // For example, you could instantiate a grid, position objects,
        // or whatever your game needs.
    }
}
