using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "cat", menuName = "cat")]
public class Item_Catalogue : ScriptableObject
{
    [SerializeField]
    bool runReOrder;
    
    [SerializeField]
    Item_Data[] allItemData;

    public Item_Data[] getCatalogue() { return this.allItemData; }

    void reOrderItemsByNumber()
    {
        for(int i=0; i<allItemData.Length; i++)
        {
            allItemData[i].editorOnlySetItemNo(i);
        }
    }
}
