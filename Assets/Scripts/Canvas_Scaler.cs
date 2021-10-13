using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class Canvas_Scaler : MonoBehaviour
{
    Canvas canvas;

    int refHeight = 360;

    int currentHeight;
    int prevHeight;
    
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();

        currentHeight = Screen.height;
        prevHeight = Screen.height;

        ScaleResolution();

    }

    // Update is called once per frame
    void Update()
    {
        if(currentHeight != Screen.height)
        {
            prevHeight = currentHeight;
            currentHeight = Screen.height;
        }
        if(currentHeight != prevHeight)
            ScaleResolution();
    }


    void ScaleResolution()
    {
        int height = Screen.height;

        //if
        int scale = height / refHeight;

        canvas.scaleFactor = scale;
    }


}
