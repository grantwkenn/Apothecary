using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    //reference the parent Room's position


    void Start()
    {
        float parentY = this.GetComponentInParent<Transform>().position.y;

        GetComponentInChildren<SpriteRenderer>().sortingOrder = -1-(int)parentY;
    }

}
