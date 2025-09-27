using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// GemAmber
//    

public class GemSpawner : MonoBehaviour
{
    private Dictionary<GemColor, GameObject> gemDict;
    [SerializeField] private GameObject[] gems;
    [SerializeField] private Board2 board;

    void Start()
    {
        // Camera.main.transform.position.z;
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10f));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10f));
        Debug.Log($"bottomLeft {bottomLeft} topRight {topRight} ::: cameraPos {Camera.main.transform.position}");

        gemDict = new Dictionary<GemColor, GameObject>();

        for (int i = 0; i < gems.Length; i++)
        {
            Gem2 gemScript = gems[i].GetComponent<Gem2>();
            GemSO gemDetails = gemScript.gemDetails;

            gemDict.Add(gemDetails.color, gems[i]);
        }
    }

    // void Start()
    // {

    // }

    public void SpawnGem(GemColor color, Vector3 position)
    {
        // Debug.Log($">>>> SpawnGem : {color} : {position}");

        GameObject gem;
        gemDict.TryGetValue(color, out gem);
        if (!gem)
        {
            Debug.LogError($">>>> SpawnGem ERROR : color:{color} : position:{position}, gem:{gem}");
            return;
        }
        Instantiate(gem, new Vector3(position.x, position.y, 1f), Quaternion.identity);
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