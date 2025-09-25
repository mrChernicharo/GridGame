using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GemSpawner : MonoBehaviour
{
    [SerializeField] private GemSO[] gems;
    private Dictionary<GemColor, GemSO> gemDict;

    void Start()
    {
        gemDict = new Dictionary<GemColor, GemSO>();

        for (int i = 0; i < gems.Length; i++)
        {
            GemSO gem = gems[i];
            gemDict.Add(gem.color, gem);
            Debug.Log(i);
            Debug.Log(gem.color);
        }
    }

    public void SpawnGem(GemColor color, Vector3 position)
    {
        Debug.Log($">>>> SpawnGem : {color} : {position}");
        GameObject newGem = new GameObject("Gem");
        newGem.transform.position = new Vector3(position.x, position.y, 1f);

        GemSO gemSO;
        gemDict.TryGetValue(color, out gemSO);


        if (gemSO != null)
        {

            SpriteRenderer sr = newGem.AddComponent<SpriteRenderer>();
            sr.sprite = gemSO.sprite;
            Debug.Log($">>>> SpawnGem >>>> Scriptable Object: {gemSO.color} : {position}");
        }

    }

}