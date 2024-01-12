using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class SaveManager : MonoBehaviour
{
    Quest_Manager qm;
    Inventory_Manager invMan;
    Player player;
    Scene_Manager sceneMan;
    
    
    string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "saveData.dat";
    }

    private void OnEnable()
    {
        qm = this.GetComponent<Quest_Manager>();
        invMan = this.GetComponent<Inventory_Manager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sceneMan = this.GetComponent<Scene_Manager>();
    }

    public void saveGame()
    {

        SaveData sd = new SaveData();

        sd.setHealth(player.getHealth());
        sd.setInventory(invMan.serializeInventory());
        sd.setQuestCompletion(qm.getCompletion());
        sd.setQuestLog(qm.getSerialQuestLog());
        sd.setSceneName(sceneMan.getSceneName());
        sd.setEntranceNo(sceneMan.getEntrance().getEntranceNo());
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = File.Create(savePath);

        formatter.Serialize(fileStream, sd);
        fileStream.Close();

        Debug.Log("Saved file to: " + savePath);
    }

    SaveData loadData()
    {
        if(File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.Open);

            SaveData data = (SaveData)formatter.Deserialize(fileStream);
            fileStream.Close();

            return data;

        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
    }

    public void loadGame()
    {
                      
        SaveData data = loadData();
        Debug.Log(data != null);

        sceneMan.loadSaveFile(data);

    }

}

