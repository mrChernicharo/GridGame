using UnityEngine;

public class Board2 : MonoBehaviour
{
    private int rows;
    private int cols;
    private float tileSize = 0.42f;

    [HideInInspector] public Tile[,] tiles;
    [HideInInspector] public GameObject[] spawnPoints;
    [SerializeField] private GameObject tilePrefab;

    void Start()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10f));
        // Debug.Log($"bottomLeft {bottomLeft} topRight {topRight} ::: cameraPos {Camera.main.transform.position}");

        LevelSO levelSO = LevelLoader.levelToLoad;
        if (levelSO == null)
        {
            Debug.LogError("Board2 ::: failed to reference LevelSO out of LevelLoader.levelToLoad");
            return;
        }

        rows = levelSO.rows;
        cols = levelSO.columns;
        spawnPoints = new GameObject[cols];
        tiles = new Tile[rows, cols];

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
                }
            }
        }
    }

    public Tile GetTile(int row, int col)
    {
        return tiles[row, col];
    }

}
