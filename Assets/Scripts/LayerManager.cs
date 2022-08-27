﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    //reference the parent Room's position

    public bool dynamic;
    Vector3 lastPosition;

    Transform _transform;
    BoxCollider2D bc;
    SpriteRenderer sr;
    Rigidbody2D rb;

    float colliderHalfHeight;
    float colliderOffset;
    float colliderBottomY;

    float y;


    void Start()
    {
        _transform = this.GetComponent<Transform>();
        bc = this.GetComponent<BoxCollider2D>();
        sr = this.GetComponentInChildren<SpriteRenderer>();
        rb = this.GetComponent<Rigidbody2D>();
        
        updateLayer();

        _transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y);

        if (!dynamic)
            this.enabled = false;

    }


    private void FixedUpdate()
    {


        
    }

    private void Update()
    {
        if (lastPosition != _transform.position)
        {
            updateLayer();
            lastPosition = _transform.position;

            lastPosition.z = lastPosition.y;
            
            _transform.position = lastPosition;


        }
      
    }

    void updateLayer()
    {
        if (bc == null)
        {
            y = _transform.position.y;
        }

        else
        {
            y = _transform.position.y + bc.offset.y - (bc.size.y / 2.0f);
        }

        
        sr.sortingOrder = 0-(int)(y*16);



    }

}
