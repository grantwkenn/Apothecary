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
    List<KeyValuePair<int, string>> test;

    [SerializeField]
    Dictionary<Vector3Int, SerializableCrop> sCropMap;

    HashSet<Byte3> tilledTiles;
    HashSet<Byte3> wateredTiles;

    [SerializeField]
    List<Byte3> tilledTilesList;
    [SerializeField]
    List<Byte3> wateredTilesList;

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
        tilledTilesList = new List<Byte3>();
        wateredTilesList = new List<Byte3>();

        sCrops = new List<SerializableCrop>();
        sCropMap = new Dictionary<Vector3Int, SerializableCrop>();
    }
    
    public void init()
    {
        //repopulate the hashsets from their list form (Unity Serialization workaround...)


        if (sCrops == null) sCrops = new List<SerializableCrop>();

        tilledTiles = new HashSet<Byte3>();
        wateredTiles = new HashSet<Byte3>();


        if (tilledTilesList == null) tilledTilesList = new List<Byte3>();
        if (wateredTilesList == null) wateredTilesList = new List<Byte3>();


        foreach (Byte3 v3 in tilledTilesList) tilledTiles.Add(v3);
        foreach (Byte3 v3 in wateredTilesList) wateredTiles.Add(v3);


        sCropMap = new Dictionary<Vector3Int, SerializableCrop>();

        foreach(SerializableCrop sCrop in sCrops)
        {
            Byte3 b3 = sCrop.getLocationKey();
            sCropMap.Add(new Vector3Int(b3.x, b3.y, b3.z), sCrop);
        }

    }

    public void setDugTile(Byte3 point)
    {
        if(!tilledTiles.Contains(point))
        {
            tilledTiles.Add(point);
            tilledTilesList.Add(point);
        }        
    }

    public void setWateredTile(Byte3 point)
    {
        if(!wateredTiles.Contains(point))
        {
            wateredTiles.Add(point);
            wateredTilesList.Add(point);
        }
        
    }

    public List<Byte3> getTilledTiles() { return new List<Byte3>(this.tilledTiles != null ? this.tilledTiles : new List<Byte3>()); }
    public List<Byte3> getWateredTiles() { return new List<Byte3>(this.wateredTiles != null ? this.wateredTiles : new List<Byte3>()); }


    public void addCrop(Byte3 location, SerializableCrop sCrop)
    {
        sCrops.Add(sCrop);
        sCropMap.Add(location.toVector3Int(), sCrop);
    }

    public void removeCrop(Vector3Int location)
    {      
        sCrops.Remove(sCropMap[location]);
        sCropMap.Remove(location);
    }

    public Dictionary<Vector3Int, SerializableCrop> getSCrops()
    {
        return this.sCropMap;
    }

    public SerializableCrop getSCrop(Vector3Int key)
    {
        if (sCropMap.ContainsKey(key))
            return sCropMap[key];
        return null;

    }

    public void ageCrops()
    {
        foreach (SerializableCrop sCrop in sCrops)
        {
            sCrop.updateAge(1);
        }
    }

    public List<SerializableCrop> getSCropList() { return this.sCrops; }

    public void loadSaveData(crop_Save_Data csd)
    {
        this.sCrops = csd.sCrops;
        this.tilledTilesList = csd.tilledTiles;
        this.wateredTilesList = csd.wateredTiles;
    }
}

[System.Serializable]
public class crop_Save_Data
{
    public List<SerializableCrop> sCrops;
    public List<Byte3> tilledTiles;
    public List<Byte3> wateredTiles;

    public crop_Save_Data(Crop_Persistent_Data cpd)
    {
        this.sCrops = cpd.getSCropList();
        this.tilledTiles = cpd.getTilledTiles();
        this.wateredTiles = cpd.getWateredTiles();

    }
}
