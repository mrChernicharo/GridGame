using System;
using UnityEngine;


public class Gem2 : MonoBehaviour
{
    private float yTarget = -6f;
    private float speed = 0f;
    private bool isMoving = false;
    private bool isFalling = true;

    // ************

    public static event EventHandler<GemPlacedEventArgs> GemPlaced;

    // *************
    private Vector3 destination;
    public GemSO gemDetails;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float fallAcceleration;
    [SerializeField] private float maxSpeed;


    protected virtual void OnGemPlaced()
    {
        if (GemPlaced == null) return;

        GemPlaced.Invoke(this.gameObject, new GemPlacedEventArgs(gemDetails.color, gameObject.transform.position));
    }


    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void Update()
    {

        Vector3 pos = gameObject.transform.position;

        if (isFalling)
        {
            if (pos.y > yTarget)
            {

                speed += fallAcceleration * Time.deltaTime;
                pos.y -= Mathf.Min(speed, maxSpeed);
                gameObject.transform.position = pos;
            }
            else
            {
                speed = 0f;
                isFalling = false;
                Vector3 posCopy = gameObject.transform.position;
                gameObject.transform.position = new Vector3(posCopy.x, yTarget, posCopy.z);
                OnGemPlaced();
            }
            return;
        }

        if (isMoving)
        {
            if (Vector3.Distance(gameObject.transform.position, destination) > 0.001f)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destination, moveSpeed * Time.deltaTime);
            }
            else
            {
                // Debug.Log("Done!");
                isMoving = false;
                destination = Vector3.zero;
                OnGemPlaced();

            }
        }

    }


    public bool IsMoving()
    {
        return isMoving;
    }

    public void SetYTarget(float y)
    {
        yTarget = y;
    }

    public void Move(Vector3 _destination)
    {
        isMoving = true;
        destination = _destination;
    }

    public void Explode()
    {
        Destroy(gameObject);
    }

    public void Fall(int newYPosition)
    {
        yTarget = newYPosition;
        isFalling = true;
    }


}



public class GemPlacedEventArgs : System.EventArgs
{
    public GemColor color;
    public Vector2 position;

    public GemPlacedEventArgs(GemColor color, Vector2 position)
    {
        this.color = color;
        this.position = position;
    }
}




// 0 => -3.5
// 1 => -2.75
// 2 => -2.0
// 3 => -1.25
// 4 => -0.5
// 5 => 0.25
// 6 => 1.0
// 7 => 1.75
// 8 => 2.5