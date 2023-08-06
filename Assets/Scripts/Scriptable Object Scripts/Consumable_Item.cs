using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable_Item : Item
{
    byte depletionIndex;
    Consumable_Data data;
    
    public Consumable_Item(Consumable_Data data, int quant) : base(data, quant)
    {
        this.data = data;
        this.depletionIndex = (byte)0;
    }

    public byte consume()
    {
        depletionIndex++;
        return data.getHealth();

    }

    public bool depleted() { return depletionIndex == data.numSprites(); }

    public override Sprite getSprite()
    {
        return this.data.getSprite(depletionIndex);
    }

}
