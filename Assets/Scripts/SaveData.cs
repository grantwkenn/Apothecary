using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public byte entranceNo;
    
    public int health;
    public List<SerializableItem> inventory;

    public bool[] questsComplete;
    public List<SerializableQuest> questLog;

    public List<Scene_Save_Data> sceneSaveData;

}





