using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private float targetY;
    private float speed = 0f;

    [SerializeField] private float fallAcceleration;
    [SerializeField] private float maxSpeed;

    public void SetTargetY(float y)
    {
        targetY = y;
    }


    void Start()
    {

    }

    void Update()
    {

        Vector3 pos = gameObject.transform.position;

        if (pos.y > targetY)
        {
            speed += fallAcceleration * Time.deltaTime;
            pos.y -= Mathf.Min(speed, maxSpeed);
            gameObject.transform.position = pos;
        }
        else
        {

        }

    }

}
