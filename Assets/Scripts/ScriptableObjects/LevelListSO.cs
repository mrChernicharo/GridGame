using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "Scriptable Object/LevelList")]
public class LevelListSO : ScriptableObject
{
    public LevelSO[] levels;
}