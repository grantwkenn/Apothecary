using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gatherObjective", menuName = "Questing/Objectives/Gather")]
public class Gather_Objective_Data : Quest_Objective_Data
{
    public int itemID;
    public int quantity;
    public bool retroactive;
}
