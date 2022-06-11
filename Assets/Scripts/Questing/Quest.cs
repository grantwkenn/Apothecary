using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    //TODO quest should have a list of pre-requisite scriptable object sub-types
    // such as "unlock this part of the map" "get this ability" "time of week, or month"
    // then keep track of when these requisites have been satisfied to make a quest available.
    // in addition to the pre-req quest



    //public Quest_Data questData;


    //These should be deprecated!
    //Only Need the objectives in the quest object in log, other data can be located by QM


    //public int QID;
    //public int QGID;
    //public int TurnInQGID;
    //public bool isMainQuest;

    private Quest_Data data;


    public List<Quest_Objective> objectives;
    public List<Slay_Objective> slay_objectives;
    public List<Talk_Objective> talk_objectives;
    public List<Gather_Objective> gather_objectives;

    //Subtypes
    //enemies slain
    //items obtained
    //people talked to (dialogue terminated)
    //places visited
    //unlocked achievement (built bridge, opened shop, etc.)

    public Quest(Quest_Data qData)
    {
        this.data = qData;

        objectives = new List<Quest_Objective>();
        slay_objectives = new List<Slay_Objective>();
        talk_objectives = new List<Talk_Objective>();
        gather_objectives = new List<Gather_Objective>();

        foreach (Slay_Objective_Data sod in qData.getSlayObjectives())
        {
            Slay_Objective so = new Slay_Objective(sod);
            slay_objectives.Add(so);
        }

        foreach(Talk_Objective_Data tod in qData.getTalkObjectives())
        {
            Talk_Objective to = new Talk_Objective(tod);
            talk_objectives.Add(to);
        }

        foreach(Gather_Objective_Data god in qData.getGatherObjectives())
        {
            Gather_Objective go = new Gather_Objective(god);
            gather_objectives.Add(go);
        }
    }

    public Quest_Data getData() { return this.data; }
    
    ///TODO change all of these to appropriate protection level
    ///and use data's getters instead of other functions.

    //public int getQGID() { return data.getQuestGiverID(); }
    //public int getTurnInQGID() { return data.getTurnInQGID(); }

    //public int getQID() { return data.getQuestID(); }

    //public bool isMainQuest() { return data.getIsMainQuest(); }

    //public int getRewards() { return data.getRewards().Count; }
}

