using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text tx;

    public Input_Manager ip;

    int framerate;

    int rates = 0;

    int averageframerate;

    int counter;

    
    // Start is called before the first frame update
    void Start()
    {
        ip = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Input_Manager>();

        


        framerate = (int)(1f / Time.unscaledDeltaTime);

        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = ip.readInput();
        
        int fixd = (int)(1f / Time.fixedDeltaTime);
        
        framerate = (int)(1f / Time.unscaledDeltaTime);

        counter++;
        if(counter >= 20)
        {    
            tx.text = framerate.ToString() + " FPS" + " | INPUT: " + input.x + ", " + input.y;
            counter = 0;
        }


    }


}
