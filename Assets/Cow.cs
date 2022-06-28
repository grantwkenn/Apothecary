using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{

    public float speed;

    Vector3 spd;
    
    // Start is called before the first frame update
    void Start()
    {
        spd = new Vector3(speed,0,0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        this.transform.Translate(spd * Time.fixedDeltaTime);
    }
}
