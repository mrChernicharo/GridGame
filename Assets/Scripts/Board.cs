using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.SearchService;
using UnityEngine;


enum Direction
{
    Up, Down, Left, Right
}

public enum GemColor
{
    Amber, Emerald, Fucsia, Ruby, Saphire, Turquoise,
}

struct BoardResult
{
    public List<List<Gem>> rowGems;
    public List<List<Gem>> colGems;
    public List<Gem> gemsToRemove;

    public BoardResult(List<List<Gem>> rg, List<List<Gem>> cg, List<Gem> gtr)
    {
        rowGems = rg;
        colGems = cg;
        gemsToRemove = gtr;
    }
}

struct GemColorAndPrefab
{
    public GemColor color;
    public GameObject prefab;

    public GemColorAndPrefab(GameObject _prefab, GemColor _color)
    {
        color = _color;
        prefab = _prefab;
    }
}

public class Board : MonoBehaviour
{
    static public float CELL_GAP = 0.55f;
    [SerializeField] private float LEFT_MARGIN; // -1.885
    [SerializeField] private float TOP_MARGIN; // -1.885
    [SerializeField] private int rows;
    [SerializeField] private int columns;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject[] gemPrefabs;
    [SerializeField] private GameObject[] explosionEfx;

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
            List<GameObject> boardRow = new List<GameObject>();

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
                boardRow.Add(slot);
            }
            board.Add(boardRow);
        }
    }

    GemColorAndPrefab PickRandomGemColorAndPrefab()
    {
        int RandPrefabIdx = UnityEngine.Random.Range(0, gemPrefabs.Length);

        GameObject gemPrefab = gemPrefabs[RandPrefabIdx];

        string colorName = gemPrefab.GetComponent<Renderer>().sharedMaterial.name;
        GemColor color = (GemColor)Enum.Parse(typeof(GemColor), colorName);
        // Debug.Log($"PickRandomGemColorAndPrefab ::: colorName: {color}");

        return new GemColorAndPrefab(gemPrefab, color);
    }


    void SpawnGem(GameObject prefab, int row_, int col_)
    {
        GameObject spawnPoint = spawnPoints[col_];
        GameObject gemGO = Instantiate(prefab, spawnPoint.transform.position, Quaternion.Euler(-90, 0, 0));
        Gem gem = gemGO.GetComponent<Gem>();

        gem.row = row_;
        gem.col = col_;

        GameObject targetSlot = board[gem.row][gem.col];

        gem.SetInitialY(targetSlot.transform.position.y);
        gem.UpdateText();

        Debug.Log($"color {gem.color} col {col_} initialY {targetSlot.transform.position.y}");

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

                BoardResult boardResult = CheckBoard();


                // pluck gems to remove
                if (boardResult.gemsToRemove.Count > 0)
                {
                    StartCoroutine(SpawnNewGems(boardResult));
                }
                else
                {
                    var backDir = dir switch
                    {
                        Direction.Up => Direction.Down,
                        Direction.Right => Direction.Left,
                        Direction.Down => Direction.Up,
                        _ => Direction.Right,
                    };
                    // move gem back
                    StartCoroutine(GemSwapBack(thisGem, gemIdx, otherGem, otherIdx, backDir));
                }

                break;
                // case TouchPhase.Ended:
        }
    }

    void GemSwap(GameObject gem, int gemIdx, GameObject other, int otherIdx, Direction direction)
    {
        Debug.Log($"::: GemSwap ::: Gem: {gem.GetComponent<Gem>().color} Other: {other.GetComponent<Gem>().color} Direction: {direction}");
        Gem thisGem = gem.GetComponent<Gem>();
        Gem otherGem = other.GetComponent<Gem>();

        thisGem.Move(other.transform.position);
        otherGem.Move(gem.transform.position);

        (otherGem.row, thisGem.row) = (thisGem.row, otherGem.row);
        (otherGem.col, thisGem.col) = (thisGem.col, otherGem.col);

        thisGem.UpdateText();
        otherGem.UpdateText();

        gems[gemIdx] = other;
        gems[otherIdx] = gem;

        gems = gems.OrderBy(g => g.GetComponent<Gem>().row).ThenBy(g => g.GetComponent<Gem>().col).ToList();

    }

    BoardResult CheckBoard()
    {
        // Debug.Log("CheckBoard");

        List<List<Gem>> rowGems = new List<List<Gem>>();
        List<List<Gem>> colGems = new List<List<Gem>>();


        // initialize rowGems / colGems
        for (int i = 0; i < rows; i++) rowGems.Add(new List<Gem>());

        for (int i = 0; i < columns; i++) colGems.Add(new List<Gem>());


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


        List<Gem> gemsToRemove = new List<Gem>();
        List<Gem> currSequence = new List<Gem>();

        for (int i = 0; i < rowGems.Count; i++)
        {
            List<Gem> currRow = rowGems[i];

            for (int j = 0; j < currRow.Count; j++)
            {
                Gem item = currRow[j];
                bool isFirstItem = j == 0;
                bool isLastItem = j == currRow.Count - 1;
                if (isFirstItem)
                {
                    currSequence.Add(item);
                }
                else
                {
                    Gem prev = currRow[j - 1];

                    if (prev.color == item.color)
                    {
                        currSequence.Add(item);
                    }

                    if (prev.color != item.color || isLastItem)
                    {
                        if (currSequence.Count >= 3)
                        {
                            // Debug.Log("COMBO! COMBO!");
                            foreach (Gem obj in currSequence) gemsToRemove.Add(obj);
                        }

                        currSequence.Clear();
                        if (!isLastItem) currSequence.Add(item);
                    }
                }
            }
        }

        for (int i = 0; i < colGems.Count; i++)
        {
            List<Gem> currCol = colGems[i];

            for (int j = 0; j < currCol.Count; j++)
            {
                Gem item = currCol[j];
                bool isFirstItem = j == 0;
                bool isLastItem = j == currCol.Count - 1;
                if (isFirstItem)
                {
                    currSequence.Add(item);
                }
                else
                {
                    Gem prev = currCol[j - 1];

                    if (prev.color == item.color)
                    {
                        currSequence.Add(item);
                    }

                    if (prev.color != item.color || isLastItem)
                    {
                        if (currSequence.Count >= 3)
                        {
                            // Debug.Log("COMBO! COMBO!");
                            foreach (Gem obj in currSequence) gemsToRemove.Add(obj);
                        }

                        currSequence.Clear();
                        if (!isLastItem) currSequence.Add(item);
                    }
                }
            }
        }

        if (gemsToRemove.Count > 0) Debug.Log("=========================================");

        gemsToRemove = gemsToRemove.Distinct().ToList();

        string removeMsg = "";
        foreach (Gem g in gemsToRemove)
        {
            removeMsg.Concat($"** Gem to remove ** {g.color} {g.row} {g.col} \n");
        }
        Debug.Log(removeMsg);

        BoardResult result = new BoardResult(rowGems, colGems, gemsToRemove);
        return result;
    }

    IEnumerator SpawnNewGems(BoardResult br)
    {
        while (br.gemsToRemove.Count > 0)
        {

            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < br.colGems.Count; i++)
            {
                int colDestroyCount = 0;
                List<Gem> gemsInCol = br.colGems[i];

                yield return new WaitForSeconds(0.01f);

                for (int j = 0; j < gemsInCol.Count; j++)
                {


                    yield return new WaitForSeconds(0.01f);

                    Gem currGem = gemsInCol[j];
                    int gIdx = currGem.row * columns + currGem.col;
                    GameObject gGO = gems[gIdx];

                    if (gGO.GetComponent<Gem>() != currGem) Debug.LogError("Houston!");


                    if (br.gemsToRemove.Contains(currGem))
                    {
                        br.gemsToRemove.Remove(currGem);

                        GameObject explosion = Instantiate(explosionEfx[currGem.prefabIdx], currGem.transform.position, Quaternion.identity);
                        explosion.GetComponent<ParticleSystem>().Play();

                        // destroy combo'd gems
                        colDestroyCount++;
                        gems[gIdx] = null;
                        currGem.Explode();
                    }
                    else
                    {
                        if (colDestroyCount > 0)
                        {
                            // handle falling gems
                            currGem.Fall(colDestroyCount);
                            currGem.UpdateText();
                        }
                    }
                }

                // spawn new gems
                while (colDestroyCount > 0)
                {
                    colDestroyCount--;
                    int spawnRow = gemsInCol.Count - 1 - colDestroyCount;
                    // Debug.Log($"spawn row {spawnRow}");

                    yield return new WaitForSeconds(0.05f);
                    GemColorAndPrefab picked = PickRandomGemColorAndPrefab();
                    SpawnGem(picked.prefab, spawnRow, i);
                }
            }


            // update gems' rows & cols
            gems = gems.Where(g => g != null).OrderBy(g => g.GetComponent<Gem>().row).ThenBy(g => g.GetComponent<Gem>().col).ToList();
            // gems.ForEach(g => g.GetComponent<Gem>().PrintInfo());

            yield return new WaitForSeconds(0.1f);

            br = CheckBoard();
        }

        isLocked = false;
    }

    IEnumerator GemSwapBack(GameObject gem, int gemIdx, GameObject other, int otherIdx, Direction direction)
    {
        yield return new WaitForSeconds(1.0f);

        GemSwap(gem, gemIdx, other, otherIdx, direction);
        isLocked = false;

    }

    bool CanSpawnGem(GemColor color)
    {
        Debug.Log($"CanSpawnGem {_row} {_col} gems:{gems.Count}");

        if (_col >= 2)
        {
            Gem leftNeighbor = gems[_row * columns + (_col - 1)].GetComponent<Gem>();
            Gem leftNeighbor2 = gems[_row * columns + (_col - 2)].GetComponent<Gem>();

            if (leftNeighbor.color == color && leftNeighbor2.color == color)
                return false;
        }
        if (_row >= 2)
        {
            Gem bottomNeighbor = gems[(_row - 1) * columns + _col].GetComponent<Gem>();
            Gem bottomNeighbor2 = gems[(_row - 2) * columns + _col].GetComponent<Gem>();

            if (bottomNeighbor.color == color && bottomNeighbor2.color == color)
                return false;
        }
        return true;
    }

    IEnumerator InitializeGems()
    {
        while (_row < rows)
        {
            yield return new WaitForSeconds(0.025f);

            // prevent combos at beginning
            GemColorAndPrefab picked = PickRandomGemColorAndPrefab();
            while (!CanSpawnGem(picked.color))
            {
                picked = PickRandomGemColorAndPrefab();
            }

            SpawnGem(picked.prefab, _row, _col);

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
            // Debug.Log($"InitializeGems ::: {_row}::{_col}");
        }
        // Debug.Log("Gem initialization finished!");
    }
}


