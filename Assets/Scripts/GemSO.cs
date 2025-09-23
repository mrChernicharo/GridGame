using UnityEngine;

[CreateAssetMenu(fileName = "Gem", menuName = "Scriptable Objects/Gem")]
public class GemSO : ScriptableObject
{
    public GemColor color;
    public GameObject explosionEffect;
    public Material material;
    // public GameObject gameObject;
    // public Sprite sprite;
}
