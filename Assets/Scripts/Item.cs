using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Item
{
    [SerializeField]
    Item_Data data;
    [SerializeField]
    int stackQuantity;

    public Item_Data getData()
    {
        return this.data;
    }

    public Item(Item_Data data, int quant)
    {
        this.data = data;
        this.stackQuantity = quant;
    }

    public int getItemNo() { return data.getItemNo(); }

    public Sprite getSprite() { return data.getSprite(); }

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

}
