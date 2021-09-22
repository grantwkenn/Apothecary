using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    //reference the parent Room's position

    public bool dynamic;


    void Start()
    {
        float parentY = this.GetComponentInParent<Transform>().position.y;

        GetComponentInChildren<SpriteRenderer>().sortingOrder = -1-(int)parentY;
    }

    private void Update()
    {
        if(dynamic)
        {
            float parentY = this.GetComponentInParent<Transform>().position.y;

            GetComponentInChildren<SpriteRenderer>().sortingOrder = -1 - (int)parentY;
        }
    }

}
