using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GemSpawner gemSpawner;
    [SerializeField] private Board2 board;
    private int playerLevel;


    async void Start()
    {
        playerLevel = GameData.LoadPlayerLevel();
        Debug.Log($"GameManager ::: playerLevel: {playerLevel}");

        await board.InitializeBoard();
        await gemSpawner.InitializeGems();

        // StartCoroutine(RunGameStartSequence());
    }




    void OnDestroy()
    {
        Debug.Log($"RESET PLAYER LEVEL");
        GameData._ResetPlayerLevel();
    }

    // void Update()
    // {
    //     if (Input.touchCount == 0) return;

    //     OnScreenTouch();
    // }


    // void OnScreenTouch()
    // {
    //     Touch touch = Input.GetTouch(0);
    //     if (touch.phase == TouchPhase.Began)
    //     {

    //         GemColor color = Helpers.GetRandomGemColor();
    //         Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
    //         Vector2 pos = new Vector3(touchPos.x, touchPos.y, 2.0f);
    //         gemSpawner.SpawnGem(color, pos);
    //     }
    // }


}