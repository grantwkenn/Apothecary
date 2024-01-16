using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Item", menuName = "Item/Consumable Item Data")][System.Serializable]
public class Consumable_Data : Item_Data
{
    [SerializeField]
    byte health;

    [SerializeField]
    Sprite[] depletionSprites;

    public override Sprite getSprite(int index)
    {
        return depletionSprites[index];
    }

    public override int numSprites() { return depletionSprites.Length; }

    public byte getHealth() { return health; }



}