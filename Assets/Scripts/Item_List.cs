using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item List", menuName = "Item/Item List")]
public class Item_List : ScriptableObject
{
    public Item_Data[] items;
}
