using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class GemPrefabEntry
{
    public GemColor color;
    public GameObject prefab;
    public int defaultPoolSize = 20;
    public int maxPoolSize = 50;

}

public class GemPoolManager : MonoBehaviour
{
    public List<GemPrefabEntry> gemPrefabEntries;
    private Dictionary<GemColor, IObjectPool<GameObject>> gemPools;

    // [SerializeField] private GameObject[] gemPrefabs;
    // public int defaultPoolSize = Board.rows * Board.columns;
    // public int maxPoolSize = Board.rows * Board.columns + 20;
    // private IObjectPool<GameObject> gemPool;


    void Awake()
    {
        gemPools = new Dictionary<GemColor, IObjectPool<GameObject>>();

        foreach (var entry in gemPrefabEntries)
        {
            var gemColor = entry.color;
            var gemPrefab = entry.prefab;
            var defaultSize = entry.defaultPoolSize;
            var maxSize = entry.maxPoolSize;

            IObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
                () => CreatePooledGem(gemPrefab),
                (gem) => gem.SetActive(true),
                (gem) => gem.SetActive(false),
                (gem) => Destroy(gem),
                true,
                defaultSize,
                maxSize
            );

            gemPools.Add(gemColor, newPool);
        }
    }



    // This is the function the pool will call to create a new gem.
    // It is a local function or lambda that captures the specific prefab.
    private GameObject CreatePooledGem(GameObject gemPrefab)
    {
        GameObject gem = Instantiate(gemPrefab);
        return gem;
    }

    // Public method to get a gem of a specific color from the pool
    public GameObject GetGem(GemColor color)
    {
        if (gemPools.TryGetValue(color, out IObjectPool<GameObject> pool))
        {
            return pool.Get();
        }
        
        Debug.LogError($"Pool for {color} not found!");
        return null;
    }

    // Public method to return a gem to its specific pool
    public void ReturnGemToPool(GameObject gem, GemColor color)
    {
        if (gemPools.TryGetValue(color, out IObjectPool<GameObject> pool))
        {
            pool.Release(gem);
        }
        else
        {
            Debug.LogError($"Pool for {color} not found! Cannot return object.");
        }
    }

    // public GameObject SpawnGem(GemColor color, int row, int col)
    // {
    //     GameObject gemGO = gemPool.Get();
    //     return gemGO;
    // }
}
