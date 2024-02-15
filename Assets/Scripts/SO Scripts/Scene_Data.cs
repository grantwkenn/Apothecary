using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "XXSceneXX Data", menuName = "Scene Data")]
[ExecuteInEditMode]
public class Scene_Data : ScriptableObject
{
    [SerializeField]
    Object scene;

    [SerializeField]
    string sceneName;

    [SerializeField]
    Crop_Persistent_Data crop_data;

    public string getSceneName() { return this.sceneName; }
    public Crop_Persistent_Data getCropPersistentData() { return this.crop_data ?? null; }

    public Scene_Save_Data getSceneSaveData()
    {
        return new Scene_Save_Data(sceneName, new crop_Save_Data(crop_data));
    }

    private void OnValidate()
    {
        if(scene != null) sceneName = scene.name;      
    }

    public void loadFromSave(Scene_Save_Data ssd)
    {
        if (ssd.crop_data == null || crop_data == null) return;
        
        this.crop_data.loadSaveData(ssd.crop_data);
    }

}

[System.Serializable]
public class Scene_Save_Data
{
    [SerializeField]
    public string sceneName;

    [SerializeField]
    public crop_Save_Data crop_data;

    public Scene_Save_Data(string name, crop_Save_Data csd)
    {
        this.sceneName = name;
        this.crop_data = csd;
    }


}


