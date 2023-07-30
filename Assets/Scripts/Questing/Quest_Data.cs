using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "quest", menuName = "Questing/New Quest")]
public class Quest_Data : ScriptableObject
{
    //Maybe these need to be private, but only the QM should have access
    // to Quest Data objects right now...

    [SerializeField]
    string title;

    [SerializeField]
    private int[] preQuests, worldValues;

    [SerializeField]
    private int relationship, questID, questGiverID, turnIn_QGID;

    [SerializeField]
    private bool isMainQuest;

    [SerializeField]
    private Quest_Dialogue_Data dialogue_data;

    //public Quest_Data preRequisite;
    //public Quest_Data nextQuest;

    [SerializeField]
    private List<Quest_Objective_Data> objectives;

    [SerializeField]
    private List<Slay_Objective_Data> slay_objectives;

    [SerializeField]
    private List<Talk_Objective_Data> talk_objectives;

    [SerializeField]
    private List<Gather_Objective_Data> gather_objectives;


    [SerializeField]
    List<Item> questItems;
    [SerializeField]
    List<Item> rewards;

    public int[] getPreQuests() { return preQuests; }

    public Quest_Dialogue_Data getDialogueData() { return dialogue_data; }

    public bool getIsMainQuest() { return isMainQuest; }
    
    public int getQuestID() { return questID; }
    
    public int getQuestGiverID() { return questGiverID; }

    public int getTurnInQGID() { return turnIn_QGID; }

    public List<Item> getRewards() { return rewards; }

    public List<Item> getQuestItems() { return questItems; }

    public int numRewards() { return rewards.Count; }

    public int numQuestItems() { return questItems.Count; }

    public List<Quest_Objective_Data> getObjectives() { return objectives; }

    public List<Talk_Objective_Data> getTalkObjectives() { return talk_objectives; }

    public List<Slay_Objective_Data> getSlayObjectives() { return slay_objectives; }

    public List<Gather_Objective_Data> getGatherObjectives() { return gather_objectives; }

    public string getTitle() { return this.title; }



    //what else is a reward for a quest? Unlock new parts of the map?
    /*
     * New behavior routine for an NPC? (walks out the door, etc. --> cutscene)
     * 
     */
}
