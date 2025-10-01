using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GemSpawner gemSpawner;
    [SerializeField] private Board board;
    private int playerLevel;


    async void Start()
    {
        playerLevel = GameData.LoadPlayerLevel();
        Debug.Log($"GameManager ::: playerLevel: {playerLevel}");

        await board.InitializeBoard();
        await gemSpawner.InitializeGems();
    }

    void OnDestroy()
    {
        Debug.Log($"RESET PLAYER LEVEL");
        GameData._ResetPlayerLevel();
    }
}