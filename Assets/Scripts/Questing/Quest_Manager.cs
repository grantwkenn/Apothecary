using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Quest_Manager : MonoBehaviour
{
    //Reference
    Inventory_Manager inventory_Manager;
    Dialogue_Manager dialogueManager;

    
    //list of all Quest Data Scriptable Objects from Assets
    public Quest_List allQuests; //from Assets

    //all quest data by quest ID
    [SerializeField]
    Quest_Data[] questDataByQID;

    int questCount;

    // maps QGID to list of their quests
    Dictionary<int, List<int>> Quests_by_QGID; //populated on enable

    //TODO?: rename Quests_by_QGID to Quests_Given_by_QGID
    //Dictionary<int, List<int>> Quests_Given_by_QGID; //populated on enable
    Dictionary<int, List<int>> Quests_turnedIn_by_QGID; //populated on enable

    //To and From Disk on Save
    [SerializeField]
    private bool[] questsCompletionbyQID;


    Dictionary<int, Quest_Giver> activeQG_By_ID;

    Dictionary<int, int> currentQuest_by_QGID;


    // QUEST LOG//////////////////////////////////
    [SerializeField]
    List<Quest> questLog;

    //These are not going to work 6/7/22
    //List<Talk_Objective> talkObjectives;
    //List<Slay_Objective> slayObjectives;
    //List<Gather_Objective> gatherObjectives;

    const int LOG_CAPACITY = 20;
    //[SerializeField]
    //int questsInLog;

    //not to be used? 6/7/22
    //List<int> MainPendingTurnIn;
    //List<int> PendingTurnIn;
    //////////////////////////////////////////////



    private void OnEnable()
    {
        //after Game Manager, Inventory Manager
        inventory_Manager = this.GetComponentInParent<Inventory_Manager>();
        dialogueManager = this.GetComponentInParent<Dialogue_Manager>();
        if (inventory_Manager == null) Debug.Log("Inventory Manager Null");

        questCount = allQuests.questList.Count;

        questDataByQID = new Quest_Data[questCount];

        //before getQuestMessage
        Quests_by_QGID = new Dictionary<int, List<int>>();
        Quests_turnedIn_by_QGID = new Dictionary<int, List<int>>();

        //before QGEnable and getQuestMessage
        activeQG_By_ID = new Dictionary<int, Quest_Giver>();

        currentQuest_by_QGID = new Dictionary<int, int>();

        //populate Quests_by_QGID
        //map all quests to their Quest Givers
        foreach (Quest_Data qd in allQuests.questList)
        {
            int QID = qd.getQuestID();
            int QGID = qd.getQuestGiverID();
            
            //map quest data by QID
            questDataByQID[QID] = qd;

            //add to QG's list///////////////////////////////////////////
            List<int> list = new List<int>();

            if (Quests_by_QGID.TryGetValue(QGID, out list))
                Quests_by_QGID.Remove(QGID);
            else
                list = new List<int>();

            list.Add(QID);
            Quests_by_QGID.Add(QGID, list);
            /////////////////////////////////////////////////////////////
            ///
            /*
            //map turnIn QG by QID
            list = new List<int>();

            if (Quests_turnedIn_by_QGID.TryGetValue(QGID, out list))
                Quests_turnedIn_by_QGID.Remove(QGID);
            else
                list = new List<int>();

            list.Add(QID);
            Quests_turnedIn_by_QGID.Add(QGID, list); */
            /////////////////////////////////////////////////////////////
            ///
        }


        //???sort all Quests_by_QGID lists by priority
        //!!!!!!!! This is not a proper priority sorting, main quests are not in front, simply by qid
        foreach (KeyValuePair<int, List<int>> questList in Quests_by_QGID)
        {
            questList.Value.Sort();
            //should sort is ascending order
        }

    }

    void Start()
    {
        questsCompletionbyQID = new bool[questCount];


        //instantiate the quest log, fill with current quests from disk
        questLog = new List<Quest>();
        //questsInLog = 0;

        //populate quest Log with quest objects
        // .............


        //populate all objective Lists from Quest Log
        foreach (Quest quest in questLog)
        {
            if (quest == null) continue;
            
            //bool allComplete = true;
            
            //track all objectives
            /*
            foreach(Quest_Objective ob in quest.objectives)
            {
                if (!ob.isComplete()) allComplete = false;
                
                if(ob is Gather_Objective)
                {
                    gatherObjectives.Add(ob as Gather_Objective);
                }
                else if(ob is Slay_Objective)
                {
                    slayObjectives.Add(ob as Slay_Objective);
                }
                else if(ob is Talk_Objective)
                {
                    talkObjectives.Add(ob as Talk_Objective);
                }
            }
            */
            
        }

    }

    //when a QG activates
    //add to map of active QGs
    public void questGiverEnable(Quest_Giver QG)
    {
        if (!activeQG_By_ID.ContainsKey(QG.QGID))
        {
            activeQG_By_ID.Add(QG.QGID, QG);
        } 
    }

    //when a QG deactivates
    public void questGiverDisable(Quest_Giver QG)
    {
        if (activeQG_By_ID.ContainsKey(QG.QGID))
            activeQG_By_ID.Remove(QG.QGID);
    }

    //DM uses to populate messager with relevant quest message
    public Message getQuestMessage(Messager messager)
    {
        //First check for any talk objective messages
        foreach (Quest q in questLog)
        {
            foreach (Talk_Objective to in q.talk_objectives)
            {
                if (to.data.NPC_ID != messager.getID()) continue;

                //return the talk objective message
                if (!to.isComplete())
                {
                    messager.setSymbol(2);
                    return to.data.getResponse();
                    
                }
                    
            }
        }


        //Next check if this messager is a Quest Giver with active state
        Quest_Giver QG = messager.GetComponentInParent<Quest_Giver>();
        if (QG == null) return null;

        QuestGiverState state = QG.getState();
        if (state == QuestGiverState.none)
        {
            messager.setSymbol(-1);
            return null;
        }


        Quest_Dialogue_Data data = getQuestDialogueData(QG.QGID);


        if (state == QuestGiverState.active)
        {
            messager.setSymbol(2);
            return data.getOngoingMessage();
        }

        if (state == QuestGiverState.available)
        {
            messager.setSymbol(0);
            return data.pitch;
        }

        //This state is shared by inventory full and quest log full
        if (state == QuestGiverState.availableFull)
        {
            messager.setSymbol(1);
            
            if (questLog.Count >= LOG_CAPACITY)
                return data.questLogFull;

            return data.pitchInvFull;
        }

        if (state == QuestGiverState.turnIn)
        {
            messager.setSymbol(1);
            return data.turnIn;
        }

        if (state == QuestGiverState.turnInFull)
        {
            messager.setSymbol(1);
            return data.turnInFull;
        }

        return null;
    }

    //helper method
    Quest_Dialogue_Data getQuestDialogueData(int QGID)
    {
        //if the quest giver doesn't have a current quest mapped
        if (!currentQuest_by_QGID.ContainsKey(QGID))
        {
            Debug.Log("ERROR: Key not found");
            return null;
        }

        //get the current quest ID for this giver
        int QID = currentQuest_by_QGID[QGID];

        //Error Checking
        if (QID < 0 || QID >= questDataByQID.Length)
        {
            Debug.Log("ERROR: Key not found");
            return null;
        }

        return questDataByQID[QID].getDialogueData();
    }


    //DM has terminated a message, check if this triggers any quest progression

    //TODO check if this was an objective as well!!
    public void checkDialogueProgression(Messager messager)
    {
        
        //TODO what if there are multiple quests needing to talk to the same NPC.
        //The messager will only have completed one of them, and need to know which one..

        //check if this messager's previous message (still saved) is equal to
        //the response stored in the talk objective.
        
        //check if this message was a talk objective
        //for all quests in log
        foreach(Quest quest in questLog)
        {
            foreach(Talk_Objective to in quest.talk_objectives)
            {
                if (to.data.NPC_ID != messager.getID()) continue;


                //NOTE this may not work with the comparison. IF not, find proper way to compare two messages.
                if (Dialogue_Manager.compareMessages(messager.getMessage(), to.data.getResponse()))
                {
                    //set this talk objective completed
                    to.dialogueComplete();

                    //check if quest is complete
                    //if(objectivesComplete(quest))
                    //{
                    //    int turnInQGID = quest.getData().getTurnInQGID();

                    //    if (activeQG_By_ID.ContainsKey(turnInQGID))
                    //    {
                    //        Quest_Giver qg = activeQG_By_ID[turnInQGID];

                    //        qg.setState(evaluateQuestGiverState(qg.QGID));

                    //        //currentQuest_by_QGID[qg.QGID] = quest.getData().getQuestID();

                    //        //qg.setState(QuestGiverState.turnIn);
                    //        dialogueManager.messagerRefresh(qg.GetComponentInParent<Messager>());
                    //    }


                    //}

                    checkObjectivesComplete(quest);
                        

                    //do not evaluate any other quest progression in this call
                    return;
                }
                
            }

        }
        
        //verify this messager is quest giver
        Quest_Giver QG = messager.GetComponentInParent<Quest_Giver>();
        if (QG == null) return;

        int QID;

        //check if no active quests for this QG
        if (!currentQuest_by_QGID.TryGetValue(QG.QGID, out QID)) return;
        

        // QID is the current quest for this Giver

        QuestGiverState state = QG.getState();
        
        //check progression from available to active or full
        if(state == QuestGiverState.available)
        {
            //check this last message was the pitch
            Message pitch = questDataByQID[QID].getDialogueData().pitch;
            Message lastMessage = messager.getMessage();

            //ensure that the message just finished was the pitch for this quest!
            //this check may be redundent
            if (!Dialogue_Manager.compareMessages(pitch, lastMessage)) return;
            
            //check for log full or inventory full, shared state
            if (questLog.Count >= LOG_CAPACITY 
                || !inventory_Manager.enoughSpace(questDataByQID[QID].numQuestItems()))
            {
                //set QG state, 
                QG.setState(QuestGiverState.availableFull);
            }
            
            else
            {
                //accept this quest
                Quest addedQuest = addtoLog(QID);
                QG.setState(QuestGiverState.active);


                //check if this quest has no objectives (just turn into someone else)
                //if(objectivesComplete(addedQuest))
                //{
                //    int qgid = addedQuest.getData().getTurnInQGID();

                //    Quest_Giver qg;
                //    if (activeQG_By_ID.TryGetValue(qgid, out qg))
                //    {
                //        qg.setState(evaluateQuestGiverState(addedQuest.getData().getTurnInQGID()));
                //    }

                //}

                checkObjectivesComplete(addedQuest);

                foreach(Talk_Objective to in addedQuest.talk_objectives)
                {
                    if (!activeQG_By_ID.ContainsKey(to.data.NPC_ID)) continue;

                    Quest_Giver QG2 = activeQG_By_ID[to.data.NPC_ID];
                    QG2.setState(evaluateQuestGiverState(to.data.NPC_ID));
                    Messager msgr = QG2.GetComponentInParent<Messager>();
                    msgr.nextMessage();
                }

                    //TODO: check all talk objective NPCs for their refreshes. Only the active ones!

                //now refresh all messagers in the scene
                //dialogueManager.allMessagerRefresh();
            }
            return;
        }

        //turn in this quest
        else if(state == QuestGiverState.turnIn)
        {
            //Find quest in log by QID

            Quest quest = null;
            //first find the associated quest
            foreach(Quest q in questLog)
            {
                if (q.getData().getQuestID() == QID)
                {
                    quest = q;
                    break;
                }               
            }
            if(quest == null)
            {
                Debug.Log("QM Error: Quest Not Found by QID #" + QID);
            }

            List<Item> rewards = quest.getData().getRewards();

            //check if inventory is full
            int spaceNeeded;

            if(!inventory_Manager.enoughSpace(quest.getData().numRewards()))
            {
                //set full state
                QG.setState(QuestGiverState.turnInFull);
            }

            else //enough space, complete quest
            {
                Quest_Data qd = quest.getData();

                removeFromLog(quest);


                
                questsCompletionbyQID[qd.getQuestID()] = true;


                //Quest_Giver starter = activeQG_By_ID[qd.getQuestGiverID()];

                int starterID = quest.getData().getQuestGiverID();

                if(starterID != qd.getTurnInQGID())
                {
                    //have different QGs
                    if(activeQG_By_ID[starterID] != null)
                    {
                        Quest_Giver starter = activeQG_By_ID[starterID];
                        starter.setState(evaluateQuestGiverState(starterID));
                        Messager starterMessager = starter.GetComponentInParent<Messager>();
                        starterMessager.nextMessage();
                    }
                }


                //QG.setState(QuestGiverState.none);
                //if (starterID != QG.QGID)
                //activeQG_By_ID[starterID].setState(QuestGiverState.none);

                Debug.Log("State: " + QG.getState());

                //re-evaluate state of turn In QG and original QG
                QG.setState(evaluateQuestGiverState(QG.QGID));
                


                //remove quest Items
                inventory_Manager.removeObjectives(quest.gather_objectives);
                
                //add rewards
                inventory_Manager.offerItems(rewards);
            }




        }


    }


    //set this Quest Giver's State (available, active, etc)
    //Each Quest Giver calls this when they enter scene / Start()
    //or when they need a refresh: after turn in, etc.
    public QuestGiverState evaluateQuestGiverState(int QGID)
    {
        //must re-evaluate state for any changes after this QG re-enables
        
        //remove the current mapping if any, cannot edit values in a Dict.
        currentQuest_by_QGID.Remove(QGID);
        
        //Check for Pending Turn In
        Quest pending = checkForPending(QGID);


        if(pending != null)
        {

            currentQuest_by_QGID.Add(QGID, pending.getData().getQuestID());

            //NOTE: turn in full will be checked later, 
            //just set to turn in once done with objectives
            return QuestGiverState.turnIn;

        }

        //Check for active quest
        Quest activeQuest = checkForActive(QGID);
        if (activeQuest != null)
        {
            currentQuest_by_QGID.Add(QGID, activeQuest.getData().getQuestID());
            return QuestGiverState.active;
        }



        //check for available quest
        List<int> questsGiven = null;
        Quests_by_QGID.TryGetValue(QGID, out questsGiven);
        if (questsGiven != null && questsGiven.Count > 0)
        {

            int priorityQID = -1;

            foreach (int qid in questsGiven)
            {
                if (!isAvailable(qid)) continue;

                //return first main quest
                if (questDataByQID[qid].getIsMainQuest())
                {
                    currentQuest_by_QGID.Add(QGID, qid);

                    if (inventory_Manager.enoughSpace(questDataByQID[qid].numQuestItems()))
                        return QuestGiverState.available;
                    else return QuestGiverState.availableFull;
                }

                //track the first non-main quest in this sorted list
                if (priorityQID == -1)
                {
                    priorityQID = qid;
                }
            }

            //returning the highest priority non-main quest
            if (priorityQID != -1)
            {
                currentQuest_by_QGID.Add(QGID, priorityQID);

                if (inventory_Manager.enoughSpace(questDataByQID[priorityQID].numQuestItems()))
                    return QuestGiverState.available;
                else return QuestGiverState.availableFull;
            }
        }

        //No Quests Current, leave mapping empty.
        //Set state to none;
        return QuestGiverState.none;

    }


    //check for highest priority quest ready for turn in for this Quest Giver
    //return the index in the quest log of the first match


    //TODO: need to account for when the turn in NPC is not the same as QG!
    Quest checkForPending(int QGID)
    {
        if (questLog.Count == 0) return null;

        Quest pending = null;

        foreach(Quest quest in questLog)
        {
            if(quest.getData().getTurnInQGID() == QGID && objectivesComplete(quest))
            {
                pending = quest;

                

                //do not consider another quest if main quest found in log
                if (quest.getData().getIsMainQuest()) break;
            }
        }


        if (pending == null) return null;

        //a quest is complete, que the turn in qg?


        Quest_Giver qg;
        if(activeQG_By_ID.TryGetValue(pending.getData().getTurnInQGID(), out qg))
        {
            qg.setState(QuestGiverState.turnIn);
            Messager messager = qg.GetComponentInParent<Messager>();
            //caused exception
            //messager.setMessage(getQuestMessage(messager));
        }


        return pending;
        
        //for(int index = 0; index< questLog.Count; index++)
        //{
        //    //find the first quest that matches quest giver and is pending completion
        //    //questLog is already sorted on priority??
        //    if (questLog[index].getTurnInQGID() == QGID && objectivesComplete(questLog[index]))
        //    {
        //        return questLog[index];
        //    }
        //}

        //return null;
    }

    //check quest log for quest from this Giver
    Quest checkForActive(int QGID)
    {
        if (questLog.Count == 0) return null;

        foreach(Quest quest in questLog)
        {
            if (quest.getData().getQuestGiverID() == QGID)
                return quest;

            foreach(Talk_Objective to in quest.talk_objectives)
            {
                if (to.data.NPC_ID == QGID)
                    return quest;
            }
        }
        

        return null;
    }

    bool isAvailable(int QID)
    {
        if (questsCompletionbyQID[QID]) return false;
        
        Quest_Data data = questDataByQID[QID];
        int[] preQuests = data.getPreQuests();

        if (preQuests == null || preQuests.Length == 0) return true;
        
        foreach(int preQuest in preQuests)
        {
            if (!questsCompletionbyQID[preQuest]) return false;
        }
        
        return true;
    }
    
    
    public void itemAddedOrRemoved(int itemID, int Quantity)
    {

        //optimally we would only search quests that have gather objectives.. but only n<=20
        //make this async if it gets too slow!

        //search each quest in log
        foreach (Quest quest in questLog)
        {
            //TODO when objective was complete before discarding, need to re evaluate to incomplete.
            foreach(Gather_Objective obj in quest.gather_objectives)
            {
                if (obj.getData().getItemID() != itemID)  continue;
                
                obj.countUpdate(Quantity);

                if (obj.isComplete())
                {
                    //check quest is complete
                    //bool questComplete = objectivesComplete(quest);

                    //if (questComplete)
                    //{


                    //    int turnInQGID = quest.getData().getTurnInQGID();

                    //    //attempt to change the QG's (or Turn In's) state / notify them
                    //    //NOTE: need to support turn in quest givers as separate from starters
                    //    if (activeQG_By_ID.ContainsKey(turnInQGID))
                    //    {
                    //        //get the turning in QG
                    //        Quest_Giver QG = activeQG_By_ID[turnInQGID];


                    //        QG.setState(evaluateQuestGiverState(turnInQGID));
                    //        Messager messager = QG.GetComponentInParent<Messager>();
                    //        messager.nextMessage();
                    //        //messager.setMessage(dialogueManager.nextMessage(QG.GetComponentInParent<Messager>()));
                    //    }

                    //}

                    checkObjectivesComplete(quest);
                }

            }
        }
        
    }

    public bool checkObjectivesComplete(Quest quest)
    {
        if (!objectivesComplete(quest)) return false;

        int turnInQGID = quest.getData().getTurnInQGID();

        if (activeQG_By_ID.ContainsKey(turnInQGID))
        {
            Quest_Giver QG = activeQG_By_ID[turnInQGID];

            QG.setState(evaluateQuestGiverState(turnInQGID));


            QG.GetComponentInParent<Messager>().nextMessage();

        }

        return true;
    }

    //helper
    bool objectivesComplete(Quest q)
    {
        if (q == null) Debug.Log("quest is null");
        
        foreach(Quest_Objective ob in q.objectives)
        {
            if (!ob.isComplete()) return false;
        }
        foreach (Talk_Objective ob in q.talk_objectives)
        {
            if (!ob.isComplete()) return false;
        }
        foreach (Gather_Objective ob in q.gather_objectives)
        {
            if (!ob.isComplete()) return false;
        }
        foreach (Slay_Objective ob in q.slay_objectives)
        {
            if (!ob.isComplete()) return false;
        }
        return true;

    }

    Quest addtoLog(int QID)
    {
        //questLog. 
        if (questLog.Count >= LOG_CAPACITY)
        {
            Debug.Log("Could not add to Log");
            return null;
        }
        
        Quest_Data qData = questDataByQID[QID];

        Quest quest = new Quest(qData);

        questLog.Add(quest);

        
        //do for each gather objective in this quest
        foreach(Gather_Objective ob in quest.gather_objectives)
        {
            if (!ob.getData().isRetroactive()) continue;
            //check inventory for counts of any retroactive gather objectives
            int count = inventory_Manager.countItem(ob.getData().getItemID());
            ob.countUpdate(count);
            if(ob.isComplete())
            {
                Debug.Log("Testing");
                checkObjectivesComplete(quest);
            }
        }

        return quest;
    }

    void removeFromLog(Quest quest)
    {

        questLog.Remove(quest);
    }

}

