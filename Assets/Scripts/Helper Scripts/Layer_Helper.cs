using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer_Helper : MonoBehaviour, ICustomLayer
{

    public void layer(string sortingLayerName)
    {
        //find all child objects
        SpriteRenderer[] srs = this.GetComponentsInChildren<SpriteRenderer>();

        string[] splitName = sortingLayerName.Split(" ");

        Sprite_Layer_Helper slh;

        foreach (SpriteRenderer sr in srs)
        {

            float offset = 0.0f;
            slh = sr.GetComponent<Sprite_Layer_Helper>();
            if (slh != null)
                offset = slh.getOffset();


            sr.transform.position =
                new Vector3(sr.transform.position.x, sr.transform.position.y, sr.transform.position.y+offset);

            int order = 4096 - (int)(sr.transform.position.z * 16);
            sr.sortingOrder = order;

            if (slh != null && slh.getName().CompareTo("Auto") != 0 )
            {
                string customLayer = slh.getName();

                sr.sortingLayerName = splitName[0] + " " + customLayer;
            }

            else
            {
                sr.sortingLayerName = sortingLayerName;
            }
        }

    }



}


