using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_Layering : MonoBehaviour, ICustomLayer
{
    [SerializeField]
    GameObject layerBuddy;

    [SerializeField]
    bool customLevel;
    
    public void layer(string sortingLayerName)
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.transform.position =
            new Vector3(sr.transform.position.x, sr.transform.position.y, layerBuddy.transform.position.z);

        int order = 4096 - (int)(sr.transform.position.z * 16);
        sr.sortingOrder = order;

        if(!customLevel)
        {
            sr.sortingLayerName = sortingLayerName;
        }


    }
}
