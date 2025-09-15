using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private bool isInitializing = true;
    private bool isFalling = true;
    private float yPosition;
    private float speed = 0f;

    public string color;

    public int row;
    public int col;


    // *************
    private bool isMoving = false;
    private Vector3 destination;
    [SerializeField] private float moveSpeed;

    [SerializeField] private float fallAcceleration;
    [SerializeField] private float maxSpeed;

    public bool IsMoving()
    {
        return isMoving;
    }

    public void SetInitialY(float y)
    {
        yPosition = y;
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


    public void Fall(int slotCount)
    {
        Debug.Log($"Fall!!! {color} ::: {slotCount} ");
        isFalling = true;
        yPosition -= slotCount * 0.75f;
    }

    void Start()
    {

    }

    void Update()
    {

        Vector3 pos = gameObject.transform.position;

        if (isInitializing)
        {
            if (pos.y > yPosition)
            {

                speed += fallAcceleration * Time.deltaTime;
                pos.y -= Mathf.Min(speed, maxSpeed);
                gameObject.transform.position = pos;
            }
            else
            {
                speed = 0f;
                isInitializing = false;
            }
        }

        if (isFalling)
        {
            if (pos.y > yPosition)
            {

                speed += fallAcceleration * Time.deltaTime;
                pos.y -= Mathf.Min(speed, maxSpeed);
                gameObject.transform.position = pos;
            }
            else
            {
                speed = 0f;
                isFalling = false;
            }
        }

        if (isMoving)
        {
            if (Vector3.Distance(gameObject.transform.position, destination) > 0.000001f)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destination, moveSpeed * Time.deltaTime);
            }
            else
            {
                // Debug.Log("Done!");
                isMoving = false;
                destination = Vector3.zero;
            }
        }

    }

}
