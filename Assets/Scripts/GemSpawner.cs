using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;

// GemAmber
//    

public class GemSpawner : MonoBehaviour
{
    private Dictionary<GemColor, GameObject> gemDict;
    [SerializeField] private GameObject[] gems;
    [SerializeField] private Board board;

    [Tooltip("in milliseconds")]
    [SerializeField] private int spawnInterval = 40;

    void OnEnable()
    {
        BoardChecker.SpawnGem += OnSpawnGem;
    }

    void OnDisable()
    {
        BoardChecker.SpawnGem -= OnSpawnGem;
    }

    void Start()
    {
        // Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10f));
        // Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10f));
        // Debug.Log($"bottomLeft {bottomLeft} topRight {topRight} ::: cameraPos {Camera.main.transform.position}");

        gemDict = new Dictionary<GemColor, GameObject>();

        for (int i = 0; i < gems.Length; i++)
        {
            Gem2 gemScript = gems[i].GetComponent<Gem2>();
            GemSO gemDetails = gemScript.gemDetails;

            gemDict.Add(gemDetails.color, gems[i]);
        }
    }

    protected virtual void OnSpawnGem(object sender, SpawnGemEventArgs ev)
    {
        SpawnGem(ev.color, ev.spawnPos, ev.targetYPos);
    }

    public GameObject SpawnGem(GemColor color, Vector3 spawnPosition, float targetYPos = -6f)
    {
        GameObject gem;
        gemDict.TryGetValue(color, out gem);
        if (!gem)
        {
            Debug.LogError($">>>> SpawnGem ERROR : color:{color} : position:{spawnPosition}, gem:{gem}");
            throw new Exception();
        }

        GameObject gemInstance = Instantiate(gem, new Vector3(spawnPosition.x, spawnPosition.y, 1f), Quaternion.identity, this.transform);
        Gem2 gemBehavior = gemInstance.GetComponent<Gem2>();
        gemBehavior.SetYTarget(targetYPos);

        return gemInstance;
    }

    public async Task InitializeGems()
    {
        if (board.tiles == null)
        {
            Debug.Log($"GameManager ::: RunGameStartSequence ERROR, no board.tiles");
        }
        else
        {
            foreach (Tile tile in board.tiles)
            {
                await Task.Delay(spawnInterval);

                GemColor color = board.GetRandomGemColor();
                if (!CanSpawnGem(color, tile))
                {
                    color = board.GetRandomGemColor();
                }

                GameObject spawnPoint = board.spawnPoints[tile.col];
                Vector3 spawnPos = spawnPoint.transform.position;

                // gotta assign board.gems here otherwise CanSpawnGem would fail
                board.gems[tile.row, tile.col] = SpawnGem(color, spawnPos, tile.GetPosition().y);
            }

        }
    }



    bool CanSpawnGem(GemColor color, Tile tile)
    {
        // Debug.Log($"CanSpawnGem {_row} {_col} gems:{gems.Count}");

        if (tile.col >= 2)
        {
            Gem2 leftNeighbor = board.gems[tile.row, tile.col - 1].GetComponent<Gem2>();
            Gem2 leftNeighbor2 = board.gems[tile.row, tile.col - 2].GetComponent<Gem2>();

            if (leftNeighbor.gemDetails.color == color && leftNeighbor2.gemDetails.color == color)
                return false;
        }
        if (tile.row >= 2)
        {
            Gem2 bottomNeighbor = board.gems[tile.row - 1, tile.col].GetComponent<Gem2>();
            Gem2 bottomNeighbor2 = board.gems[tile.row - 2, tile.col].GetComponent<Gem2>();

            if (bottomNeighbor.gemDetails.color == color && bottomNeighbor2.gemDetails.color == color)
                return false;
        }
        return true;
    }
}