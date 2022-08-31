using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Persistence", menuName = "Persistence")]
public class Scene_Persistence : ScriptableObject
{

    public int health;

    public Item[] items;
    
    public int inventorySize;

    byte entranceNo;

    bool changingScenes;


    private void OnEnable()
    {
        if(items == null)
        {
            items = new Item[inventorySize];
        }
    }

    public void setInventorySize(int num) { this.inventorySize = num; }

    public void setEntrance(byte entranceNo)
    {
        this.entranceNo = entranceNo;
            
    }

    public byte getEntranceNo() { return entranceNo; }

    public int getHealth() { return health; }

    public bool isChangingScenes() { return changingScenes; }

    public void setChangingScenes(bool b) { changingScenes = b; }
}


