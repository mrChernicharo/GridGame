using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine;

public class BoardChecker : MonoBehaviour
{
    [SerializeField] private Board2 board;
    [SerializeField] private float timeToCheck = 0.5f;


    public static event EventHandler<MoveGemsBackEventArgs> MoveGemsBack;


    private float timeSinceGemPlaced = 0f;
    private bool shouldCheck = false;


    private void OnEnable()
    {
        Gem2.GemPlaced += OnGemPlaced;
    }

    private void OnDisable()
    {
        Gem2.GemPlaced -= OnGemPlaced;
    }

    private void OnGemPlaced(object sender, GemPlacedEventArgs ev)
    {
        timeSinceGemPlaced = 0f;
        shouldCheck = true;
    }

    private List<Gem2> CheckBoard()
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
        for (int i = 0; i < board.gems.Length; i++)
        {
            Gem2 currGem = board.gems[tempRow, tempCol].GetComponent<Gem2>();

            rowGems[tempRow].Add(currGem);
            colGems[tempCol].Add(currGem);
            // Debug.Log($"i {i} tempRow {tempRow} tempCol {tempCol} color {currGem.color}");

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

        // Debug.Log($"****** gemsToRemove ***** {gemsToRemove.Count}");
        // foreach (Gem2 g in gemsToRemove)
        // {
        //     Tile t = board.GetTileFromPosition(g.transform.position);
        //     Debug.Log($"{g.gemDetails.color} {t.row} {t.col}");
        // }

        // return new BoardResult(rowGems, colGems, gemsToRemove);
        return gemsToRemove;

    }

    async void Update()
    {
        timeSinceGemPlaced += Time.deltaTime;

        if (shouldCheck && timeSinceGemPlaced > timeToCheck)
        {
            shouldCheck = false;
            List<Gem2> gemsToRemove = CheckBoard();

            if (gemsToRemove.Count == 0)
            {
                MoveGemsBack.Invoke(this, new MoveGemsBackEventArgs());
            }
            else { }
        }
    }





}


public class MoveGemsBackEventArgs : System.EventArgs
{
    // public Gem2 gem1;
    // public Gem2 gem2;
    // public Vector2 pos1;
    // public Vector2 pos2;
    // public MoveGemsBackEventArgs(Gem2 gem1, Gem2 gem2)

    public MoveGemsBackEventArgs()
    {
        // this.pos1 = gem1.transform.position;
        // this.pos2 = gem2.transform.position;
        // this.gem1 = gem1;
        // this.gem2 = gem2;
    }
}