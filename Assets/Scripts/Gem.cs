using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private bool isInitializing = true;
    private float initialY;
    private float speed = 0f;

    public int row;
    public int col;


    // *************
    private bool isMoving = false;
    private Vector3 destination;
    [SerializeField] private float moveSpeed;

    [SerializeField] private float fallAcceleration;
    [SerializeField] private float maxSpeed;

    public void SetInitialY(float y)
    {
        initialY = y;
    }

    public void Move(Vector3 _destination)
    {
        isMoving = true;
        destination = _destination;
    }

    void Start()
    {

    }

    void Update()
    {

        Vector3 pos = gameObject.transform.position;

        if (isInitializing)
        {
            if (pos.y > initialY)
            {

                speed += fallAcceleration * Time.deltaTime;
                pos.y -= Mathf.Min(speed, maxSpeed);
                gameObject.transform.position = pos;
            }
            else
            {
                isInitializing = false;
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
                Debug.Log("Done!");
                isMoving = false;
                destination = Vector3.zero;
            }
        }

    }

}
