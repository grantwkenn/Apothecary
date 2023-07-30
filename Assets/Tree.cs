using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    SpriteRenderer canopy;
    SpriteRenderer thisSR;
    
    [ExecuteInEditMode]
    // Start is called before the first frame update
    void Start()
    {
        thisSR = this.GetComponent<SpriteRenderer>();
        canopy = this.transform.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        canopy.color = thisSR.color;
    }
}
