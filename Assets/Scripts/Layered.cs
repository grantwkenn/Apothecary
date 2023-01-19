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

    SpriteRenderer[] children;

    //public Transform parentObject;

    public int pixelOffset;
    float heightOffset;


    private void OnEnable()
    {
        
        _transform = this.GetComponent<Transform>();

        bc = this.GetComponent<BoxCollider2D>();

        heightOffset = pixelOffset / 16.0f;

        if (bc != null)
            offset = heightOffset + bc.offset.y - (bc.size.y / 2.0f);

        children = this.GetComponentsInChildren<SpriteRenderer>();

        _transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y);

        updateLayer();

        if (!dynamic)
            this.enabled = false;
    }


    void Start()
    {


    }


    private void FixedUpdate()
    {


        
    }

    private void Update()
    {
        if (lastPosition != _transform.position)
        {
            updateLayer();
           
        }
      
    }

    void updateLayer()
    {
        if (bc == null)
        {
            offset = heightOffset;
        }
            

        y = _transform.position.y + offset;


        int order = 4096 - (int)(y * 16);
        //// FIX THIS

        foreach(SpriteRenderer s in children)
        {
            s.sortingOrder = order;
        }


        //update z and previous location
        lastPosition = _transform.position;
        lastPosition.z = lastPosition.y;
        _transform.position = lastPosition;
    }


    /**
    void setupTilemap()
    {
        TilemapRenderer tr = this.GetComponent<TilemapRenderer>();
        
        y = _transform.position.y;

        
        //// FIX THIS
        tr.sortingOrder = 900 - (int)(y * 16);

    }
    **/

}
