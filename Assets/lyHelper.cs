using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class lyHelper : MonoBehaviour
{
    //reference the parent Room's position

    public bool edgeCase;

    public Transform layerParent;

    Transform t;
    BoxCollider2D bc;

    float offset = 0;
    float y;

    SpriteRenderer sr;

    //public Transform parentObject;

    public bool pixels;

    public bool calculate;

    public bool run;

    public bool offsetFromY; //use its local y as offset from its parent

    public float heightOffset; //world unit offset

    public int pixelOffset; //pixel offset from self



    Vector3 temp;




    private void Update()
    {
        if(calculate)
        {
            calculateOffset();
            calculate = false;
        }

        if(run)
        {
            updateLayer();
            run = false;
        }
    }

    void calculateOffset()
    {
        if (layerParent != null)
        {
            heightOffset = this.transform.position.y - layerParent.transform.position.y;
            return;
        }

        if (pixels)
        {
            heightOffset = pixelOffset / 16;
            return;
        }

        if(offsetFromY)
        {
            heightOffset = this.transform.localPosition.y;
            return;
        }
    }

    private void Awake()
    {
        temp = new Vector3();
    }

    private void OnEnable()
    {

        t = this.GetComponent<Transform>();

        bc = this.GetComponent<BoxCollider2D>();
        sr = this.GetComponent<SpriteRenderer>();

        calculateOffset();

        pixelOffset = (int)(heightOffset * 16);

        offset = 0 - (heightOffset);

        //use the bottom of the box collider if present
        if (bc != null)
            offset += bc.offset.y - (bc.size.y / 2.0f);

        updateLayer();

        this.enabled = false;
    }


    void updateLayer()
    {
        temp = t.position;
        temp.z = temp.y + offset;
        t.position = temp;

        int order = 4096 - (int)(t.position.z * 16);
        
        sr.sortingOrder = order;

    }

    public static void layerManHelper(SpriteRenderer sr)
    {
        Vector3 pos = sr.transform.position;
        pos.z = pos.y;
        sr.transform.position = pos;

        int order = 4096 - (int)(sr.transform.position.z * 16);

        sr.sortingOrder = order;
    }

}
