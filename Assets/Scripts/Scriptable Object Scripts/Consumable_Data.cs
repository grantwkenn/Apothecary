using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Item/Consumable Item Data")]
public class Consumable_Data : Item_Data
{
    [SerializeField]
    byte health;

    [SerializeField]
    Sprite[] depletionSprites;

    public Sprite getSprite(byte index) { return depletionSprites[index]; }

    public override Sprite getSprite()
    {
        return depletionSprites[0];
    }

    public int numSprites() { return depletionSprites.Length; }

    public byte getHealth() { return health; }

}