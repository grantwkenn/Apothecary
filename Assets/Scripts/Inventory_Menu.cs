using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Menu : Menu
{
    Inventory_Manager im;

    Image[] itemImages;

    RectTransform spriteGrid;

    Text coinCounter;

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

        coinCounter = this.transform.Find("Coin Counter").GetComponent<Text>();

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
        coinCounter.text = commaSeparatedAmount(im.getCoinAmount());
        
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

    public string commaSeparatedAmount2(int amt)
    {
        string result = amt.ToString("NO");
        return result;
    }

    public string commaSeparatedAmount(int amt)
    {
        if (amt == 0) return "0";
        
        string reverse = "";
        while(amt > 0)
        {
            for(int i = 0; i<3; i++)
            {
                if (amt <= 0) break;
                int digit = amt % 10;
                amt = amt / 10;
                reverse += digit;
            }

            if (amt > 0) reverse += ",";
        }
        string result = "";
        for(int i = reverse.Length-1; i>=0; i--)
        {
            result += reverse[i];
        }
        return result;
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

    public int getSelection()
    {
        return this.menuSelection;
    }
}
