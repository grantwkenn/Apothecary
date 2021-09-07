using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : MonoBehaviour
{
    Item[] items;
    int inventorySize = 11;
    int itemCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        items = new Item[inventorySize];
        for(int i=0; i<inventorySize; i++)
        {
        
            items[i] = null;
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

    public void addItem(Item i)
    {
        //redundent
        if (inventoryFull()) return;

        items[itemCount] = i;
        
    }

    public Item[] getItems()
    {
        return items;
    }
}
