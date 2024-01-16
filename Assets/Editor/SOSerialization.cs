using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


//This is a tool for serializing Item Data (and any other data in the future)
// to a JSON file.

//It will then replace the Scriptable Objects in the editor with whatever is stored in corresponding JSON file.

public class SOSerialization : MonoBehaviour
{
    static string pathToSOs = "Assets/Scriptable Objects/Test Folder";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("Tools/Serialize SOs")]
    private static void ExecuteMyEditorScript()
    {
        Debug.Log("Executing My Editor Script!");
        // Add your script logic here
        string folderPath = pathToSOs; 

        // Find all assets of type ScriptableObject in the specified folder
        string[] guids = AssetDatabase.FindAssets("t:Item_Data", new[] { folderPath });


        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Item_Data data = AssetDatabase.LoadAssetAtPath<Item_Data>(assetPath);

            if (data != null)
            {
                // Do something with the ScriptableObject (e.g., print its name)
                string path = "Assets/JSON/" + data.name + ".json";

                string jsonData = JsonUtility.ToJson(data);

                System.IO.File.WriteAllText(path, jsonData);


            }
        }



    }

    [MenuItem("Tools/Load SOs")]
    private static void LoadSOs()
    {
        string folderPath = pathToSOs;
        string jsonDataFolderPath = "Assets/JSON";

        // Find all assets of type ScriptableObject in the specified folder
        string[] guids = AssetDatabase.FindAssets("t:Item_Data", new[] { folderPath });


        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Item_Data data = AssetDatabase.LoadAssetAtPath<Item_Data>(assetPath);

            if (data != null)
            {
                
                // Read JSON data from the file
                string jsonFileName = data.name + ".json";
                string jsonFilePath = System.IO.Path.Combine(jsonDataFolderPath, jsonFileName);

                if (System.IO.File.Exists(jsonFilePath))
                {
                    // Read JSON data from the file
                    string loadedJsonData = File.ReadAllText(jsonFilePath);

                    ItemDataJSON loadedData = JsonUtility.FromJson<ItemDataJSON>(loadedJsonData);

                    string oldName = data.name;

                    AssetDatabase.DeleteAsset(assetPath);

                    Item_Data newSO = ScriptableObject.CreateInstance<Item_Data>();

                    string newFilePath = pathToSOs + "/" + oldName + ".asset";
                    Debug.Log(newFilePath);

                    AssetDatabase.CreateAsset(newSO, newFilePath);

                    //replace asset values
                    newSO.replaceData(loadedData);


                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                }
                else
                {
                    Debug.LogError("JSON file not found for ScriptableObject: " + data.name);
                }
            }
        }
    }



}


