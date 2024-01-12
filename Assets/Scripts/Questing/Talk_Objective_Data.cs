using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "talkObjective", menuName = "Questing/Objectives/Talk To")]
[System.Serializable]
public class Talk_Objective_Data : Quest_Objective_Data
{
    public int NPC_ID;

    [SerializeField]
    private Message response;

    public Message getResponse() { return response; }

}
