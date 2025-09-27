using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class BoardInputManager : MonoBehaviour
{

    public bool isLocked = false;
    public bool isDragging = false;
    private Vector3 offset = Vector3.zero;
    private BoxCollider2D currentCollider = null;
    private float dragTriggerDistance = 0.25f;

    [SerializeField] Board2 board;

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

                Gem2 draggingGem = currentCollider.GetComponent<Gem2>();
                Tile tile = board.GetTileFromPosition(draggingGem.transform.position);

                Vector3 startPos = draggingGem.transform.position + offset;
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);


                if (Vector2.Distance(startPos, touchPos) <= dragTriggerDistance) return;
                isLocked = true;

                float angleRad = Mathf.Atan2(touchPos.y - startPos.y, touchPos.x - startPos.x);
                float angleDeg = angleRad * (180 / Mathf.PI);

                // Debug.Log($"angle {angleDeg}");



                Nullable<Direction> dir = null;
                if (tile.row > 0 && angleDeg >= -135 && angleDeg < -45)
                {
                    dir = Direction.Down;
                }
                else if (tile.col < board.cols - 1 && angleDeg >= -45 && angleDeg < 45)
                {
                    dir = Direction.Right;
                }
                else if (tile.row < board.rows - 1 && angleDeg >= 45 && angleDeg < 135)
                {
                    dir = Direction.Up;
                }
                else if (tile.col > 0 && angleDeg >= 135 || angleDeg < -135)
                {
                    dir = Direction.Left;
                }

                if (dir == null) return;

                // Debug.Log($"angle {angleDeg} -> Direction {dir}");

                // int gemIdx = tile.row * board.cols + tile.col;
                // int otherIdx = -1;
                // switch (dir)
                // {
                //     case Direction.Up:
                //         otherIdx = gemIdx + board.cols;
                //         break;
                //     case Direction.Right:
                //         otherIdx = gemIdx + 1;
                //         break;
                //     case Direction.Down:
                //         otherIdx = gemIdx - board.cols;
                //         break;
                //     case Direction.Left:
                //         otherIdx = gemIdx - 1;
                //         break;
                // }

                // GameObject thisGem = board.gems[gemIdx];
                // GameObject otherGem = board.gems[otherIdx];

                // GameObject thisGem = draggingGem.gameObject;
                GameObject otherGemObj = dir switch
                {
                    Direction.Up => board.gems[tile.row + 1, tile.col],
                    Direction.Right => board.gems[tile.row, tile.col + 1],
                    Direction.Down => board.gems[tile.row - 1, tile.col],
                    Direction.Left => board.gems[tile.row, tile.col - 1],
                    _ => board.gems[tile.row, tile.col],
                };
                Tile otherTile = dir switch
                {
                    Direction.Up => board.GetTile(tile.row + 1, tile.col),
                    Direction.Right => board.GetTile(tile.row, tile.col + 1),
                    Direction.Down => board.GetTile(tile.row - 1, tile.col),
                    Direction.Left => board.GetTile(tile.row, tile.col - 1),
                    _ => board.GetTile(tile.row, tile.col),
                };
                Gem2 otherGem = otherGemObj.GetComponent<Gem2>();

                draggingGem.Move(otherTile.GetPosition());
                otherGem.Move(tile.GetPosition());

                await Task.Delay(500);

                isLocked = false;


                // GemSwap(thisGem, gemIdx, otherGem, otherIdx, (Direction)dir);

                // BoardResult boardResult = CheckBoard();

                // // pluck gems to remove
                // if (boardResult.gemsToRemove.Count > 0)
                // {
                //     StartCoroutine(SpawnNewGems(boardResult));
                // }
                // else
                // {
                //     var backDir = dir switch
                //     {
                //         Direction.Up => Direction.Down,
                //         Direction.Right => Direction.Left,
                //         Direction.Down => Direction.Up,
                //         _ => Direction.Right,
                //     };
                //     // move gem back
                //     StartCoroutine(GemSwapBack(thisGem, gemIdx, otherGem, otherIdx, backDir));
                // }

                break;
            case TouchPhase.Ended:
                isDragging = false;
                currentCollider = null;
                offset = Vector3.zero;
                break;
        }
    }
}