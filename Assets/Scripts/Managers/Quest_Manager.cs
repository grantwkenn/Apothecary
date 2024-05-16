using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public enum QuestGiverState
{
    none, available, availableFull, active, turnIn, turnInFull
}


public class Quest_Manager : MonoBehaviour
{
    public bool debugMode;
    
    //Reference
    Inventory_Manager inventory_Manager;
    Dialogue_Manager dialogueManager;
    Scene_Manager sm;
    Data_Persistence dp;


    
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
    private bool[] questsComplete;


    Dictionary<int, Quest_Giver> activeQG_By_ID;

    Dictionary<int, int> currentQuest_by_QGID;


    // QUEST LOG//////////////////////////////////
    [SerializeField]
    List<Quest> questLog;

    [SerializeField]
    Sprite[] questSymbols;

    const int LOG_CAPACITY = 20;


    private void OnEnable()
    {
        //after Game Manager, Inventory Manager
        inventory_Manager = this.GetComponentInParent<Inventory_Manager>();
        dialogueManager = this.GetComponentInParent<Dialogue_Manager>();
        sm = this.GetComponentInParent<Scene_Manager>();

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

        }

        //sort all Quests_by_QGID lists by priority
        //This is not a proper priority sorting, main quests are not in front, simply by qid
        foreach (KeyValuePair<int, List<int>> questList in Quests_by_QGID)
        {
            questList.Value.Sort();
        }

        Reference_Manager resoMan;
        //get the quest indicator prefab from the Resource Manager
        resoMan = this.GetComponent<Reference_Manager>();

