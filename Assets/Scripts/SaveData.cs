using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    string sceneName;
    
    int health;
    List<SerializableItem> inventory;

    bool[] questsComplete;
    List<SerializableQuest> questLog;

    public void setHealth(int _health) { this.health = _health; }

    public void setInventory(List<SerializableItem> _items) { this.inventory = _items; }

    public void setQuestCompletion(bool[] qc) { this.questsComplete = qc; }

    public void setQuestLog(List<SerializableQuest> quests) { this.questLog = quests; }

    public int getHealth() { return this.health; }

    public List<SerializableItem> getSerializedItems() { return this.inventory; }

    public List<SerializableQuest> getSerializedQuests() { return this.questLog; }

    public string getSceneName() { return this.sceneName; }

    public void setSceneName(string scn) { this.sceneName = scn; }
}





