using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Perfect : MonoBehaviour
{
    Transform[] transforms;
    SpriteRenderer[] srs;
    PolygonCollider2D[] pcs;
    BoxCollider2D[] bcs;

    public bool dynamic;

    public bool units;


    private void OnEnable()
    {
        loadReferences();

        run();
    }

    void loadReferences()
    {
        transforms = GameObject.FindObjectsOfType<Transform>();
        srs = GameObject.FindObjectsOfType<SpriteRenderer>();
        pcs = GameObject.FindObjectsOfType<PolygonCollider2D>();
        bcs = GameObject.FindObjectsOfType<BoxCollider2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dynamic)
            run();
        
    }

    public void run()
    {
        loadReferences();
        
        if(units)
        {
            unitAlign();
            return;
        }
        
        //first align this parent transform to the grid
        this.transform.position = new Vector3(sixteenths(this.transform.position.x), 
            sixteenths(this.transform.position.y), this.transform.position.z);

        //align transform to grid
        foreach (Transform t in transforms)
        {
            t.position = new Vector3(sixteenths(t.position.x), sixteenths(t.position.y), t.position.z);
        }
        
        //align sprite size to grid
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
            bc.size = new Vector2(sixteenths(bc.size.x), sixteenths(bc.size.y));
        }

    }

    float sixteenths(float input)
    {
        float px = input * 16;

        if(input < 0)
        {
            int floor = (int)px;
            int roof = ((int)px) - 1;

            //which one is closer?
            if(px < (floor - 0.5f))
            {
                return roof / 16.0f;
            }
            return floor / 16.0f;

        }

        else
        {
            int floor = (int)px;
            int roof = ((int)px) + 1;

            //which one is closer?
            if (px > (floor + 0.5f))
            {
                return roof / 16.0f;
            }
            return floor / 16.0f;
        }
    }



    float thirtiseconds(float input)
    {
        if ((input * 32) > (((int)(input * 32)) + 0.5f))
            return ((int)(input * 32) + 1) / 32.0f;


        return ((int)(input * 32)) / 32.0f;
    }

    void unitAlign()
    {
        int x = (int)Mathf.Round(this.transform.position.x);
        int y = (int)Mathf.Round(this.transform.position.y);
        int z = (int)Mathf.Round(this.transform.position.z);

        this.transform.position = new Vector3(x, y, z);

    }
        
}
