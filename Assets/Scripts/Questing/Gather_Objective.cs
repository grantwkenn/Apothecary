using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gather_Objective : Quest_Objective
{
    [SerializeField]
    private Gather_Objective_Data data;

    [SerializeField]
    private int count = 0;
    //count the number of items

    //if objective is retro, then we count how many already holding at beginning
    bool retroactive;

    //This must keep track of the quantity on hand
    //measure everything coming in or out to maintain accuracy

    public void countUpdate(int Quantity)
    {
        count += Quantity;
        if (count >= data.getNumToGather()) complete = true;
        else complete = false;

    }

    public Gather_Objective(Gather_Objective_Data god)
    {
        this.data = god;
        count = 0;
        retroactive = god.isRetroactive();
    }

    public Gather_Objective_Data getData()
    {
        return data;
    }

    public int getCount() { return count; }

    public void setCount(int _count) { this.count = _count; }

}
