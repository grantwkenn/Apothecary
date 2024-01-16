using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class Player_Persistence : ScriptableObject
{
    //TODO add current selection

    [SerializeField]
    SaveData save;
    
    [SerializeField]
    int health;


    [SerializeField]
    List<SerializableItem> sInventory;


    byte invSelection;

    byte inventorySize;

    [SerializeField]
    byte entranceNo;

    bool changingScenes;

    [SerializeField]
    bool[] questsComplete;

    [SerializeField]
    List<SerializableQuest> newquestLog;


    public void setEntrance(byte entranceNo)
    {
        this.entranceNo = entranceNo;
            
    }

    public byte getEntranceNo() { return entranceNo; }

    public int getHealth() { return health; }

    public void setHealth(int health) { this.health = health; }

    public bool isChangingScenes() { return changingScenes; }

    public void setChangingScenes(bool b) { changingScenes = b; }



    public void setItems(List<SerializableItem> sItems)
    {
        this.sInventory = sItems;
    }
    

    public List<SerializableItem> getSerializableInventory()
    {
        return this.sInventory;
    }

    public void setInvSelection(byte sel) { this.invSelection = sel; }

    public byte getInvSelection() { return this.invSelection; }


    public void newStoreQuestData(bool[] questsComplete, List<SerializableQuest> squests)
    {
        this.newquestLog = new List<SerializableQuest>();
        foreach(SerializableQuest sq in squests)
        {
            newquestLog.Add(sq);
        }
        this.questsComplete = new bool[questsComplete.Length];
        for (int b = 0; b < questsComplete.Length; b++)
        {
            this.questsComplete[b] = questsComplete[b];
        }
    }

    public void getQuestData(ref bool[] completion, ref List<SerializableQuest> squests)
    {
        if(this.questsComplete.Length == completion.Length)
        {
            for (int i = 0; i < questsComplete.Length; i++)
            {
                completion[i] = this.questsComplete[i];
            }
        }

        
        foreach (SerializableQuest sq in newquestLog)
        {
            squests.Add(sq);
        }

    }


    public void setSaveFile(SaveData _save)
    {
        this.save = _save;

        this.newquestLog = save.getSerializedQuests();
        this.questsComplete = save.getQuestCompletion();

        this.sInventory = new List<SerializableItem>(save.getSerializedItems());
        //TODO could set the inventory selection here
    }
    

    public SaveData getSaveData() { return this.save; }

    public bool fromSave() { return this.save != null; }

}


