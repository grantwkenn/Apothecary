using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LayerManager : MonoBehaviour
{
    public bool run;
    
    // Start is called before the first frame update
    void Start()
    {
        //relayer();
    }

    void relayer()
    {
        //Find every object in scene which is layered
        Layered[] LayeredObjects = GameObject.FindObjectsOfType<Layered>();
        
        foreach(Layered ob in LayeredObjects)
        {
            ob.enabled = false;
            ob.enabled = true;
        }


        //turn its component on to run once

    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            relayer();
            run = false;
        }
        
    }
}