public enum QuestGiverState
{
    none, available, availableFull, active, turnIn, turnInFull
}




//public class QuestSort : IComparer<Quest>
//{
//    public int Compare(Quest q1, Quest q2)
//    {
//        if (q1.isMainQuest && !q2.isMainQuest) return -1;

//        if (q2.isMainQuest && !q1.isMainQuest) return 1;

//        if (q1.QID < q2.QID) return -1;

//        return 1;
//    }
//}

//used by Dialogue Manager to retrieve quest dialogue

//public Quest_Dialogue_Data setQuestMessage(int QGID)
//{

//    //Check for Turn In Quest
//    Quest pending = checkForPending(QGID);
//    if(pending != null)
//    {    
//        //there is a pending quest for this QG!
//        //return the turn in message
//        Quest_Dialogue_Data dialogue_data = questDataByQID[pending.QID].dialogue_data;

//        //check inventory full

//        //num items to receive:
//        int rewards = questDataByQID[pending.QID].numRewards();
//        if(rewards == 0 || inventory_Manager.enoughSpace(rewards))
//        {
//            activeQG_By_ID[QGID].setState(QuestGiverState.turnIn);
//            //return the turn in message
//            return dialogue_data;
//        }
//        else
//        {
//            activeQG_By_ID[QGID].setState(QuestGiverState.turnInFull);
//            return dialogue_data;
//        }
//    }

