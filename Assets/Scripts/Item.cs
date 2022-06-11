using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item",menuName = "Item/New Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int itemNo;
    public string description;
    public Sprite sprite;

    public int value;

    public int quantity;

}
