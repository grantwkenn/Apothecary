using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class Player_Persistence : ScriptableObject
{
    //TODO add current selection
    
    [SerializeField]
    int health;

    [SerializeField]
    Item[] items;

    byte invSelection;

    byte inventorySize;

    [SerializeField]
    byte entranceNo;

    bool changingScenes;

    bool[] questsComplete;
    List<Quest> questLog;


    private void OnValidate()
    {


    }

    public void setEntrance(byte entranceNo)
    {
        this.entranceNo = entranceNo;
            
    }

    public byte getEntranceNo() { return entranceNo; }

    public int getHealth() { return health; }

    public void setHealth(int health) { this.health = health; }

    public bool isChangingScenes() { return changingScenes; }

    public void setChangingScenes(bool b) { changingScenes = b; }

    public void setItem(int index, Item item)
    {
        this.items[index] = item;
    }

    public void setItems(Item[] items)
    {
        this.items = new Item[items.Length];

        for(int i = 0; i<items.Length; i++)
        {
            this.items[i] = items[i];
        }
    }

    public Item[] getItems() {
        Item[] result = new Item[44];
        for(int i =0; i< 44; i++)
        {
            
            //Debug.Log(items[i].GetType());
            //if (consumables!= null && consumables[i] != null) result[i] = consumables[i];
            //else result[i] = this.items[i];
            result[i] = this.items[i];
        }
        

        return result;
    
    }

    public void setInvSelection(byte sel) { this.invSelection = sel; }

    public byte getInvSelection() { return this.invSelection; }

    public void storeQuestData(bool[] questsComplete, List<Quest> questLog)
    {
        this.questLog = new List<Quest>();
        foreach(Quest q in questLog)
        {
            this.questLog.Add(q);
        }
        this.questsComplete = new bool[questsComplete.Length];
        for (int b = 0; b < questsComplete.Length; b++)
        {
            this.questsComplete[b] = questsComplete[b];
        }
    }


    public void loadQuestData(ref bool[] _questsComplete, ref List<Quest> _questLog)
    {
        _questsComplete = new bool[questsComplete.Length];
        for(int b = 0; b< questsComplete.Length; b++)
        {
            _questsComplete[b] = this.questsComplete[b];
        }

        _questLog = new List<Quest>();
        foreach(Quest q in questLog)
        {
            _questLog.Add(q);
        }
    }
}


