using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gatherObjective", menuName = "Questing/Objectives/Gather")]
public class Gather_Objective_Data : Quest_Objective_Data
{
    [SerializeField]
    private Item_Data item_data;

    [SerializeField]
    private int numToGather;

    [SerializeField]
    private bool retroactive;

    public int getItemID() { return item_data.getItemNo(); }

    public string getItemName() { return item_data.getName(); }

    public int getNumToGather() { return numToGather; }

    public bool isRetroactive() { return retroactive; }
}
