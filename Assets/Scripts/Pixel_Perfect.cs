using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pixel_Perfect : MonoBehaviour
{
    SpriteRenderer sr;

    PolygonCollider2D pc;

    private void OnEnable()
    {
        sr = this.GetComponent<SpriteRenderer>();

        if (sr != null && sr.drawMode == SpriteDrawMode.Tiled)
        {
            float width = sixteenths(sr.size.x);

            float height = sixteenths(sr.size.y);

            //sr.size.Set(width, height);

            sr.size = new Vector2(width, height);

        }

        
        
        pc = this.GetComponent<PolygonCollider2D>();

        if (pc != null)
        {
            int len = pc.points.Length;
            for (int i = 0; i < len; i++)
            {
                Vector2 point = pc.points[i];
                float x = sixteenths(point.x);
                float y = sixteenths(point.y);

                pc.points[i].Set(x, y);

            }

        }



    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {



    }

    float sixteenths(float input)
    {
        if((input * 16) > (((int) (input * 16)) + 0.5f))
            return ((int)(input * 16) + 1) / 16.0f;


        return ((int)(input * 16)) / 16.0f;
    }
        
}
