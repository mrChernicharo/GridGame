using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;



public class Board : MonoBehaviour
{
    [SerializeField] private float CELL_GAP;
    [SerializeField] private float LEFT_MARGIN; // -1.885
    [SerializeField] private float TOP_MARGIN; // -1.885
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject[] gemPrefabs;

    float spawnTimer = 0f;
    float SPAWN_INTERVAL = 0.5f;
    private int _col = 0;
    private int _row = 0;

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

        int idx = Random.Range(0, gemPrefabs.Length);
        GameObject gemPrefab = gemPrefabs[idx];
        GameObject spawnPoint = spawnPoints[col];

        GameObject targetSlot = board[row][col];

        Debug.Log($"targetSlot {targetSlot.transform.position}");

        GameObject gemGO = Instantiate(gemPrefab, spawnPoint.transform.position, Quaternion.Euler(-90, 0, 0));
        Gem gem = gemGO.GetComponent<Gem>();
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
}
