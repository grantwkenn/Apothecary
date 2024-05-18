using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer_Helper : MonoBehaviour, ICustomLayer
{

    public void layer(string sortingLayerName)
    {
        //TODO is this duplicated after/before layer manager?
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.y);
        
        
        //find all child objects
        SpriteRenderer[] childSprites = this.GetComponentsInChildren<SpriteRenderer>();

        string[] splitName = sortingLayerName.Split(" ");

        Sprite_Layer_Helper slh;

        //Iterate over all child sprites
        foreach (SpriteRenderer child in childSprites)
        {
            //check for an slh component on each
            slh = child.GetComponent<Sprite_Layer_Helper>();
            if (slh == null || slh.isIgnored()) continue;

            float offset = 0.0f;

            if (slh != null)
                offset = slh.getOffset();

            //update child Z with the offset
            child.transform.position = 
                new Vector3(child.transform.position.x, child.transform.position.y, child.transform.position.y + offset);

            //update the layer order based on the newly offset Z
            child.sortingOrder = 4096 - (int)(child.transform.position.z * 16);

            //Check if sort layer is not set to Auto
            if (slh != null && slh.getLevelName().CompareTo("Auto") != 0 )
            {
                string customLayer = slh.getLevelName();

                child.sortingLayerName = splitName[0] + " " + customLayer;
            }

            else //sort layer name set to Auto, set it equal to its parent sorting layer
            {
                child.sortingLayerName = sortingLayerName;
            }
        }

    }



}


