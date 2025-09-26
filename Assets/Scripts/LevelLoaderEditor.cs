using UnityEngine;
using UnityEditor;
using System.Linq;

// This class handles the automated loading of LevelSOs into a runtime asset.
public static class LevelLoaderEditor
{
    private const string TargetFolderPath = "Assets/ScriptableObjects/Levels";
    private const string LevelListPath = "Assets/ScriptableObjects/LevelList.asset"; // Path to your LevelListSO instance

    // Editor menu item to trigger the automatic update
    [MenuItem("Tools/Level System/Update Level List")]
    public static void UpdateLevelList()
    {
        // 1. Load the runtime Level List asset (or create it if it doesn't exist)
        LevelListSO allLevelsAsset = AssetDatabase.LoadAssetAtPath<LevelListSO>(LevelListPath);

        if (allLevelsAsset == null)
        {
            allLevelsAsset = ScriptableObject.CreateInstance<LevelListSO>();
            AssetDatabase.CreateAsset(allLevelsAsset, LevelListPath);
            Debug.Log($"Created new LevelListSO asset at {LevelListPath}");
        }

        // 2. Find all LevelSO assets in the specified folder
        string[] guids = AssetDatabase.FindAssets("t:LevelSO", new[] { TargetFolderPath });

        // 3. Convert GUIDs to actual LevelSO objects
        LevelSO[] loadedLevels = (LevelSO[])guids
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath<LevelSO>(path))
            .OrderBy(level => level.name).ToArray(); // Optional: Sort by name, ID, etc.

        // 4. Update the runtime asset and save
        allLevelsAsset.levels = loadedLevels;
        EditorUtility.SetDirty(allLevelsAsset); // Mark the asset as changed
        AssetDatabase.SaveAssets();             // Save changes to disk

        Debug.Log($"Level List successfully updated with {loadedLevels} levels.");
    }
}