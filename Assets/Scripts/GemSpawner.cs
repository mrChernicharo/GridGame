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

    public void SpawnGem(GemColor color, Vector3 spawnPosition, float targetYPos = -6f)
    {
        GameObject gem;
        gemDict.TryGetValue(color, out gem);
        if (!gem)
        {
            Debug.LogError($">>>> SpawnGem ERROR : color:{color} : position:{spawnPosition}, gem:{gem}");
            return;
        }

        GameObject gemInstance = Instantiate(gem, new Vector3(spawnPosition.x, spawnPosition.y, 1f), Quaternion.identity);
        Gem2 gemBehavior = gemInstance.GetComponent<Gem2>();
        gemBehavior.SetYTarget(targetYPos);
    }
}