using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class BoardInputManager : MonoBehaviour
{

    [HideInInspector] public bool isLocked = false;
    [HideInInspector] public bool isDragging = false;
    private Vector3 offset = Vector3.zero;
    private BoxCollider2D currentCollider = null;
    private float dragTriggerDistance = 0.36f;

    Gem2 draggingGem;
    Gem2 otherGem;

    [SerializeField] Board2 board;

    void OnEnable()
    {
        BoardChecker.MoveGemsBack += OnMoveGemsBack;
    }

    void OnDisable()
    {
        BoardChecker.MoveGemsBack -= OnMoveGemsBack;
    }

    async void Update()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);
        // Debug.Log($"Touch {touch.position}");

        switch (touch.phase)
        {
            case TouchPhase.Began:
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);

                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                if (hit.collider == null) return;

                BoxCollider2D collider = hit.collider.GetComponent<BoxCollider2D>();
                if (collider == null) return;

                Debug.Log($"Hit Gem: {collider.name} at position {worldPoint}");

                Gem2 clickedGem = collider.GetComponent<Gem2>();
                if (clickedGem.IsMoving()) return;

                currentCollider = collider;
                isDragging = true;
                offset = clickedGem.transform.position - new Vector3(worldPoint.x, worldPoint.y, 0f);

                break;
            case TouchPhase.Moved:
                if (!isDragging || isLocked || currentCollider == null) return;

                draggingGem = currentCollider.GetComponent<Gem2>();
                Tile tile = board.GetTileFromPosition(draggingGem.transform.position);

                Vector3 startPos = draggingGem.transform.position + offset;
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);


                if (Vector2.Distance(startPos, touchPos) <= dragTriggerDistance) return;

                isLocked = true;
                isDragging = false;
                currentCollider = null;
                offset = Vector3.zero;

                float angleRad = Mathf.Atan2(touchPos.y - startPos.y, touchPos.x - startPos.x);
                float angleDeg = angleRad * (180 / Mathf.PI);

                Nullable<Direction> dir = null;
                if (tile.row > 0 && angleDeg >= -135 && angleDeg < -45)
                    dir = Direction.Down;
                else if (tile.col < board.cols - 1 && angleDeg >= -45 && angleDeg < 45)
                    dir = Direction.Right;
                else if (tile.row < board.rows - 1 && angleDeg >= 45 && angleDeg < 135)
                    dir = Direction.Up;
                else if (tile.col > 0 && angleDeg >= 135 || angleDeg < -135)
                    dir = Direction.Left;
                if (dir == null) return;

                // Debug.Log($"angle {angleDeg} -> Direction {dir}");

                GameObject otherGemObj = dir switch
                {
                    Direction.Up => board.gems[tile.row + 1, tile.col],
                    Direction.Right => board.gems[tile.row, tile.col + 1],
                    Direction.Down => board.gems[tile.row - 1, tile.col],
                    Direction.Left => board.gems[tile.row, tile.col - 1],
                    _ => board.gems[tile.row, tile.col],
                };

                otherGem = otherGemObj.GetComponent<Gem2>();

                SwapGems(draggingGem, otherGem);
                // await Task.Delay(200);

                isLocked = false;
                break;
            case TouchPhase.Ended:
                isDragging = false;
                currentCollider = null;
                offset = Vector3.zero;
                break;
        }
    }


    void SwapGems(Gem2 gem1, Gem2 gem2)
    {
        Vector2 pos1 = gem1.transform.position;
        Vector2 pos2 = gem2.transform.position;
        gem1.Move(pos2);
        gem2.Move(pos1);

    }


    protected virtual void OnMoveGemsBack(object sender, MoveGemsBackEventArgs ev)
    {
        if (draggingGem && otherGem)
            SwapGems(draggingGem, otherGem);

        // draggingGem = null;
        // otherGem = null;
    }
}