using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    //reference the parent Room's position

    public bool dynamic;
    Vector2 lastPosition;

    Transform _transform;
    BoxCollider2D bc;
    SpriteRenderer sr;

    float colliderHalfHeight;
    float colliderOffset;
    float colliderBottomY;

    public float y;


    void Start()
    {
        _transform = this.GetComponent<Transform>();
        bc = this.GetComponent<BoxCollider2D>();
        sr = this.GetComponentInChildren<SpriteRenderer>();
        
        updateLayer();

        if (!dynamic)
            this.enabled = false;
    }

    private void FixedUpdate()
    {
        if(dynamic && lastPosition != (Vector2) _transform.position)
        {
            updateLayer();
            lastPosition = (Vector2)_transform.position;
        }
    }

    void updateLayer()
    {
        if (bc == null)
        {
            y = _transform.position.y + sr.sprite.bounds.min.y;
        }

        else
        {
            y = _transform.position.y + bc.offset.y - (bc.size.y / 2.0f);
        }

        
        sr.sortingOrder = 0-(int)(y*16);



    }

}
