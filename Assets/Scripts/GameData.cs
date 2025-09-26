using UnityEngine;

public static class GameData
{
    private const string LevelKey = "PlayerCurrentLevel";

    public static void SavePlayerLevel(int levelNumber)
    {
        PlayerPrefs.SetInt(LevelKey, levelNumber);
        // Write the data to disk
        PlayerPrefs.Save();
        // Debug.Log("Level saved: " + levelNumber);
    }

    public static int LoadPlayerLevel()
    {
        int defaultVal = 0;
        int loadedLevel = PlayerPrefs.GetInt(LevelKey, defaultVal);
        // Debug.Log("Level loaded: " + loadedLevel);
        return loadedLevel;
    }

    public static void _ResetPlayerLevel()
    {
        PlayerPrefs.SetInt(LevelKey, 0);
    }
}