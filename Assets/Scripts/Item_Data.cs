﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item",menuName = "Item/New Item")]
public class Item_Data : ScriptableObject
{
        
    [SerializeField]
    string itemName, description;

    [SerializeField]
    int itemNo, value;

    [SerializeField]
    int stackLimit = 16;

    [SerializeField]
    Sprite sprite;

    [SerializeField]
    bool tileSelector;

    [SerializeField]
    int depletions;

    public int getDepletions() { return this.depletions;  }
    
    public virtual Sprite getSprite(int index){ return this.sprite; }

    public virtual Sprite getSprite() { return this.sprite; }

    public virtual int numSprites() { return 1; }

    public int getItemNo() { return this.itemNo; }

    public string getName() { return this.itemName; }

    public int getStackLimit() { return this.stackLimit; }

    public void editorOnlySetItemNo(int number)
    {
        this.itemNo = number;
    }

    public bool isTileSelector() { return tileSelector; }

    public void editorSetItemName(string name) { this.itemName = name; }

    public void replaceData(ItemDataJSON newData)
    {
        this.description = newData.description;
        this.itemName = newData.itemName;
        this.itemNo = newData.itemNo;
        this.value = newData.value;
        this.stackLimit = newData.stackLimit;
        this.sprite = newData.sprite;
    }
}
