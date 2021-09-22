using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : MonoBehaviour
{
    public Item emptyItem;
    
    Item[] items;
    int inventorySize = 11;
    int itemCount = 0;
    int selection = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        items = new Item[inventorySize];
        for(int i=0; i<inventorySize; i++)
        {
            items[i] = emptyItem;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool inventoryFull()
    {

        return itemCount == inventorySize;
    }

    public void addItem(Item item)
    {
        //redundent
        if (inventoryFull()) return;

        if (items[selection] == emptyItem)
            items[selection] = item;
        else
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (items[i] == emptyItem)
                {
                    items[i] = item;
                    break;
                }
            }
        }
              
    }

    public Item[] getItems()
    {
        return items;
    }


    public Item getEmptyItem()
    {
        return emptyItem;
    }

    public void toggleSelection(int val)
    {
        selection += val;
        if (selection >= inventorySize)
            selection = 0;
        else if (selection < 0)
            selection = inventorySize - 1;
    }

    public int getSelectionNumber()
    {
        return selection;
    }

    public Item getSelectedItem()
    {
        return items[selection];
    }

    public void discardSelection()
    {
        items[selection] = emptyItem;
        itemCount--;
    }
}
