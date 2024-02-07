using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Save Data", menuName = "Scene Save Data")]
[System.Serializable]
public class Scene_Save_Data : ScriptableObject
{
    [SerializeField]
    string sceneName;
    
    [SerializeField]
    Crop_Persistent_Data crop_data;

    public string getSceneName() { return this.sceneName; }
    public Crop_Persistent_Data getCropPersistentData() { return this.crop_data ?? null; }
}