        questSymbols = resoMan.getQuestSymbols();

    }

    void Start()
    {
        //This must wait until after Scene Manager has OnEnabled?? TODO investigate execution order here
        dp = this.GetComponent<Scene_Manager>().getDataPersistence();

        
        loadQuestProgress();

    }

    void loadQuestProgress()
    {
        //call on Scene Manager to populate questLog and completion array
        List<SerializableQuest> squests = new List<SerializableQuest>();
        questsComplete = new bool[questCount];
        sm.getQuestData(ref questsComplete, ref squests);

        questLog = new List<Quest>();

        foreach (SerializableQuest sq in squests)
        {
            Quest_Data qd = this.questDataByQID[sq.getID()];
            Quest q = new Quest(qd);
            q.setProgress(sq);
            questLog.Add(q);
        }


    }

    public void storePersistenceData()
    {
        //pp.storeQuestData(questsComplete, questLog);
        dp.newStoreQuestData(questsComplete, serializeQuestLog());
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
                if (to.getData().NPC_ID != messager.getID()) continue;

                //return the talk objective message
                if (!to.isComplete())
                {
                    messager.setSymbol(2);
                    return to.getData().getResponse();
                    
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
                if (to.getData().NPC_ID != messager.getID()) continue;


                //NOTE this may not work with the comparison. IF not, find proper way to compare two messages.
                if (Dialogue_Manager.compareMessages(messager.getMessage(), to.getData().getResponse()))
                {
                    //set this talk objective completed
                    to.dialogueComplete();

                    if(objectivesComplete(quest))
                        setActiveQGTurnIn(quest);
                        
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
        Debug.Log(state.ToString());
        
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
                || !inventory_Manager.isSpaceForItems(questDataByQID[QID].getQuestItems()))
            {
                //set QG state, 
                QG.setState(QuestGiverState.availableFull);
            }
            
            else
            {
                //accept this quest
                Quest addedQuest = addtoLog(QID);
                QG.setState(QuestGiverState.active);

                if(objectivesComplete(addedQuest))
                    setActiveQGTurnIn(addedQuest);

                foreach(Talk_Objective to in addedQuest.talk_objectives)
                {
                    if (!activeQG_By_ID.ContainsKey(to.getData().NPC_ID)) continue;

                    Quest_Giver QG2 = activeQG_By_ID[to.getData().NPC_ID];
                    QG2.setState(evaluateQuestGiverState(to.getData().NPC_ID));
                    QG2.nextMessage();
                }

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


            if(!inventory_Manager.isSpaceForItems(quest.getData().getRewards()))
            {
                //set full state
                QG.setState(QuestGiverState.turnInFull);
            }

            else //enough space, complete quest
            {
                Quest_Data qd = quest.getData();

                removeFromLog(quest);


                questsComplete[qd.getQuestID()] = true;

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


                List<Talk_Objective_Data> toList = qd.getTalkObjectives();
                foreach(Talk_Objective_Data tod in toList)
                {
                    if(activeQG_By_ID.ContainsKey(tod.NPC_ID))
                    {
                        Quest_Giver qg = activeQG_By_ID[tod.NPC_ID];
                        qg.setState(evaluateQuestGiverState(qg.QGID));
                        Messager msgr = qg.GetComponentInParent<Messager>();
                        msgr.nextMessage();

                    }
                }



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

                    if (inventory_Manager.isSpaceForItems(questDataByQID[qid].getQuestItems()))
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

                if (inventory_Manager.isSpaceForItems(questDataByQID[priorityQID].getQuestItems()))
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

        }

        return pending;      
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
                if (to.getData().NPC_ID == QGID)
                    return quest;
            }
        }
        

        return null;
    }

    bool isAvailable(int QID)
    {
        if (questsComplete[QID]) return false;
        
        Quest_Data data = questDataByQID[QID];
        int[] preQuests = data.getPreQuests();

        if (preQuests == null || preQuests.Length == 0) return true;
        
        foreach(int preQuest in preQuests)
        {
            if (!questsComplete[preQuest]) return false;
        }
        
        return true;
    }


    public void itemRemoved(int itemID, int remaining)
    {
        foreach(Quest quest in questLog)
        {
            foreach(Gather_Objective go in quest.gather_objectives)
            {
                if (itemID != go.getData().getItemID()) continue;

                //this item matches this gather objective
                bool wasComplete = go.isComplete() && objectivesComplete(quest);

                if(remaining < go.getCount())
                    go.countUpdate(0 - go.getCount() - remaining);

                //if no longer complete
                if(wasComplete && !go.isComplete())
                {
                    //need to re-evaluate this quest
                    //find quest Giver
                    if(activeQG_By_ID.ContainsKey(quest.getData().getTurnInQGID()))
                    {
                        Quest_Giver QG = activeQG_By_ID[quest.getData().getTurnInQGID()];
                        //re-evaluate state
                        QG.setState(evaluateQuestGiverState(QG.QGID));
                        QG.nextMessage();

                    }
                    
                }
            }
        }
    }
    
    
    public void itemAdded(int itemID, int Quantity)
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
                    if(objectivesComplete(quest))
                        setActiveQGTurnIn(quest);
                }

            }
        }
        
    }

    void setActiveQGTurnIn(Quest quest)
    {

        int turnInQGID = quest.getData().getTurnInQGID();

        if (activeQG_By_ID.ContainsKey(turnInQGID))
        {
            Quest_Giver QG = activeQG_By_ID[turnInQGID];

            QG.setState(evaluateQuestGiverState(turnInQGID));

            QG.GetComponentInParent<Messager>().nextMessage();

        }

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
                if(objectivesComplete(quest))
                    setActiveQGTurnIn(quest);
            }
        }

        return quest;
    }

    void removeFromLog(Quest quest)
    {

        questLog.Remove(quest);
    }

    public List<Quest> getLog() { return this.questLog; }

    public Sprite getQuestSymbol(int index)
    {
        return questSymbols[index];
    }

    public bool[] getCompletion() { return questsComplete; }

    public List<SerializableQuest> serializeQuestLog()
    {
        List<SerializableQuest> list = new List<SerializableQuest>();
        
        foreach(Quest q in questLog)
        {
            SerializableQuest sq = q.serializeQuest();
            if (sq != null)
                list.Add(sq);
        }

        return list;
    }


}

[System.Serializable]
public class SerializableQuest
{
    [SerializeField]
    int QuestID;

    [SerializeField]
    List<int> objectiveProgress;

    public SerializableQuest(int qid, List<int> objs)
    {
        this.QuestID = qid;
        this.objectiveProgress = objs;
    }

    public int getID() { return QuestID; }

    public List<int> getObjectiveProgress() { return this.objectiveProgress; }
}





