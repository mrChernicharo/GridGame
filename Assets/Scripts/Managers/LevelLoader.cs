using UnityEngine;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    [HideInInspector]
    public static LevelSO currentLevel;

    [SerializeField] private LevelListSO levelList;
    [SerializeField] private TextMeshProUGUI levelTextMesh;
    void Awake()
    {

        currentLevel = levelList.levels[ScreenManager.levelIdx];
        if (currentLevel == null)
        {
            Debug.LogError("No Level Scriptable Object assigned to LevelLoader!");
            return;
        }

        // Access the data from the Scriptable Object
        int numRows = currentLevel.rows;
        int numCols = currentLevel.columns;

        Debug.Log($"Loading level with {numRows} rows and {numCols} columns.");
        levelTextMesh.text = currentLevel.name;
    }

}
