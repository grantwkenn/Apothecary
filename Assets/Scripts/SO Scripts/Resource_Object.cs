using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//SCRIPT EXECUTION ORDER: it is best to go early since it is only responsible for holding references and
// can be used by any scripts

[CreateAssetMenu(fileName = "Resources Object", menuName = "Resources Object")]
public class Resource_Object : ScriptableObject
{
    //TODO setup a Dictionary<type, Dictionary<string, object>>
    //so I can call one function to search all resources by just the type, and the name of the asset

    [SerializeField]
    List<Sprite> sprites;

    [SerializeField]
    Sprite[] questSymbols;

    [SerializeField]
    GameObject[] prefabs;

    [SerializeField]
    Player_Persistence playerPersistence;

    [SerializeField]
    TileBase tilledTileBase, wateredTileBase;

    Dictionary<string, int> prefabMap;


    public Sprite getSprite(int index)
    {
        if (index < 0 || index >= sprites.Count) return null;
        return sprites[index];
    }

    public Sprite getResource(string name, int index)
    {
        return null;
    }

    public Player_Persistence getPlayerPersistence() { return this.playerPersistence; }

    public GameObject getPrefab(string name) { return prefabs[prefabMap[name]]; }

    public void OnValidate()
    {
        //re map all prefabs by name when change is made in Editor
        if (prefabMap == null) prefabMap = new Dictionary<string, int>();
        prefabMap.Clear();
        for(int i=0; i<prefabs.Length; i++)
        {
            if (!prefabMap.TryAdd(prefabs[i].name, i)) Debug.Log("Mapping Error");
        }


    }

    public Sprite[] getQuestSymbols()
    {
        return this.questSymbols;
    }

    public List<TileBase> cropManagerInit()
    {
        List<TileBase> list = new List<TileBase>();
        list.Add(tilledTileBase); 
        list.Add(wateredTileBase);
        return list;
    }

}
