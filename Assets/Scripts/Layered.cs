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

    Transform _transform;
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




    private void OnEnable()
    {
        
        _transform = this.GetComponent<Transform>();

        bc = this.GetComponent<BoxCollider2D>();
        sr = this.GetComponent<SpriteRenderer>();

        children = this.GetComponentsInChildren<SpriteRenderer>();

        if(offsetFromY)
        {
            heightOffset = this.transform.localPosition.y;
        }

        pixelOffset = (int) (heightOffset * 16);

        offset = 0 - (pixelOffset / 16.0f);
        
        if (bc != null)
            offset += bc.offset.y - (bc.size.y / 2.0f);


        //adjust the z location (layering) according to y + offset
        //_transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y + offset);

        setLevel();
        updateLayer();

        if (!dynamic)
            this.enabled = false;
    }

    void Start()
    {


    }

    private void Update()
    {
        if (lastPosition != _transform.position)
        {
            updateLayer();          
        }
      
    }

    void setLevel()
    {
               
        foreach(SpriteRenderer s in children)
        {
            s.sortingLayerName = "" + level + " Object";

            //this is a work-around to match the Layered property level with sprite level.
            //could be better implementation but works for now
            Layered ly = s.GetComponent<Layered>();
            if (ly != null) ly.level = level;

            
        }
    }

    void updateLayer()
    {
        int order = 4096 - (int)(_transform.position.z * 16);

        if(sr != null)
            sr.sortingOrder = order;
        
        foreach(SpriteRenderer s in children)
        {
            Layered ly = s.GetComponent<Layered>();
            if (ly == null) s.sortingOrder = order;


        }

        //USED FOR DYNAMIC OBJECTS
        //update z and previous location
        lastPosition = _transform.position;
        lastPosition.z = lastPosition.y + offset;
        _transform.position = lastPosition;
    }


}
