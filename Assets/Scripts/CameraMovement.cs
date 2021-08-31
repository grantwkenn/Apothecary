using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float height = 1080.0f /64.0f;
    float width = 1920 / 64;
    float halfWidth;
    float halfHeight;

    public Vector2 maxCorner;
    public Vector2 minCorner;



    public Transform target;
    public float smoothingSpeed;

    public Vector2 maxPos;
    public Vector2 minPos;

    private Vector3 targetPosition;


    // Start is called before the first frame update
    void Start()
    {
        targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        halfHeight = height / 2.0f;
        halfWidth = width / 2.0f;

        Vector3 startPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        transform.position = startPosition;

        //THIS FUNCTION MUST BE CALLED AFTER DEFAULT EXECUTION OF ROOM MANAGER.
        //SEE EXECUTION TIME IN PROJECT SETTINGS
        GameObject currentRoom = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RoomManager>().getCurrentRoom();
        minPos = currentRoom.GetComponent<RoomScript>().minPos;
        maxPos = currentRoom.GetComponent<RoomScript>().maxPos;


        maxCorner = new Vector2(transform.position.x + halfWidth, transform.position.y + halfHeight);
        minCorner = new Vector2(transform.position.x - halfWidth, transform.position.y - halfHeight);


    }

    private void Update()
    {
        maxCorner.x = transform.position.x + halfWidth;
        maxCorner.y = transform.position.y + halfHeight;

        minCorner.x = transform.position.x - halfWidth;
        minCorner.y = transform.position.y - halfHeight;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Move camera to player (in the x and y)
        if(transform.position != target.position)
        {
            targetPosition.x = target.position.x;
            targetPosition.y = target.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingSpeed * Time.deltaTime);
        }

        checkBounds();
    }

    void checkBounds()
    {
        if ((transform.position.x + halfWidth) > maxPos.x) transform.position = new Vector3(maxPos.x - halfWidth, transform.position.y, transform.position.z);
        if (transform.position.x - halfWidth < minPos.x) transform.position = new Vector3(minPos.x + halfWidth, transform.position.y, transform.position.z);
        if (transform.position.y + halfHeight > maxPos.y) transform.position = new Vector3(transform.position.x, maxPos.y - halfHeight, transform.position.z);
        if (transform.position.y - halfHeight < minPos.y) transform.position = new Vector3(transform.position.x, minPos.y + halfHeight, transform.position.z);
    }

    public void setBounds(Vector2 min, Vector2 max)
    {
        maxPos = max;
        minPos = min;
    }
}
