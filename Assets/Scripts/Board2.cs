using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Text;

public class Board2 : MonoBehaviour
{
    public int rows;
    public int cols;
    public static float tileSize = 0.42f;
    public bool isLocked = false;

    [HideInInspector] public Tile[,] tiles;
    [HideInInspector] public GameObject[,] gems;
    [HideInInspector] public GameObject[] spawnPoints;
    [SerializeField] private GameObject tilePrefab;

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
        Tile tile = GetTileFromPosition(ev.position);

        GameObject gem = sender as GameObject;
        gems[tile.row, tile.col] = gem;

        // Debug.Log($"*** Board received 'GemPlaced' event! *** color: {ev.color} position: {ev.position}, tile: {tile.row} {tile.col}, gemObj ::: {gem.name}");
    }

    public async Task InitializeBoard()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10f));
        // Debug.Log($"bottomLeft {bottomLeft} topRight {topRight} ::: cameraPos {Camera.main.transform.position}");

        LevelSO levelSO = LevelLoader.currentLevel;
        if (levelSO == null)
        {
            Debug.LogError("Board2 ::: failed to reference LevelSO out of LevelLoader.levelToLoad");
            return;
        }

        rows = levelSO.rows;
        cols = levelSO.columns;

        tiles = new Tile[rows, cols];
        gems = new GameObject[rows, cols];
        spawnPoints = new GameObject[cols];

        float startX = (cols - 1) * -tileSize * 0.5f;
        float startY = bottomLeft.y + 1f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var pos = new Vector3(startX + (j * tileSize), startY + (i * tileSize), 0);
                GameObject tileGO = Instantiate(tilePrefab, pos, Quaternion.identity, this.transform);
                Tile tile = tileGO.GetComponent<Tile>();
                tile.row = i;
                tile.col = j;
                tiles[i, j] = tile;
                // Debug.Log($"row:{i} col:{j}, tilePos: {tile.GetPosition()}");

                if (i == rows - 1)
                {
                    GameObject spawnPoint = new GameObject("SpawnPoint");
                    var spawnPos = new Vector3(pos.x, pos.y + 1f, 0);
                    spawnPoint.transform.position = spawnPos;
                    spawnPoint.transform.SetParent(this.transform);
                    spawnPoints[j] = spawnPoint;
                }
            }
        }

        await Task.Delay(500);
    }


    public Tile GetTileFromPosition(Vector2 pos)
    {
        Tile selectedTile = null;
        float minDist = float.PositiveInfinity;

        foreach (Tile t in tiles)
        {
            float dist = Vector2.Distance(t.GetPosition(), pos);
            if (dist < minDist)
            {
                minDist = dist;
                selectedTile = t;
            }
        }
        return selectedTile;
    }

    public Tile GetTile(int row, int col)
    {
        return tiles[row, col];
    }

    public GemColor GetRandomGemColor()
    {
        var colors = Enum.GetValues(typeof(GemColor));
        int idx = UnityEngine.Random.Range(0, colors.Length);

        return (GemColor)colors.GetValue(idx);
    }

    public void LogGrid()
    {
        StringBuilder logBuilder = new StringBuilder();
        logBuilder.AppendLine("--- Gem Grid Layout ---");
        for (int r = rows - 1; r >= 0; r--)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject gem = gems[r, c];
                string cellRepresentation;
                if (gem != null)
                {
                    GemColor color = gem.GetComponent<Gem2>().gemDetails.color;
                    cellRepresentation = color.ToString()[..3] + " ";
                }
                else
                {
                    cellRepresentation = "_ ";
                }
                logBuilder.Append(cellRepresentation);
            }
            logBuilder.AppendLine();
        }
        logBuilder.AppendLine("--- End of Grid ---");
        Debug.Log(logBuilder.ToString());
    }
}
