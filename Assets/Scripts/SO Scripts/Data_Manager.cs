using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Manager : MonoBehaviour
{
    Reference_Manager rm;
    
    
    [SerializeField]
    Data_Persistence gameData;


    //option one:
    //copy a dup list of save data info.
    //also maintain the savedata file.. why?

    //on scene transition: store all persistent data in DM

    //on save: store all save data (the current Data Manager "state") to disk

    //on load: copy the state of data manager from the save file before scene exit

    public void OnEnable()
    {
        rm = this.GetComponent<Reference_Manager>();

        gameData = rm.getDataPersistence();
        
        gameData.init();

    }


    public void loadFile(SaveData file)
    {
        //set the Data Persistence object using the save file
        gameData.loadDataFromSave(file);
    }

    //produce the list of Scene Save Data from the list of SceneData stored in DP object
    public List<Scene_Save_Data> getSceneSaveData() {

        List<Scene_Save_Data> list = new List<Scene_Save_Data>();
        foreach(Scene_Data sd in gameData.allSceneData)
        {
            crop_Save_Data csd = null;
            if (sd.getCropPersistentData() != null) csd = new crop_Save_Data(sd.getCropPersistentData());
            list.Add(new Scene_Save_Data(sd.name, csd));
        }
        return list;
    }
}
