using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Persistence", menuName = "Persistence")]
public class Scene_Persistence : ScriptableObject
{
    [SerializeField]
    int health;

    public string[] itemNames;

    [SerializeField]
    Item_Data[] items;

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

    public void setItemNames(string[] names)
    {
        itemNames = names;
    }

    public string[] getItemNames() { return itemNames; }

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

}


