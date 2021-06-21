using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text tx;
    
    int framerate;

    int rates = 0;

    int averageframerate;

    int counter;

    
    // Start is called before the first frame update
    void Start()
    {

        framerate = (int)(1f / Time.unscaledDeltaTime);

        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int fixd = (int)(1f / Time.fixedDeltaTime);
        
        framerate = (int)(1f / Time.unscaledDeltaTime);

        counter++;
        if(counter >= 60)
        {    
            tx.text = "Framerate: " + framerate.ToString() + "Fixed: " + fixd;
            counter = 0;
        }

    }


}
