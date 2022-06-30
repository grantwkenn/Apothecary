using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;


//REFACTOR: name this the Camera Manager

//TODO: when new resolution detected, move the camera to recenter on room. IE: from 1080p to 900p

public class CameraMovement : MonoBehaviour
{
    float heightOffset;
    float widthOffset;
    float scale;

    int pixelHeight;
    int pixelWidth;


    public Transform target;
    public float smoothingSpeed;

    public Vector2 maxPos;
    public Vector2 minPos;

    private Vector3 targetPosition;

    [SerializeField]
    UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera ppc;

    bool zoomIn;


    private void OnEnable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        //heightOffset = height / 2.0f;
        //widthOffset = width / 2.0f;

        Vector3 startPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        transform.position = startPosition;

        //THIS FUNCTION MUST BE CALLED AFTER DEFAULT EXECUTION OF ROOM MANAGER.
        //SEE EXECUTION TIME IN PROJECT SETTINGS
        GameObject currentRoom = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RoomManager>().getCurrentRoom();
        //minPos = currentRoom.GetComponent<RoomScript>().getMinPos();
        //maxPos = currentRoom.GetComponent<RoomScript>().getMaxPos();

        calculateOffsets();

        setBounds(currentRoom.GetComponent<RoomScript>().getMinPos(), currentRoom.GetComponent<RoomScript>().getMaxPos());

        ppc = this.GetComponentInParent<UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera>();
    }

    void calculateOffsets()
    {
        //calculate offsets
        pixelHeight = Camera.main.pixelHeight;
        pixelWidth = Camera.main.pixelWidth;

        //get whole integer scale
        //EXAMPLE: 900p / 360 ref --> (int) 2.5 --> 2
        /*
        scale = (pixelHeight /
            (this.GetComponent<PixelPerfectCamera>().refResolutionY));
        if (scale == 0) scale = 1;

        heightOffset = pixelHeight /
            (2 * scale * this.GetComponent<PixelPerfectCamera>().assetsPPU);

        widthOffset = pixelWidth /
            (2 * scale * this.GetComponent<PixelPerfectCamera>().assetsPPU); */

        scale = pixelHeight / 360;
        if (scale == 0) scale = 1;

        heightOffset = pixelHeight / (2 * scale * 16);

        widthOffset = pixelWidth / (2 * scale * 16); 

    }

    private void FixedUpdate()
    {
        if (zoomIn)
            cameraZoomIn();
        else
            cameraZoomOut();
    }

    private void Update()
    {
        
        
        //check for resolution change
        if(pixelHeight != Camera.main.pixelHeight || pixelWidth != Camera.main.pixelWidth)
        {
            calculateOffsets();
            
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Move camera to player (in the x and y)
        if (transform.position != target.position)
        {
            targetPosition.x = target.position.x;
            targetPosition.y = target.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingSpeed * Time.unscaledDeltaTime);
        }

        checkBounds();
    }

    void checkBounds()
    {
        if (transform.position.x + widthOffset > maxPos.x) transform.position = new Vector3(maxPos.x - widthOffset, transform.position.y, transform.position.z);
        if (transform.position.x - widthOffset < minPos.x) transform.position = new Vector3(minPos.x + widthOffset, transform.position.y, transform.position.z);
        if (transform.position.y + heightOffset > maxPos.y) transform.position = new Vector3(transform.position.x, maxPos.y - heightOffset, transform.position.z);
        if (transform.position.y - heightOffset < minPos.y) transform.position = new Vector3(transform.position.x, minPos.y + heightOffset, transform.position.z);
    }

    public void setBounds(Vector2 min, Vector2 max)
    {
        maxPos = max;
        minPos = min;

        float roomWidth = max.x - min.x;
        float roomHeight = max.y - min.y;
        
        if(((max.y-min.y)/2.0) < heightOffset)
        {
            //must center Y
            minPos.x = min.x - (widthOffset - (roomWidth / 2));
            minPos.y = min.y - (heightOffset - (roomHeight / 2));

            maxPos = max;
        }

    }


    void cameraZoomIn()
    {
        if (ppc.assetsPPU < 32)
            ppc.assetsPPU += 1;
    }

    void cameraZoomOut()
    {
        if (ppc.assetsPPU > 16)
            ppc.assetsPPU -= 1;
    }

    public void toggleZoom()
    {
        if (zoomIn)
            zoomIn = false;
        else zoomIn = true;
    }


}
