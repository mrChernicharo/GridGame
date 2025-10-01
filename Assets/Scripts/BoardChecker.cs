using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class BoardChecker : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private float timeToCheck = 0.5f;


    public static event EventHandler<MoveGemsBackEventArgs> MoveGemsBack;
    public static event EventHandler<SpawnGemEventArgs> SpawnGem;



    private void OnEnable()
    {
        BoardInputManager.evaluateBoard += OnEvaluateBoard;
    }

    private void OnDisable()
    {
        BoardInputManager.evaluateBoard -= OnEvaluateBoard;
    }

    private void OnGemPlaced(object sender, GemPlacedEventArgs ev)
    {
        Debug.Log($"BoardChecker ::::: OnGemPlaced {ev.color}");

    }

    private async void OnEvaluateBoard(object sender, EvaluateBoardEventArgs ev)
    {
        // Debug.Log($"BoardChecker ::::: OnEvaluateBoard");
        board.LogGrid();


        BoardResult2 br = CheckBoard();
        if (br.gemsToRemove.Count == 0)
        {
            MoveGemsBack.Invoke(this, new MoveGemsBackEventArgs());
        }
        else
        {
            await WrangleGems(br);
        }

        board.UnLock();
    }

    private BoardResult2 CheckBoard()
    {
        // Debug.Log("*** Check board now! ***");
        List<List<Gem2>> rowGems = new List<List<Gem2>>();
        List<List<Gem2>> colGems = new List<List<Gem2>>();

        // initialize rowGems / colGems
        for (int i = 0; i < board.rows; i++) rowGems.Add(new List<Gem2>());
        for (int i = 0; i < board.cols; i++) colGems.Add(new List<Gem2>());


        // assign gems to rowGems / colGems
        int tempRow = 0;
        int tempCol = 0;
        foreach (GameObject gGO in board.gems)
        {
            if (gGO != null)
            {

                // GameObject gGO = board.gems[tempRow, tempCol];
                Gem2 currGem = gGO.GetComponent<Gem2>();

                rowGems[tempRow].Add(currGem);
                colGems[tempCol].Add(currGem);
                // Debug.Log($"i {i} tempRow {tempRow} tempCol {tempCol} color {currGem.color}");
            }

            int lastColIdx = board.cols - 1;
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


        List<Gem2> gemsToRemove = new List<Gem2>();
        List<Gem2> currSequence = new List<Gem2>();

        for (int i = 0; i < rowGems.Count; i++)
        {
            List<Gem2> currRow = rowGems[i];

            for (int j = 0; j < currRow.Count; j++)
            {
                Gem2 item = currRow[j];
                bool isFirstItem = j == 0;
                bool isLastItem = j == currRow.Count - 1;
                if (isFirstItem)
                {
                    currSequence.Add(item);
                }
                else
                {
                    Gem2 prev = currRow[j - 1];

                    if (prev.gemDetails.color == item.gemDetails.color)
                    {
                        currSequence.Add(item);
                    }

                    if (prev.gemDetails.color != item.gemDetails.color || isLastItem)
                    {
                        if (currSequence.Count >= 3)
                        {
                            // Debug.Log("COMBO! COMBO!");
                            foreach (Gem2 obj in currSequence) gemsToRemove.Add(obj);
                        }

                        currSequence.Clear();
                        if (!isLastItem) currSequence.Add(item);
                    }
                }
            }
        }

        for (int i = 0; i < colGems.Count; i++)
        {
            List<Gem2> currCol = colGems[i];

            for (int j = 0; j < currCol.Count; j++)
            {
                Gem2 item = currCol[j];
                bool isFirstItem = j == 0;
                bool isLastItem = j == currCol.Count - 1;
                if (isFirstItem)
                {
                    currSequence.Add(item);
                }
                else
                {
                    Gem2 prev = currCol[j - 1];

                    if (prev.gemDetails.color == item.gemDetails.color)
                    {
                        currSequence.Add(item);
                    }

                    if (prev.gemDetails.color != item.gemDetails.color || isLastItem)
                    {
                        if (currSequence.Count >= 3)
                        {
                            // Debug.Log("COMBO! COMBO!");
                            foreach (Gem2 obj in currSequence) gemsToRemove.Add(obj);
                        }

                        currSequence.Clear();
                        if (!isLastItem) currSequence.Add(item);
                    }
                }
            }
        }

        gemsToRemove = gemsToRemove.Distinct().ToList();

        return new BoardResult2(rowGems, colGems, gemsToRemove);

    }


    private async Task WrangleGems(BoardResult2 br)
    {

        List<Gem2> removing = new();
        List<FallingGem> falling = new();
        List<SpawningGem> spawning = new();

        for (int i = 0; i < br.colGems.Count; i++)
        {
            int colDestroyCount = 0;
            List<Gem2> gemsInCol = br.colGems[i];

            for (int j = 0; j < gemsInCol.Count; j++)
            {
                Gem2 currGem = gemsInCol[j];

                if (br.gemsToRemove.Contains(currGem))
                {
                    br.gemsToRemove.Remove(currGem);
                    colDestroyCount++;
                    // await Task.Delay(20);
                    // currGem.Explode();
                    removing.Add(currGem);
                }
                else
                {
                    if (colDestroyCount > 0)
                    {
                        falling.Add(new FallingGem(currGem, colDestroyCount));
                    }
                }

            }

            while (colDestroyCount > 0)
            {
                colDestroyCount--;
                int spawnRow = gemsInCol.Count - 1 - colDestroyCount;
                GameObject spawnPoint = board.spawnPoints[i];
                Tile targetTile = board.GetTile(spawnRow, i);
                spawning.Add(new SpawningGem(spawnPoint.transform.position, targetTile.GetPosition().y));
            }
        }



        foreach (Gem2 gem in removing)
        {
            await Task.Delay(30);
            gem.Explode();
        }
        await Task.Delay(200);


        foreach (FallingGem fg in falling)
        {
            await Task.Delay(30);
            fg.gem.Fall(fg.fallCount);
        }
        await Task.Delay(200);

        foreach (SpawningGem sg in spawning)
        {
            await Task.Delay(30);
            GemColor color = board.GetRandomGemColor();
            SpawnGem.Invoke(this, new SpawnGemEventArgs(color, sg.spawnPos, sg.targetYPos));
        }
        await Task.Delay(800);

        BoardResult2 br2 = CheckBoard();
        if (br2.gemsToRemove.Count > 0)
        {
            Debug.Log($"gems to remove:: {br2.gemsToRemove.Count}");
            await WrangleGems(br2);
        }
    }
}


public class MoveGemsBackEventArgs : System.EventArgs
{
}

public class SpawnGemEventArgs : System.EventArgs
{
    public GemColor color;
    public Vector3 spawnPos;
    public float targetYPos;
    public SpawnGemEventArgs(GemColor color, Vector3 spawnPos, float targetYPos)
    {
        this.color = color;
        this.spawnPos = spawnPos;
        this.targetYPos = targetYPos;
    }
}


struct BoardResult2
{
    public List<List<Gem2>> rowGems;
    public List<List<Gem2>> colGems;
    public List<Gem2> gemsToRemove;

    public BoardResult2(List<List<Gem2>> rg, List<List<Gem2>> cg, List<Gem2> gtr)
    {
        rowGems = rg;
        colGems = cg;
        gemsToRemove = gtr;
    }
}


struct FallingGem
{
    public FallingGem(Gem2 gem, int fallCount)
    {
        this.gem = gem;
        this.fallCount = fallCount;
    }

    public Gem2 gem;
    public int fallCount;
}

struct SpawningGem
{
    public SpawningGem(Vector3 spawnPos, float targetYPos)
    {
        this.spawnPos = spawnPos;
        this.targetYPos = targetYPos;
    }
    public Vector3 spawnPos;
    public float targetYPos;
}