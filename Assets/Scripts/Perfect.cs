using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Perfect : MonoBehaviour
{

    SpriteRenderer[] srs;
    PolygonCollider2D[] pcs;
    BoxCollider2D[] bcs;

    public bool dynamic;

    private void OnEnable()
    {

        srs = GameObject.FindObjectsOfType<SpriteRenderer>();
        pcs = GameObject.FindObjectsOfType<PolygonCollider2D>();
        bcs = GameObject.FindObjectsOfType<BoxCollider2D>();


        run();

        if(!dynamic)
            this.enabled = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        run();
    }

    void run()
    {
        foreach(SpriteRenderer sr in srs)
        {
            float width = sixteenths(sr.size.x);

            float height = sixteenths(sr.size.y);

            sr.size = new Vector2(width, height);
        }
        
        
        foreach(PolygonCollider2D pc in pcs)
        {
            //for each path in collider
            for(int i=0; i< pc.pathCount; i++)
            {
                Vector2[] path = pc.GetPath(i);
                Vector2[] newPath = new Vector2[path.Length];

                for (int j = 0; j < path.Length; j++)
                {
                    Vector2 point = path[j];
                    point.x = sixteenths(point.x);
                    point.y = sixteenths(point.y);

                    newPath[j] = point;
                }

                pc.SetPath(i, newPath);
            }
        }

        foreach(BoxCollider2D bc in bcs)
        {
            //adjust the center and offsets of the box
            
            bc.offset = new Vector2(thirtiseconds(bc.offset.x), thirtiseconds(bc.offset.y));
            bc.size = new Vector2(thirtiseconds(bc.size.x), thirtiseconds(bc.size.y));
        }

    }

    float sixteenths(float input)
    {
        if((input * 16) > (((int) (input * 16)) + 0.5f))
            return ((int)(input * 16) + 1) / 16.0f;


        return ((int)(input * 16)) / 16.0f;
    }

    float thirtiseconds(float input)
    {
        if ((input * 32) > (((int)(input * 32)) + 0.5f))
            return ((int)(input * 32) + 1) / 32.0f;


        return ((int)(input * 32)) / 32.0f;
    }
        
}
