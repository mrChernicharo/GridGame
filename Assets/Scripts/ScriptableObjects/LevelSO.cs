using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class LevelSO : ScriptableObject
{
    public string name;
    public int rows;
    public int columns;
    public GemColor[] gemColors;
}
