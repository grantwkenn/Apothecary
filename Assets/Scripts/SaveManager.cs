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

    Event_Log eventLog;

    string[] savePath;
    
    //string[] savePath = { 
    //    Application.persistentDataPath + "saveData1.dat",
    //    Application.persistentDataPath + "saveData2.dat",
    //    Application.persistentDataPath + "saveData3.dat",
    //    Application.persistentDataPath + "saveData4.dat",
    //    Application.persistentDataPath + "saveData5.dat"
    //};

    [SerializeField]
    List<Scene_Save_Data> sceneData;

    private void Awake()
    {
        savePath = new string[1];
        savePath[0] = Application.persistentDataPath + "saveData1.dat";
    }

    private void OnEnable()
    {
        qm = this.GetComponent<Quest_Manager>();
        invMan = this.GetComponent<Inventory_Manager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sceneMan = this.GetComponent<Scene_Manager>();
        mm = this.GetComponent<Menu_Manager>();
        dm = this.GetComponent<Data_Manager>();
        eventLog = this.GetComponent<Event_Log>();

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
        FileStream fileStream = File.Create(savePath[0]);

        formatter.Serialize(fileStream, sd);
        fileStream.Close();

        Debug.Log("Saved file to: " + savePath[0]);
        eventLog.logEvent("Saved file to:\n" + savePath[0]);
    }

    SaveData loadData()
    {
        if(File.Exists(savePath[0]))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath[0], FileMode.Open);

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

