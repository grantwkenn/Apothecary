using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "crop_pers_data", menuName = "Crop Persistence Data")]
public class Crop_Persistent_Data : ScriptableObject
{
    //TODO, this SO relies on a redundent List / Dictionary structure
    // to store tilled and watered tiles, may or may not be using twice the memory per tile 

    [SerializeField] bool wipe_data;


    //this SO stores the location of tilled tiles, watered tiles, and crops at each level
    //the z value of each V3Int stores the level number

    [SerializeField]
    List<SerializableCrop> sCrops;

    [SerializeField]
    List<Vector3Int> cropKeys;

    [SerializeField]
    List<KeyValuePair<int, string>> test;

    [SerializeField]
    Dictionary<Vector3Int, SerializableCrop> sCropMap;

    HashSet<Vector3Int> tilledTiles;
    HashSet<Vector3Int> wateredTiles;

    List<Vector3Int> tilledTilesList;
    List<Vector3Int> wateredTilesList;

    private void OnValidate()
    {
        if(wipe_data)
        {
            wipe_data = false;
            wipeData();
        }
    }

    public void wipeData()
    {
        tilledTilesList = new List<Vector3Int>();
        wateredTilesList = new List<Vector3Int>();

        sCrops = new List<SerializableCrop>();
        cropKeys = new List<Vector3Int>();
        sCropMap = new Dictionary<Vector3Int, SerializableCrop>();
    }
    
    public void init()
    {
        //repopulate the hashsets from their list form (Unity Serialization workaround...)


        if (sCrops == null) sCrops = new List<SerializableCrop>();
        if (cropKeys == null) cropKeys = new List<Vector3Int>();

        tilledTiles = new HashSet<Vector3Int>();
        wateredTiles = new HashSet<Vector3Int>();


        if (tilledTilesList == null) tilledTilesList = new List<Vector3Int>();
        if (wateredTilesList == null) wateredTilesList = new List<Vector3Int>();


        foreach (Vector3Int v3 in tilledTilesList) tilledTiles.Add(v3);
        foreach (Vector3Int v3 in wateredTilesList) wateredTiles.Add(v3);


        sCropMap = new Dictionary<Vector3Int, SerializableCrop>();

        foreach(Vector3Int v3 in cropKeys)
        {
            int index = cropKeys.IndexOf(v3);
            sCropMap.Add(v3, sCrops[index]);
        }

    }

    public void setDugTile(Vector3Int point)
    {
        if(!tilledTiles.Contains(point))
        {
            tilledTiles.Add(point);
            tilledTilesList.Add(point);
        }        
    }

    public void setWateredTile(Vector3Int point)
    {
        if(!wateredTiles.Contains(point))
        {
            wateredTiles.Add(point);
            wateredTilesList.Add(point);
        }
        
    }

    public List<Vector3Int> getTilledTiles() { return new List<Vector3Int>(this.tilledTiles != null ? this.tilledTiles : new List<Vector3Int>()); }
    public List<Vector3Int> getWateredTiles() { return new List<Vector3Int>(this.wateredTiles != null ? this.wateredTiles : new List<Vector3Int>()); }

    public SerializableCrop getSCrop(Vector3Int key) 
    {
        if (!sCropMap.ContainsKey(key)) return null;
        return sCropMap[key];
    }

    public void addCrop(Vector3Int location, SerializableCrop sCrop)
    {
        sCrops.Add(sCrop);
        cropKeys.Add(location);
        sCropMap.Add(location, sCrop);
    }

    public void removeCrop(Vector3Int location)
    {
        //TODO ensure this Find function is matching by equality and not address
        int index = cropKeys.IndexOf(location);
        cropKeys.RemoveAt(index);
        sCrops.RemoveAt(index);
        sCropMap.Remove(location);

    }

    public Dictionary<Vector3Int, SerializableCrop> getSCrops()
    {
        return this.sCropMap;
    }

    public void ageCrops()
    {
        foreach (SerializableCrop sCrop in sCrops)
        {
            sCrop.updateAge();
        }
    }
     
}
