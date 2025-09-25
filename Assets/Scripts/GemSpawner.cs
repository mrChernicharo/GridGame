using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] gems;
    // private Dictionary<GemColor, GemSO> gemDict;

    void Start()
    {
        // Camera.main.transform.position.z;
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10f));
        Debug.Log($"bottomLeft {bottomLeft} topRight {topRight} ::: cameraPos {Camera.main.transform.position}");
    }

    // void Start()
    // {
    // gemDict = new Dictionary<GemColor, GemSO>();

    // for (int i = 0; i < gems.Length; i++)
    // {
    //     GemSO gem = gems[i];
    //     gemDict.Add(gem.color, gem);
    //     Debug.Log(i);
    //     Debug.Log(gem.color);
    // }
    // }

    public void SpawnGem(GemColor color, Vector3 position)
    {
        Debug.Log($">>>> SpawnGem : {color} : {position}");

        Instantiate(gems[0], new Vector3(position.x, position.y, 1f), Quaternion.identity);
        // GemSO gemSO = null;
        // gemDict.TryGetValue(color, out gemSO);


        // if (gemSO != null)
        // {
        // GameObject newGem = gems[0];
        // newGem.transform.position = new Vector3(position.x, position.y, 1f);

        // SpriteRenderer sr = newGem.AddComponent<SpriteRenderer>();
        // // sr.spriteSortPoint = SpriteSortPoint.Center;
        // sr.sprite = gemSO.sprite;
        // sr.sortingOrder = Mathf.RoundToInt(newGem.transform.position.y * -10000);
        // }

    }

}