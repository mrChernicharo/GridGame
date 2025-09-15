using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;


enum Direction
{
    Up, Down, Left, Right
}

public class Board : MonoBehaviour
{
    [SerializeField] private float CELL_GAP;
    [SerializeField] private float LEFT_MARGIN; // -1.885
    [SerializeField] private float TOP_MARGIN; // -1.885
    [SerializeField] private int rows;
    [SerializeField] private int columns;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject[] gemPrefabs;

    // initialization *********************************************
    private int _col = 0;
    private int _row = 0;

    // drag ********************************************************
    Rigidbody currentRigidbody = null;
    bool isLocked = false;
    bool isDragging = false;
    Vector3 offset = Vector3.zero;
    float dragTriggerDistance = 0.4f;

    // collections **************************************************
    private List<List<GameObject>> board = new List<List<GameObject>>();
    private List<GameObject> spawnPoints = new List<GameObject>();
    private List<GameObject> gems = new List<GameObject>();

    void Start()
    {
        CreateBoard();
        StartCoroutine(InitializeGems());
    }

    void Update()
    {
        HandleGemTouch();
    }

    void CreateBoard()
    {
        for (int i = 0; i < rows; i++)
        {
            List<GameObject> row = new List<GameObject>();

            for (int j = 0; j < columns; j++)
            {
                Vector3 pos = new Vector3(LEFT_MARGIN + j * CELL_GAP, TOP_MARGIN + i * CELL_GAP, 1);

                if (i == rows - 1)
                {
                    GameObject spawnPoint = new GameObject("SpawnPoint_" + j);
                    Vector3 spawnPos = new Vector3(LEFT_MARGIN + j * CELL_GAP, TOP_MARGIN + (i * CELL_GAP) + 1.5f, 0);
                    spawnPoint.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
                    spawnPoints.Add(spawnPoint);
                }

                GameObject slot = Instantiate(slotPrefab, pos, Quaternion.identity);
                slot.name = $"{i}, {j}";
                row.Add(slot);
            }
            board.Add(row);
        }
    }

    void SpawnGem(int row, int col)
    {
        int idx = UnityEngine.Random.Range(0, gemPrefabs.Length);
        GameObject gemPrefab = gemPrefabs[idx];
        GameObject spawnPoint = spawnPoints[col];

        GameObject targetSlot = board[row][col];
        Debug.Log($"targetSlot {targetSlot.transform.position}");

        GameObject gemGO = Instantiate(gemPrefab, spawnPoint.transform.position, Quaternion.Euler(-90, 0, 0));
        Gem gem = gemGO.GetComponent<Gem>();
        gem.name = $"{gem.name.Replace("(Clone)", "")}";
        gem.color = gem.name.Split("-")[1];
        gem.row = row;
        gem.col = col;
        gem.SetInitialY(targetSlot.transform.position.y);
        gems.Add(gemGO);
    }

    void HandleGemTouch()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (!Physics.Raycast(ray, out hit)) return;

                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                if (rb == null) return;

                Gem clickedGem = rb.GetComponent<Gem>();
                if (clickedGem.IsMoving()) return;

                currentRigidbody = rb;
                isDragging = true;

                Vector3 touchStartPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchStartPos.z = 0;

                offset = currentRigidbody.position - touchStartPos;

                Debug.Log($"Gem {clickedGem.name}");
                // Debug.Log($"Touch position: {convertedTouchPos} currentRigidbody position: {currentRigidbody.position} Offset: {offset}");
                break;

            case TouchPhase.Moved:
                if (!isDragging || isLocked || currentRigidbody == null) return;

                Gem gem = currentRigidbody.GetComponent<Gem>();

                Vector3 startPos = currentRigidbody.position + offset;
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;

                if (Vector3.Distance(startPos, touchPos) <= dragTriggerDistance) return;

                float angleRad = Mathf.Atan2(touchPos.y - startPos.y, touchPos.x - startPos.x);
                float angleDeg = angleRad * (180 / Mathf.PI);

                // Reset everything
                isDragging = false;
                currentRigidbody = null;
                offset = Vector3.zero;
                isLocked = true;

                Nullable<Direction> dir = null;
                if (gem.row > 0 && angleDeg >= -135 && angleDeg < -45)
                {
                    dir = Direction.Down;
                }
                else if (gem.col < columns - 1 && angleDeg >= -45 && angleDeg < 45)
                {
                    dir = Direction.Right;
                }
                else if (gem.row < rows - 1 && angleDeg >= 45 && angleDeg < 135)
                {
                    dir = Direction.Up;
                }
                else if (gem.col > 0 && angleDeg >= 135 || angleDeg < -135)
                {
                    dir = Direction.Left;
                }

                if (dir == null) return;

                int gemIdx = gem.row * columns + gem.col;
                int otherIdx = -1;
                switch (dir)
                {
                    case Direction.Up:
                        otherIdx = gemIdx + columns;
                        break;
                    case Direction.Right:
                        otherIdx = gemIdx + 1;
                        break;
                    case Direction.Down:
                        otherIdx = gemIdx - columns;
                        break;
                    case Direction.Left:
                        otherIdx = gemIdx - 1;
                        break;
                }

