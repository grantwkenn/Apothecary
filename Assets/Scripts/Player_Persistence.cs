using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Persistence : ScriptableObject
{
    //TODO add current selection
    
    [SerializeField]
    int health;

    [SerializeField]
    Item_Data[] items;

    byte invSelection;

    [SerializeField]
    int[] itemCounts;

    byte inventorySize;

    byte entranceNo;

    bool changingScenes;


    public void setEntrance(byte entranceNo)
    {
        this.entranceNo = entranceNo;
            
    }

    public byte getEntranceNo() { return entranceNo; }

    public int getHealth() { return health; }

    public void setHealth(int health) { this.health = health; }

    public bool isChangingScenes() { return changingScenes; }

    public void setChangingScenes(bool b) { changingScenes = b; }

    public void setItems(Item[] items)
    {
        this.items = new Item_Data[items.Length];
        this.itemCounts = new int[items.Length];

        for(int i = 0; i<items.Length; i++)
        {
            this.items[i] = items[i].getData();
            this.itemCounts[i] = items[i].getQuantity();
        }
    }

    public Item_Data[] getItemData()
    {
        return this.items;
    }

    public int[] getItemCounts()
    {
        return this.itemCounts;
    }

    public void setInvSelection(byte sel) { this.invSelection = sel; }

    public byte getInvSelection() { return this.invSelection; }

}


