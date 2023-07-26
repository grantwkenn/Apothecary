using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheat : MonoBehaviour
{
    Tile_Manager tm;
    
    SpriteRenderer[] sprites;

    [SerializeField]
    Transform left, right, center;

    SpriteRenderer[] centerSprites, leftSprites, rightSprites;

    SpriteRenderer top, topLeft, topRight, bottom;

    //need to swap these
    [SerializeField]
    Sprite bottomSingle, bottomConnected, bottomLeft, bottomRight;

    Vector2Int position, above, aboveLeft, aboveRight, _left, _right;


    public float width;

    public bool run;

    private void OnEnable()
    {
        tm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Tile_Manager>();
        
        sprites = this.GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer sr in sprites)
        {
            sr.sortingLayerName = "0 Object";
        }

        //get references to spriteRenderers from transformsj
        initReferences();

        position = (Vector2Int.RoundToInt((Vector2)this.transform.position));
        above = position + new Vector2Int(0,1);
        aboveLeft = above + new Vector2Int(-1,0);
        aboveRight = above + new Vector2Int(1,0);

        _left = position + new Vector2Int(-1, 0);
        _right = position + new Vector2Int(1, 0);

        centerSprites = center.GetComponentsInChildren<SpriteRenderer>();
        leftSprites = left.GetComponentsInChildren<SpriteRenderer>();
        rightSprites = right.GetComponentsInChildren<SpriteRenderer>();

    }

    void initReferences()
    {
        left = this.transform.Find("Left");
        right = this.transform.Find("Right");
        center = this.transform.Find("Center");

        top = this.transform.Find("Top").GetComponent<SpriteRenderer>();
        topLeft = this.transform.Find("TopLeft").GetComponent<SpriteRenderer>();
        topRight = this.transform.Find("TopRight").GetComponent<SpriteRenderer>();


        bottom = this.transform.Find("Bottom").GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tm.mapWheat(position);
        layer();
    }

    void layer()
    {
        Vector3 temp = this.transform.position;
        temp.z = temp.y;
        this.transform.position = temp;
        
        int order = 4096 - (int)(temp.z * 16);

        bottom.sortingOrder = order;

        for(int i=0; i<8; i++)
        {
            centerSprites[i].sortingOrder = order - 16 + (i+i);
            rightSprites[i].sortingOrder = order - 16 + (i + i);
            leftSprites[i].sortingOrder = order - 16 + (i + i);
        }

        top.sortingOrder = order - 16;
        topLeft.sortingOrder = order - 16;
        topRight.sortingOrder = order - 16;
    }

    // Update is called once per frame
    void Update()
    {
        if(run)
        {
            run = false;
            tm.mapWheat(position);
            layer();
            updateSprites();
        }


    }

    void updateSprites()
    {
        if (tm == null) return;
        
        if(tm.checkWheat(above))
        {
            this.top.enabled = false;
            this.topLeft.enabled = false;
            this.topRight.enabled = false;
        }
        else
        {
            this.top.enabled = true;
            this.topLeft.enabled = true;
            this.topRight.enabled = true;
        }

        bool isLeft = tm.checkWheat(_left);
        bool isRight = tm.checkWheat(_right);

        if(isLeft && isRight)
        {
            bottom.sprite = bottomConnected;
        }


        if (isLeft)
        {
            topLeft.enabled = false;
            foreach(SpriteRenderer sr in left.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }
            if (!isRight) bottom.sprite = bottomRight;
        }

        if (isRight)
        {
            topRight.enabled = false;
            foreach (SpriteRenderer sr in right.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }
            if (!isLeft) bottom.sprite = bottomLeft;
        }
    }
}
