using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GemSpawner gemSpawner;

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            // Vector2 pos = Input.GetTouch(0).position;
            // Debug.Log(pos);
            // Debug.Log(Camera.main.ScreenToWorldPoint(pos));

            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 pos = new Vector3(touchPos.x, touchPos.y, 2.0f);
            gemSpawner.SpawnGem(GemColor.Amber, pos);
        }
    }
}