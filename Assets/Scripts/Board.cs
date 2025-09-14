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
        gem.name = $"{gem.name.Replace("(Clone)", "")}-{col}-{row}";
        gem.SetTargetY(targetSlot.transform.position.y);

        gems.Add(gemGO);
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

    void HandleGemTouch()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        currentRigidbody = rb;
                        isDragging = true;

                        Gem clickedGem = rb.GetComponent<Gem>();
                        Debug.Log($"Gem {clickedGem.name}");

                        Vector3 touchStartPos = Camera.main.ScreenToWorldPoint(touch.position);
                        touchStartPos.z = 0;

                        offset = currentRigidbody.position - touchStartPos;
                        // Debug.Log($"Touch position: {convertedTouchPos} currentRigidbody position: {currentRigidbody.position} Offset: {offset}");
                    }
                }
                break;

            case TouchPhase.Moved:
                if (!isDragging || currentRigidbody == null) return;

                Vector3 startPos = currentRigidbody.position + offset;
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;

                float distance = Vector3.Distance(startPos, touchPos);

                if (distance <= dragTriggerDistance) return;

                float angleRad = Mathf.Atan2(touchPos.y - startPos.y, touchPos.x - startPos.x);
                float angleDeg = angleRad * (180 / Mathf.PI);
                // Debug.Log($"Dragging! {angleDeg}");

                Gem gem = currentRigidbody.GetComponent<Gem>();

                string matches = Regex.Match(gem.name, @"(\d+)-(\d+)").ToString();
                string[] coords = matches.Split('-');

                int gemCol = int.Parse(coords[0]);
                int gemRow = int.Parse(coords[1]);
                int gemIdx = gemRow * columns + gemCol;
                // Debug.Log($"matches! {matches} - x: {gemCol} y: {gemRow} gemIdx: {gemIdx}");


                // DOWN
                if (angleDeg >= -135 && angleDeg < -45)
                {
                    if (gemRow > 0)
                    {
                        GameObject other = gems[gemIdx - columns];
                        GemSwap(gems[gemIdx], other, Direction.Down);
                    }
                }
                // RIGHT
                else if (angleDeg >= -45 && angleDeg < 45)
                {
                    if (gemCol < columns - 1)
                    {
                        GameObject other = gems[gemIdx + 1];
                        GemSwap(gems[gemIdx], other, Direction.Right);
                    }
                }
                // UP
                else if (angleDeg >= 45 && angleDeg < 135)
                {
                    if (gemRow < rows - 1)
                    {
                        GameObject other = gems[gemIdx + columns];
                        GemSwap(gems[gemIdx], other, Direction.Up);
                    }
                }
                // LEFT
                else if (angleDeg >= 135 || angleDeg < -135)
                {
                    if (gemCol > 0)
                    {
                        GameObject other = gems[gemIdx - 1];
                        GemSwap(gems[gemIdx], other, Direction.Left);
                    }
                }
                break;

            case TouchPhase.Ended:
                isDragging = false;
                currentRigidbody = null;
                offset = Vector3.zero;
                break;
        }
    }


    void GemSwap(GameObject gem, GameObject other, Direction direction)
    {
        Debug.Log($"Gem: {gem.name} Other: {other.name} Direction: {direction}");
    }
}