//    //Check for active quest
//    Quest activeQuest = checkForActive(QGID);
//    if(activeQuest != null)
//    {
//        Quest_Dialogue_Data dialogue_data = questDataByQID[activeQuest.QID].dialogue_data;
//        activeQG_By_ID[QGID].setState(QuestGiverState.active);
//        return dialogue_data;
//    }

//    //check for available quest
//    List<int> questsGiven = null;
//    Quests_by_QGID.TryGetValue(QGID, out questsGiven);
//    if(questsGiven != null && questsGiven.Count > 0)
//    {
//        int priorityQID = -1;

//        foreach(int qid in questsGiven)
//        {
//            if (!isAvailable(qid)) continue;

//            //return first main quest
//            if(questDataByQID[qid].isMainQuest)
//            {
//                //check if availableFull:



//                activeQG_By_ID[QGID].setState(QuestGiverState.available);
//                return questDataByQID[qid].dialogue_data;
//            }

//            //track the first non-main quest in this sorted list
//            if(priorityQID == -1)
//            {
//                priorityQID = qid;
//            }
//        }

//        //returning the highest priority non-main quest
//        if (priorityQID != -1)
//        {
//            activeQG_By_ID[QGID].setState(QuestGiverState.available);
//            return questDataByQID[priorityQID].dialogue_data;
//        }
//    }

//    Debug.Log("qgid: " + QGID + " null");

//    return null;

//}

