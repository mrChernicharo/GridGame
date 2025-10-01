using UnityEngine;

public class LevelButtons : MonoBehaviour
{
    [SerializeField] LevelListSO levelListSO;

    void OnEnable()
    {
        Debug.Log($"currentLevel: {LevelLoader.currentLevel} levelListSO: {levelListSO.levels.Length} levels");

    }




}
