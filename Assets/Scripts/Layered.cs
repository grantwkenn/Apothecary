using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class Layered : MonoBehaviour
{
    //reference the parent Room's position

    public bool dynamic;
    Vector3 lastPosition;

    public Transform layerParent;
    public bool onTopofParent;

    Transform t;
    BoxCollider2D bc; 

    float offset = 0;
    float y;

    SpriteRenderer sr;
    SpriteRenderer[] children;

    //public Transform parentObject;

    public bool offsetFromY;

    public float heightOffset;

    public int pixelOffset;

    public byte level;

    public bool cascadeLevel;

    public bool fromParent;

    Vector3 temp;

    SpriteRenderer questIndicator;



    private void OnEnable()
    {

        t = this.GetComponent<Transform>();

        bc = this.GetComponent<BoxCollider2D>();
        sr = this.GetComponent<SpriteRenderer>();

        children = this.GetComponentsInChildren<SpriteRenderer>();

        if(offsetFromY)
        {
            heightOffset = this.transform.localPosition.y;
        }

        if(layerParent!= null)
        {
            heightOffset = this.transform.position.y - layerParent.transform.position.y;
        }

        pixelOffset = (int) (heightOffset * 16);

        offset = 0 - (pixelOffset / 16.0f);
        
        if (bc != null)
            offset += bc.offset.y - (bc.size.y / 2.0f);

        children = this.GetComponentsInChildren<SpriteRenderer>();


        //adjust the z location (layering) according to y + offset
        //_transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y + offset);

        updateLayer();


    }

    void Start()
    {


    }

    private void Update()
    {

        if (!t.position.Equals(lastPosition))
        {
            updateLayer();
        }

        //update last position
        lastPosition = transform.position;

    }


    public void updateLayer()
    {
        t = this.transform;
        
        temp = t.position;
        temp.z = temp.y + offset;
        if (layerParent != null)
            temp.z = layerParent.position.z;
        t.position = temp;




        int order = 4096 - (int)(t.position.z * 16);

        if (onTopofParent)
        {
            order += 1;
        }

        if (sr!= null)
        sr.sortingOrder = order;

        
        
        //update quest indicator child transform to match its parent
        if (questIndicator != null)
        {
            questIndicator.gameObject.layer = this.gameObject.layer;
            questIndicator.sortingLayerName = sr.sortingLayerName;
            questIndicator.sortingOrder = sr.sortingOrder;
        }

        if(cascadeLevel)
        {
            foreach(SpriteRenderer childSR in children)
            {
                childSR.sortingOrder = order;
            }
        }

    }


}
