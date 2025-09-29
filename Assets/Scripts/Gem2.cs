using System;
using System.Threading.Tasks;
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


    private void OnGemPlaced()
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

        Vector3 posCopy = gameObject.transform.position;

        if (isFalling)
        {
            if (posCopy.y > yTarget)
            {

                Vector3 targetPos = new Vector3(posCopy.x, yTarget, posCopy.z);
                speed += fallAcceleration * Time.deltaTime;
                float moveSpeed = Mathf.Min(speed, maxSpeed);
                gameObject.transform.position = Vector3.MoveTowards(posCopy, targetPos, moveSpeed); ;
            }
            else
            {
                speed = 0f;
                isFalling = false;
                gameObject.transform.position = new Vector3(posCopy.x, yTarget, posCopy.z);
                OnGemPlaced();
            }
            return;
        }

        else if (isMoving)
        {
            if (Vector3.Distance(posCopy, destination) > 0.005f)
            {
                gameObject.transform.position = Vector3.MoveTowards(posCopy, destination, moveSpeed * Time.deltaTime);
            }
            else
            {
                // Debug.Log("Done!");
                gameObject.transform.position = destination;
                isMoving = false;
                destination = Vector3.zero;
                OnGemPlaced();

            }
        }

        // else if (yTarget != posCopy.y)
        // {
        //     Debug.Log("Adjust ypos and yTarget together");
        //     gameObject.transform.position = new Vector3(posCopy.x, yTarget, posCopy.z);
        // }

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
        ParticleSystem explosion = gemDetails.explosionEffect.GetComponent<ParticleSystem>();
        explosion.Play();

        Destroy(gameObject);
    }

    public void Fall(int fallCount)
    {
        float currYPos = gameObject.transform.position.y;
        float newYTarget = currYPos - fallCount * Board2.tileSize;

        Debug.Log($"curr yTarget {yTarget} curr yPos {currYPos} newYTarget {newYTarget}");
        SetYTarget(newYTarget);
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