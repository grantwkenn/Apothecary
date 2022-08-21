using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Fire : MonoBehaviour
{
    Light2D light;
    SpriteRenderer sr;
    
    
    // Start is called before the first frame update
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        light = this.GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {


    }
}
