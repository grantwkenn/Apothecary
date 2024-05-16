using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slay_Objective : Quest_Objective
{
    [SerializeField]
    private Slay_Objective_Data data;

    [SerializeField]
    private int count = 0;

    public void countKill()
    {
        if (complete) return;

        count++;
        if (count >= data.quantity) complete = true;
    }

    public Slay_Objective(Slay_Objective_Data sod)
    {
        this.data = sod;
        this.count = 0;
    }

    public Slay_Objective_Data getData() { return data; }

    public void setCount(int _count) { this.count = _count; }

    public int getCount() { return this.count; }
}