                GameObject thisGem = gems[gemIdx];
                GameObject otherGem = gems[otherIdx];

                GemSwap(thisGem, gemIdx, otherGem, otherIdx, (Direction)dir);

                // gems[gemIdx] = otherGem;
                // gems[otherIdx] = thisGem;

                bool hasCombo = CheckBoard();

                if (hasCombo)
                {
                    // is 3 combo
                    // explode found sequences
                    // handle falling
                    // handle new gem spawning
                }
                else
                {
                    Direction backDir;
                    switch (dir)
                    {
                        case Direction.Up:
                            backDir = Direction.Down;
                            break;
                        case Direction.Right:
                            backDir = Direction.Left;
                            break;
                        case Direction.Down:
                            backDir = Direction.Up;
                            break;
                        case Direction.Left:
                        default:
                            backDir = Direction.Right;
                            break;
                    }
                    // move gem back
                    StartCoroutine(GemSwapBack(thisGem, gemIdx, otherGem, otherIdx, backDir));
                }

                break;
                // case TouchPhase.Ended:
                //     isDragging = false;
                //     currentRigidbody = null;
                //     offset = Vector3.zero;
                //     break;
        }
    }

    void GemSwap(GameObject gem, int gemIdx, GameObject other, int otherIdx, Direction direction)
    {
        Debug.Log($"Gem: {gem.name} Other: {other.name} Direction: {direction}");
        Gem thisGem = gem.GetComponent<Gem>();
        Gem otherGem = other.GetComponent<Gem>();

        thisGem.Move(other.transform.position);
        otherGem.Move(gem.transform.position);

        (otherGem.row, thisGem.row) = (thisGem.row, otherGem.row);
        (otherGem.col, thisGem.col) = (thisGem.col, otherGem.col);

        gems[gemIdx] = other;
        gems[otherIdx] = gem;

        gems = gems.OrderBy(g => g.GetComponent<Gem>().row).ThenBy(g => g.GetComponent<Gem>().col).ToList();

    }

    bool CheckBoard()
    {
        Debug.Log("CheckBoard");

        List<Gem> gemsToRemove = new List<Gem>();
        List<Gem> currSequence = new List<Gem>();

        List<List<Gem>> rowGems = new List<List<Gem>>();
        List<List<Gem>> colGems = new List<List<Gem>>();


        // initialize rowGems / colGems
        for (int i = 0; i < rows; i++)
        {
            rowGems.Add(new List<Gem>());
        }
        for (int i = 0; i < columns; i++)
        {
            colGems.Add(new List<Gem>());
        }


        // assign gems to rowGems / colGems
        int tempRow = 0;
        int tempCol = 0;
        for (int i = 0; i < gems.Count; i++)
        {
            Gem currGem = gems[i].GetComponent<Gem>();

            rowGems[tempRow].Add(currGem);
            colGems[tempCol].Add(currGem);
            // Debug.Log($"i {i} tempRow {tempRow} tempCol {tempCol} color {currGem.color}");

            int lastColIdx = columns - 1;
            if (tempCol >= lastColIdx)
            {
                tempCol = 0;
                tempRow++;
            }
            else
            {
                tempCol++;
            }
        }

        for (int ri = 0; ri < rowGems.Count; ri++)
        {
            for (int rj = 0; rj < rowGems[ri].Count; rj++)
            {
                Gem rowItem = rowGems[ri][rj];
                Debug.Log($"CheckBoard ::: Rows -> {rowItem.color} :: Row {rowItem.row} Col {rowItem.col}");
            }
        }
        for (int ci = 0; ci < colGems.Count; ci++)
        {
            for (int cj = 0; cj < colGems[ci].Count; cj++)
            {
                Gem colItem = colGems[ci][cj];
                Debug.Log($"CheckBoard ::: Cols -> {colItem.color} :: Col {colItem.col} Row {colItem.row}");
            }
        }



        // for (int i = 0; i < gems.Count; i++)
        // {
        //     GameObject gemGO = gems[i];
        //     Gem gem = gemGO.GetComponent<Gem>();
        //     Debug.Log($"CheckBoard {gem.color} :: {gem.row} {gem.col}");
        // }

        return false;
    }

    IEnumerator GemSwapBack(GameObject gem, int gemIdx, GameObject other, int otherIdx, Direction direction)
    {
        yield return new WaitForSeconds(1.0f);

        GemSwap(gem, gemIdx, other, otherIdx, direction);
        isLocked = false;

    }

    IEnumerator InitializeGems()
    {
        while (_row < rows)
        {
            SpawnGem(_row, _col);

            yield return new WaitForSeconds(0.05f);

            int lastColIdx = columns - 1;
            if (_col >= lastColIdx)
            {
                _col = 0;
                _row++;
            }
            else
            {
                _col++;
            }

            Debug.Log($"InitializeGems ::: {_row}::{_col}");
        }
        Debug.Log("Gem initialization finished!");
    }
}
