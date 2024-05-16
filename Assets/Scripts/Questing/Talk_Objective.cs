using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Talk_Objective : Quest_Objective
{
    [SerializeField]
    private Talk_Objective_Data data;

    public void dialogueComplete()
    {   
        complete = true;
    }

    public Talk_Objective(Talk_Objective_Data tod)
    {
        this.data = tod;
    }

    public Talk_Objective_Data getData() { return data; }

    public void setComplete(bool val) { complete = val; }


}
