using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Menu : Menu
{
    Inventory_Manager im;

    Image[] itemImages;

    RectTransform spriteGrid;

    int numRows = 4;
    int numCols = 11;
    int selectedRow = 0;
    int selectedCol = 0;
    int numSlots;

    Transform Selector;

    public int menuSelection = 0;

    Vector3[] spritePositions;

    private void Awake()
    {
        numSlots = numRows * numCols;

        im = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Inventory_Manager>();
        itemImages = new Image[numSlots];
        spriteGrid = (RectTransform) this.transform.Find("Sprite Grid");
        spritePositions = new Vector3[numSlots];
        Selector = this.transform.Find("Selector");

    }

    // Start is called before the first frame update
    void Start()
    {
        Transform[] rows = new RectTransform[4];
        rows[0] = spriteGrid.Find("row0");
        rows[1] = spriteGrid.Find("row1");
        rows[2] = spriteGrid.Find("row2");
        rows[3] = spriteGrid.Find("row3");

        //Find each slot Transform in each row
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                string s = "Slot" + j;
                Transform slot = rows[i].Find(s);
                Image im = slot.GetComponentInChildren<Image>();
                itemImages[(i * numCols) + j] = slot.GetComponentInChildren<Image>();
            }
        }

        
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                float x = rows[i].transform.localPosition.x - 2;
                float y = rows[i].transform.localPosition.y - 2;
                spritePositions[(numCols * i) + j] = new Vector3(x + 18 * j, y, 0);

            }
        }

        populateInvMenu();

    }

    public void populateInvMenu()
    {
        Item[] items = im.getItems();

        for (int i = 0; i < items.Length; i++)
        {

            if (items[i].getSprite() != null)
            {
                itemImages[i].enabled = true;
                itemImages[i].sprite = items[i].getSprite();
            }
            else
                itemImages[i].enabled = false;
        }
    }


    public override void handleInput(direction urdl)
    {
        if(urdl == direction.up)
        {
            if (selectedRow > 0) selectedRow--;
        }
        else if(urdl == direction.right)
        {
            if (selectedCol < numCols - 1)
                selectedCol++;
        }
        else if(urdl == direction.down)
        {
            if (selectedRow < numRows - 1) selectedRow++;
        }
        else if(urdl == direction.left)
        {
            if (selectedCol > 0) selectedCol--;
        }

        menuSelection = (selectedRow * numCols) + selectedCol;

        Selector.transform.localPosition = spritePositions[menuSelection];
    }

    public override void refresh()
    {
        populateInvMenu();
    }
}
