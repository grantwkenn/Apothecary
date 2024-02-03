using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Events;
using TMPro;
using System;
//using UnityEditor.Events;
//using UnityEngine.Experimental.Rendering.LWRP;
//using UnityEngine.Rendering.Universal;
//using UnityEngine.Experimental.Rendering.Universal;


//REFACTOR: name this the Camera Manager

//TODO: when new resolution detected, move the camera to recenter on room. IE: from 1080p to 900p

public class CameraManager : MonoBehaviour
{
    float heightOffset;
    float widthOffset;
    [SerializeField]
    int scale;

    int pixelHeight;
    int pixelWidth;

    Transform min, max;
    Transform player;
    Transform target;
    public float smoothingSpeed;
    float halfSpeed;
    float fullSpeed;

    public Vector2 maxPos;
    public Vector2 minPos;

    private Vector3 targetPosition;

    Dictionary<string, Resolution> resolutions;

    [SerializeField]
    UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera ppc;

    bool zoomIn;

    public int Yoffset = 3;

    int[] PPU;
    byte PPU_index = 0;

    public bool debug;

    FullScreenMode screenMode;
    Dictionary<string, FullScreenMode> screenModes;

    TMP_Dropdown reso_dd;
    TMP_Dropdown scaling_dd;

    private void Awake()
    {
        GameObject go = GameObject.FindGameObjectWithTag("PauseMenu");
        Transform t = go.transform.Find("Settings Menu");
        Transform r = t.Find("Resolutions");
        Transform s = t.Find("Scaling");

        reso_dd = r.GetComponent<TMP_Dropdown>();
        scaling_dd = s.GetComponent<TMP_Dropdown>();

        min = this.transform.parent.Find("CameraMin");
        max = this.transform.parent.Find("CameraMax");

    }

    private void OnEnable()
    {
        //TODO: this data can be stored in an SO? Config File?
        resolutions = new Dictionary<string, Resolution>();
        ppc = this.GetComponentInParent<UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera>();

        screenModes = new Dictionary<string, FullScreenMode>();
        screenModes.Add("Fullscreen", FullScreenMode.FullScreenWindow);
        screenModes.Add("Maximized", FullScreenMode.MaximizedWindow);
        screenModes.Add("Windowed", FullScreenMode.Windowed);

        foreach (Resolution reso in Screen.resolutions)
        {
            resolutions.Add(reso.ToString(), reso);
        }

        //set the options in the settings menu

        reso_dd.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (string s in resolutions.Keys)
        {           
            options.Add(new TMP_Dropdown.OptionData(s));
        }
        options.Reverse();
        reso_dd.AddOptions(options);

        setScalingOptions();
    }

    public void toggleFullScreen(TMP_Dropdown d) 
    { 
        this.screenMode = screenModes[d.options[d.value].text];
        Screen.SetResolution(Screen.width, Screen.height, screenMode, Screen.currentResolution.refreshRate);
    
    }

    public void setResolution(string option)
    {
        Resolution resolution = resolutions[option];

        Screen.SetResolution(resolution.width, resolution.height, screenMode, resolution.refreshRate);


        //Now populate the scaling drop down with all possible scaling options
        setScalingOptions();

    }

    void setScalingOptions()
    {
        scaling_dd.ClearOptions();
        HashSet<TMP_Dropdown.OptionData> set = new HashSet<TMP_Dropdown.OptionData>();

        if (Screen.height >= 2160) set.Add(new TMP_Dropdown.OptionData("6:1"));
        if (Screen.height >= 1440) set.Add(new TMP_Dropdown.OptionData("4:1"));
        if (Screen.height >= 1080) set.Add(new TMP_Dropdown.OptionData("3:1"));
        if (Screen.height >= 720) set.Add(new TMP_Dropdown.OptionData("2:1"));
        set.Add(new TMP_Dropdown.OptionData("1:1"));

        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>(set);
        list.Reverse();
        scaling_dd.AddOptions(list);
    }

    public void setScale(string scaleLabel)
    {
        scale = Int32.Parse(scaleLabel.Substring(0,1));
        ppc.refResolutionX = (int)(Screen.width / scale);
        ppc.refResolutionY = (int)(Screen.height / scale);
    }

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        
        targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        //heightOffset = height / 2.0f;
        //widthOffset = width / 2.0f;

        fullSpeed = smoothingSpeed;
        halfSpeed = smoothingSpeed / 4f;

        Vector3 startPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        transform.position = startPosition;

        //THIS FUNCTION MUST BE CALLED AFTER DEFAULT EXECUTION OF ROOM MANAGER.
        //SEE EXECUTION TIME IN PROJECT SETTINGS
        //GameObject currentRoom = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RoomManager>().getCurrentRoom();
        //minPos = currentRoom.GetComponent<RoomScript>().getMinPos();
        //maxPos = currentRoom.GetComponent<RoomScript>().getMaxPos();

        calculateOffsets();

        setBounds();

        PPU = new int[2];
        PPU[0] = 16; PPU[1] = 32;

    }

    //TODO call this after scale settings change
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

        scale = (int) (pixelHeight / ppc.refResolutionY);
        if (scale == 0) scale = 1;

        heightOffset = pixelHeight / (2 * scale * 16);

        widthOffset = pixelWidth / (2 * scale * 16); 

    }

    private void FixedUpdate()
    {
        if (debug) setBounds();
        
        if(ppc.assetsPPU < PPU[PPU_index])
        {
            ppc.assetsPPU += 1;
        }

        else if (ppc.assetsPPU > PPU[PPU_index])
        {
            ppc.assetsPPU -= 1;
        }

        //if (zoomIn)
            //cameraZoomIn();
        //else
            //cameraZoomOut();
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
            targetPosition.y = target.position.y + Yoffset;
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

    public void setBounds()
    {

        maxPos = max.position;
        minPos = min.position;


        float roomWidth = maxPos.x - minPos.x;
        float roomHeight = maxPos.y - minPos.y;
        
        if(((maxPos.y- minPos.y)/2.0) < heightOffset)
        {
            //must center Y
            minPos.x = minPos.x - (widthOffset - (roomWidth / 2));
            minPos.y = minPos.y - (heightOffset - (roomHeight / 2));
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
        PPU_index = (byte) ((PPU_index + 1) % PPU.Length);
        
        //if (zoomIn)
            //zoomIn = false;
        //else zoomIn = true;
    }

    public void setTarget(Transform target)
    {
        this.target = target;
        smoothingSpeed = halfSpeed;
    }

    public void resetCameraTarget()
    {
        target = player;
        smoothingSpeed = fullSpeed;
    }


}
