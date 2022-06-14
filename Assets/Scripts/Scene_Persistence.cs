using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Persistence", menuName = "Persistence")]
public class Scene_Persistence : ScriptableObject
{

    public int health;

    public Item[] items;
    
    public int inventorySize;


    private void OnEnable()
    {
        if(items == null)
        {
            items = new Item[inventorySize];
        }
    }

    public void setInventorySize(int num) { this.inventorySize = num; }
}


