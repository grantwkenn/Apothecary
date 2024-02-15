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
    Menu_Manager mm;
    Data_Manager dm;
    
    string savePath;

    [SerializeField]
    List<Scene_Save_Data> sceneData;

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
        mm = this.GetComponent<Menu_Manager>();
        dm = this.GetComponent<Data_Manager>();

    }

    public void saveGame()
    {
        SaveData sd = new SaveData();

        sd.health = player.getHealth();
        sd.inventory = invMan.serializeInventory();
        sd.questsComplete = qm.getCompletion();
        sd.questLog = qm.serializeQuestLog();
        sd.sceneName = sceneMan.getSceneName();
        sd.entranceNo = sceneMan.getEntrance().getEntranceNo();
        sd.sceneSaveData = dm.getSceneSaveData();
        
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

        mm.inputCloseMenu();

        sceneMan.loadGame(data);

        dm.loadFile(data);

    }

}

