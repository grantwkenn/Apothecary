using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event_Log : MonoBehaviour
{
    Text txt;
    float logTime;
    float logDuration = 3;

    private void OnEnable()
    {
        this.txt = GameObject.FindGameObjectWithTag("HUD").transform.Find("Event Log").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    void LateUpdate()
    {
        
        if((Time.unscaledTime - logTime) > logDuration)
        {
            txt.text = "";
        }
            
    }

    public void logEvent(string log)
    {
        txt.text = log;
        logTime = Time.unscaledTime;
    }
}
