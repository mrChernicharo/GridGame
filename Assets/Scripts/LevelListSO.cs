using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "ScriptableObject/LevelList")]
public class LevelListSO : ScriptableObject
{
    public LevelSO[] levels;
}