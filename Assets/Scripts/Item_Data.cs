using System.Collections;
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

    public virtual Sprite getSprite(){ return this.sprite; }

    public int getItemNo() { return this.itemNo; }

    public string getName() { return this.itemName; }

    public int getStackLimit() { return this.stackLimit; }

    public void editorOnlySetItemNo(int number)
    {
        this.itemNo = number;
    }

    public bool isTileSelector() { return tileSelector; }
}
