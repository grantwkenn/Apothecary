using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    //reference the parent Room's position

    public bool dynamic;
    Vector2 lastPosition;

    void Start()
    {
        updateLayer();
    }

    private void FixedUpdate()
    {
        if(dynamic && lastPosition != (Vector2) this.transform.position)
        {
            updateLayer();
            lastPosition = (Vector2) this.transform.position;
        }
    }

    void updateLayer()
    {
        float pivotY = this.GetComponentInParent<Transform>().position.y;
        float colliderOffset = this.GetComponent<BoxCollider2D>().offset.y;
        float colliderHalfHeight = this.GetComponent<BoxCollider2D>().size.y / 2.0f;
        float colliderBottomY = pivotY + colliderOffset - colliderHalfHeight;

        GetComponentInChildren<SpriteRenderer>().sortingOrder = 0-(int)(colliderBottomY*4);
    }

}
