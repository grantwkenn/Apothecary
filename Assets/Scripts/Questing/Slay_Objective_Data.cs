using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "slayObjective", menuName = "Questing/Objectives/Slay")]
[System.Serializable]
public class Slay_Objective_Data : Quest_Objective_Data
{
    public int enemyID;
    public int quantity;
}
