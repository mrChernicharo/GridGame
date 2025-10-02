using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GemSpawner gemSpawner;
    [SerializeField] private Board board;
    [SerializeField] private LevelListSO levelList;
    [HideInInspector] public LevelSO currentLevel;

    void Awake()
    {
        int levelIdx = PlayerPrefs.GetInt("currentLevelIdx", -1);
        if (levelIdx == -1)
        {
            Debug.LogError("Error loading currentLevel");
            return;
        }

        currentLevel = levelList.levels[levelIdx];
    }

    async void Start()
    {
        if (currentLevel == null)
        {
            Debug.LogError("Error loading currentLevel");
            return;
        }

        await board.InitializeBoard(currentLevel.rows, currentLevel.columns, currentLevel.gemColors);
        await gemSpawner.InitializeGems();
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt("currentLevelIdx", -1);
    }
}