using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wheat : MonoBehaviour
{
    Tall_Grass_Manager tgm;
    
    SpriteRenderer[] sprites;

    [SerializeField]
    Transform left, right, center;

    SpriteRenderer[] centerSprites, leftSprites, rightSprites;

    SpriteRenderer top, topLeft, topRight, bottom;

    //need to swap these
    [SerializeField]
    Sprite bottomSingle, bottomConnected, bottomLeft, bottomRight;

    Vector2Int gridPosition, above, _left, _right;

    [SerializeField]
    Color c;

    public bool runColor;

    float widthOffset;

    public float width;

    public bool run;

    private void OnEnable()
    {
        tgm = this.transform.parent.GetComponent<Tall_Grass_Manager>();
        
        sprites = this.GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer sr in sprites)
        {
            sr.sortingLayerName = "0 Object";
        }

        //get references to spriteRenderers from transformsj
        initReferences();

        

    }

    void setWidth()
    {
        Vector2 size = new Vector2(width, 1);
        Vector2 sliceSize = new Vector2(width, 0.125f);
        
        bottom.size = size;
        top.size = new Vector2(width, 0.375f);

        foreach(SpriteRenderer sr in centerSprites)
        {
            sr.size = sliceSize;
        }

    }


    void initReferences()
    {
        left = this.transform.Find("Left");
        right = this.transform.Find("Right");
        center = this.transform.Find("Center");

        top = this.transform.Find("Top").GetComponent<SpriteRenderer>();
        topLeft = this.transform.Find("TopLeft").GetComponent<SpriteRenderer>();
        topRight = this.transform.Find("TopRight").GetComponent<SpriteRenderer>();

        centerSprites = center.GetComponentsInChildren<SpriteRenderer>();
        leftSprites = left.GetComponentsInChildren<SpriteRenderer>();
        rightSprites = right.GetComponentsInChildren<SpriteRenderer>();


        bottom = this.transform.Find("Bottom").GetComponent<SpriteRenderer>();

    }
    
    void updatePositions()
    {

        gridPosition = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.y);
        above = gridPosition + new Vector2Int(0, 1);
        _left = gridPosition + new Vector2Int(-1, 0);
        _right = gridPosition + new Vector2Int(1, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        reload();
    }

    public Vector2Int getGridPosition()
    {
        updatePositions();
        return gridPosition;
    }

    public void reload()
    {
        setWidth();
        layer();
        updateSprites();
    }

    void layer()
    {
        
        
        Vector3 temp = this.transform.position;
        temp.z = temp.y;
        this.transform.position = temp;
        
        int order = 4096 - (int)(temp.z * 16);

        bottom.sortingOrder = order;

        for(int i=7; i>=0; i--)
        {
            centerSprites[i].sortingOrder = order - 16 + (i+i);
            centerSprites[7-i].transform.position = new Vector3(bottom.transform.position.x, bottom.transform.position.y+1+(i/8f), bottom.transform.position.z +(i/8f));
            rightSprites[i].sortingOrder = order - 16 + (i + i);
            rightSprites[7-i].transform.position = new Vector3(bottom.transform.position.x+(width/2f), bottom.transform.position.y + 1 + (i / 8f), bottom.transform.position.z + (i / 8f));
            leftSprites[i].sortingOrder = order - 16 + (i + i);
            leftSprites[7-i].transform.position = new Vector3(bottom.transform.position.x-(width/2f), bottom.transform.position.y + 1 + (i / 8f), bottom.transform.position.z + (i / 8f));

        }

        topLeft.transform.position = new Vector3(bottom.transform.position.x - 0.5f, bottom.transform.position.y + 2, bottom.transform.position.z + 1);
        topRight.transform.position = new Vector3(bottom.transform.position.x + 0.5f, bottom.transform.position.y + 2, bottom.transform.position.z + 1);
        top.transform.position = new Vector3(bottom.transform.position.x, bottom.transform.position.y + 2, bottom.transform.position.z + 1);

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
            reload();
        }

        if(runColor)
        {
            runColor = false;
            updateColor();
        }
    }

    void updateColor()
    {
        foreach(SpriteRenderer sr in this.centerSprites)
        {
            sr.color = c;
        }
        foreach (SpriteRenderer sr in this.leftSprites)
        {
            sr.color = c;
        }
        foreach (SpriteRenderer sr in this.rightSprites)
        {
            sr.color = c;
        }
        top.color = c;
        topLeft.color = c;
        topRight.color = c;
        bottom.color = c;

    }

    void updateSprites()
    {
        if (tgm == null) return;
        
        if(tgm.checkWheat(above))
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

        bool isLeft = tgm.checkWheat(_left);
        bool isRight = tgm.checkWheat(_right);

        if(isLeft && isRight)
        {
            bottom.sprite = bottomConnected;
        }

        if(!isLeft && !isRight)
        {
            bottom.sprite = bottomSingle;
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
        else
        {
            topLeft.enabled = true;
            foreach (SpriteRenderer sr in left.GetComponentsInChildren<SpriteRenderer>(true))
            {
                sr.enabled = true;
            }
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
        else
        {
            topRight.enabled = true;
            foreach (SpriteRenderer sr in right.GetComponentsInChildren<SpriteRenderer>(true))
            {
                sr.enabled = true;
            }
        }


    }
}
