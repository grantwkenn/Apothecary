using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class Data_Persistence : ScriptableObject
{
    //TODO add current selection

    
    [SerializeField]
    SaveData save;
    
    [SerializeField]
    int health;

    [SerializeField]
    int coins;


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
    List<SerializableQuest> sQuestLog;

    [SerializeField]
    public List<Scene_Data> allSceneData;

    Dictionary<string, int> sceneDataByName;

    public void init()
    {
        sceneDataByName = new Dictionary<string, int>();
        int index = 0;
        foreach(Scene_Data sd in allSceneData)
        {
            sceneDataByName.Add(sd.name, index);
            index++;
        }
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



    public void setItems(List<SerializableItem> sItems)
    {
        this.sInventory = sItems;
    }
    

    public List<SerializableItem> getSerializableInventory()
    {
        return this.sInventory;
    }

    public int getCoins()
    {
        return this.coins;
    }

    public void setCoins(int amt) { this.coins = amt; }

    public void setInvSelection(byte sel) { this.invSelection = sel; }

    public byte getInvSelection() { return this.invSelection; }


    public void newStoreQuestData(bool[] questsComplete, List<SerializableQuest> squests)
    {
        this.sQuestLog = new List<SerializableQuest>();
        foreach(SerializableQuest sq in squests)
        {
            sQuestLog.Add(sq);
        }
        this.questsComplete = new bool[questsComplete.Length];
        for (int b = 0; b < questsComplete.Length; b++)
        {
            this.questsComplete[b] = questsComplete[b];
        }
    }

    public void getQuestData(ref bool[] completion, ref List<SerializableQuest> squests)
    {
        squests = new List<SerializableQuest>();
        
        if(this.questsComplete.Length == completion.Length)
        {
            for (int i = 0; i < questsComplete.Length; i++)
            {
                completion[i] = this.questsComplete[i];
            }
        }

        
        foreach (SerializableQuest sq in sQuestLog)
        {
            squests.Add(sq);
        }

    }


    public void loadDataFromSave(SaveData file)
    {
        this.save = file;

        this.sQuestLog = save.questLog;
        this.questsComplete = save.questsComplete;
        this.coins = save.coins;
        this.sInventory = save.inventory;
        //TODO could set the inventory selection here

        foreach(Scene_Save_Data ssd in file.sceneSaveData)
        {
            allSceneData[sceneDataByName[ssd.sceneName]].loadFromSave(ssd);
        }
    }


    public SaveData getSaveData() { return this.save; }

    public bool fromSave() { return this.save != null; }



}


