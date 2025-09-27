using UnityEngine;

public class Tile : MonoBehaviour
{
    public int row;
    public int col;
    public Vector2 GetPosition()
    {
        return (Vector2)transform.position;
    }
}
