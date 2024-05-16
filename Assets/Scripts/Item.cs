using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Item
{
    [SerializeField]
    Item_Data data;
    [SerializeField]
    protected int stackQuantity;

    int depletionIndex;

    public Item_Data getData()
    {
        return this.data;
    }

    public Item(Item_Data data, int quant)
    {
        this.data = data;
        this.stackQuantity = quant;
        depletionIndex = 0;
    }

    public int getItemNo() { return data.getItemNo(); }

    public virtual Sprite getSprite() { return data.getSprite(depletionIndex); }

    public int getQuantity() { return stackQuantity; }

    public string getName() { return data.getName(); }

    public void addQuantity(int value)
    {
        this.stackQuantity += value;
    }

    public void subtractQuantity(int value)
    {
        if (value <= stackQuantity)
            this.stackQuantity = this.stackQuantity - value;
        else
            Debug.Log("Item counting error");
    }


    public bool isEmpty()
    {
        return this.data.getItemNo() == 0;
    }


    public bool deplete()
    {
        depletionIndex++;
        if (depletionIndex == data.numSprites())//need to delete
            return true;
        return false;
    }

}
