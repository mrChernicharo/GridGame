using UnityEngine;

[CreateAssetMenu(fileName = "Gem", menuName = "Scriptable Object/Gem")]
public class GemSO : ScriptableObject
{
    public GemColor color;
    public GameObject explosionEffect;
    public Sprite sprite;
}
