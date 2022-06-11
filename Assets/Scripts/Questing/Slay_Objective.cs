using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slay_Objective : Quest_Objective
{
    public Slay_Objective_Data data;

    public int count = 0;

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
}
