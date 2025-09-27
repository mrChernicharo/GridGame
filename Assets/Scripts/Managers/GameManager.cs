using System;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GemSpawner gemSpawner;

    void Start()
    {
        int playerLevel = GameData.LoadPlayerLevel();
        Debug.Log($"playerLevel {playerLevel}");
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        OnScreenTouch();
    }

    void OnDestroy()
    {
        Debug.Log($"RESET PLAYER LEVEL");
        GameData._ResetPlayerLevel();
    }

    void OnScreenTouch()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            // Vector2 pos = Input.GetTouch(0).position;
            // Debug.Log(Camera.main.ScreenToWorldPoint(pos));
            var colors = Enum.GetValues(typeof(GemColor));
            int randIdx = (int)UnityEngine.Random.Range(0f, colors.Length);

            var color = (GemColor)colors.GetValue(randIdx);
            Debug.Log($"{randIdx} :::: {color}");

            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 pos = new Vector3(touchPos.x, touchPos.y, 2.0f);
            gemSpawner.SpawnGem(color, pos);
        }
    }
}