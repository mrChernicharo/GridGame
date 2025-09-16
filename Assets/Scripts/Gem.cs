using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel;
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


    public void Fall(int fallCount)
    {
        int newRow = row - fallCount;
        float newYposition = -3.5f + 0.75f * newRow;


        Debug.Log($"Fall!!! {color} col {col} ::: fallCount {fallCount} ::: oldRow {row} newRow {newRow} ::: oldYPosition {yPosition} newYPosition {newYposition}");

        row = newRow;
        yPosition = newYposition;
        isFalling = true;
    }

    public void UpdateText()
    {
        TextMeshPro text = this.GetComponentInChildren<TextMeshPro>();
        if (text != null)
        {
            text.text = $"{row}-{col}";
        }
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
            return;
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

                if (pos.y < -4f)
                {
                    Debug.LogError($"Fall Error! pos.y: {pos.y} yPosition {yPosition}");
                }
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
            }
        }

    }

    public void PrintInfo()
    {
        Debug.Log($"GEM color: {color}, row: {row} col: {col}");
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