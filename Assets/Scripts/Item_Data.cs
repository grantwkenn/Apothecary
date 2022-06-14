using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Item/New Item")]
public class Item_Data : ScriptableObject
{
    
    
    [SerializeField]
    string itemName, description;

    [SerializeField]
    int itemNo, value;

    [SerializeField]
    bool stackable;

    [SerializeField]
    Sprite sprite;

    public Sprite getSprite(){ return this.sprite; }

    public int getItemNo() { return this.itemNo; }

    public string getName() { return this.itemName; }


}
