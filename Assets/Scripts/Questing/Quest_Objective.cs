using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest_Objective
{   
    //public Quest_Objective_Data ObjectiveData;

    //public Quest quest;

    [SerializeField]
    protected bool complete;

    public bool isComplete() { return complete; }

    public Quest_Objective()
    {
        this.complete = false;
    }
}