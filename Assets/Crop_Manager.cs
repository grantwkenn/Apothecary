using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum soilState
{
    tillable, tilled, watered, planted
}

//TODO each scene shall have its own Persistence object which stores crop data

public class Crop_Manager : MonoBehaviour
{
    Layer_Manager lm;
    Tile_Manager tm;
    Scene_Manager sm;
    Resource_Manager rm;

    //Get this from the editor for now,
    //maybe later the resource manager or scene manager will find it in the scene's folder or hashed by scene
    [SerializeField]
    Crop_Persistent_Data cropPersistentData;

    [SerializeField]
    SO_Collection cropData;

    Dictionary<string, Crop_Data> cropDataMap;

    Dictionary<byte, Tilemap> tillableTilemaps;
    Dictionary<byte, Tilemap> tilledTilemaps;
    Dictionary<byte, Tilemap> wateredTilemaps;

    Dictionary<Vector3Int, Crop> crops;

    //TODO assign these sprites automatically using resource management?
    TileBase tilledDirtTile, wateredDirtTile;

    [SerializeField] Tilemap currentTillableTilemap;
    Tilemap currentTilledTilemap;
    Tilemap currentWateredTilemap;

    [SerializeField] bool debug;

    sbyte[] seedOffsets = { 0, 0, 1, -1, 2, -2 };

    Dictionary<Vector3Int, Crop> cropMap;

    Dictionary<byte, Transform> cropParents;


    private void OnEnable()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");

        //find all the tillable tilemaps in the scene
        lm = GameManager.GetComponent<Layer_Manager>();
        tm = GameManager.GetComponent<Tile_Manager>();
        sm = GameManager.GetComponent<Scene_Manager>();
        rm = GameManager.GetComponent<Resource_Manager>();

        List<TileBase> tResources = rm.cropManagerInit();
        tilledDirtTile = tResources[0];
        wateredDirtTile = tResources[1];

        tillableTilemaps = new Dictionary<byte, Tilemap>();
        tilledTilemaps = new Dictionary<byte, Tilemap>();
        wateredTilemaps = new Dictionary<byte, Tilemap>();

        cropMap = new Dictionary<Vector3Int, Crop>();
        populateCropDataMap();

        InstantiateCropParents();

        foreach (GameObject grid in GameObject.FindGameObjectsWithTag("Grid"))
        {
            byte levelNo = (byte)int.Parse(grid.name.Split(" ")[0]);

            Transform tillable = grid.transform.Find(levelNo + " Tillable");
            Transform tilled = grid.transform.Find(levelNo + " Tilled");
            Transform watered = grid.transform.Find(levelNo + " Watered");
            if (tillable != null)
            {
                tillableTilemaps.Add(levelNo, tillable.GetComponent<Tilemap>());
            }
            if(tilled != null)
            {
                tilledTilemaps.Add(levelNo, tilled.GetComponent<Tilemap>());
            }
            if (watered != null)
            {
                wateredTilemaps.Add(levelNo, watered.GetComponent<Tilemap>());
            }
        }



    }

    void InstantiateCropParents()
    {
        cropParents = new Dictionary<byte, Transform>();

        List <GameObject> list = lm.getAllLevels();
        foreach(GameObject go in list)
        {
            string levelNo = go.name.Split(" ")[1];
            //find the "object" transform
            Transform parent = go.transform.Find(levelNo + " Object");

            //add a gameobject parent "Crops"
            GameObject g = new GameObject();
            g.name = levelNo + " Crop Clones";
            GameObject cropParent = GameObject.Instantiate(g, parent);

            //store the reference in a list / dictionary
            cropParents.Add((byte)int.Parse(levelNo), cropParent.transform);
        }
    }

    void populateCropDataMap()
    {
        cropDataMap = new Dictionary<string, Crop_Data>();
        foreach (ScriptableObject so in cropData.getCollection())
        {
            Crop_Data cd = (Crop_Data)so;
            cropDataMap.Add(cd.getName(), cd);
        }
    }

    void loadCropPersistenceData()
    {
        if (debug) return;

        cropPersistentData.init();

        //z values of the V3Int represents the level it belongs to. Ignore the "z" coordinate of 
        // the tiles by setting it to zero, relative to the rest of the tilemap (won't need this?)
        //set all the tilledTiles
        foreach(Byte3 tileLoc in cropPersistentData.getTilledTiles())
        {
            tilledTilemaps[(byte)tileLoc.z].SetTile(new Vector3Int(tileLoc.x, tileLoc.y, 0), tilledDirtTile);
        }
        //set all watered tiles
        foreach(Byte3 tileLoc in cropPersistentData.getWateredTiles())
        {
            wateredTilemaps[(byte)tileLoc.z].SetTile(new Vector3Int(tileLoc.x, tileLoc.y, 0), wateredDirtTile);
        }

        Dictionary<Byte3, SerializableCrop> sCrops = cropPersistentData.getSCrops();

        //TODO map new crop instantiated objects from serializable crops stored in cropPersistentData

        foreach (Byte3 b3 in sCrops.Keys)
        {
            SerializableCrop scrop = sCrops[b3];

            Vector3 offset = scrop.getOffset();
            Vector3 position = new Vector3(b3.x + offset.x + 0.5f, b3.y + offset.y + 0.5f, b3.y + offset.y + 0.5f);

            Transform level = lm.getLevel(b3.z);

            Crop newCrop = instantiateCrop(position, b3.z);

            newCrop.setData(scrop, cropDataMap[scrop.getName()]);

            cropMap.Add(new Vector3Int(b3.x, b3.y, b3.z), newCrop);

        }
    }

    Crop instantiateCrop(Vector3 position, byte level)
    {
        //TODO pass the crop parameter and dynamically load the correct prefab crop (Another data mapping for Resource Man)

        //instantiate the crop here
        Transform parent = cropParents[level];

        //TODO set the parent to be a crop container instead of the root of "Level 0" for ex
        GameObject newCrop = GameObject.Instantiate(rm.getPrefab("New Crop"), position, Quaternion.identity, parent);

        //TODO make a level script with a function for finding the object child object without needing to find with text..?
        
        lm.relayerMe(newCrop.GetComponent<SpriteRenderer>(), parent.parent.name);

        return newCrop.GetComponent<Crop>();

    }

    // Start is called before the first frame update
    void Start()
    {
        updatePlayerLevel();
        loadCropPersistenceData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updatePlayerLevel()
    {
        byte playerLevel = sm.getPlayerLevel();
        tillableTilemaps.TryGetValue(playerLevel, out currentTillableTilemap);
        tilledTilemaps.TryGetValue(playerLevel, out currentTilledTilemap);
        wateredTilemaps.TryGetValue(playerLevel, out currentWateredTilemap);
    }

    public bool hasTillableTile(Vector3Int target)
    {
        return currentTillableTilemap != null && currentTillableTilemap.HasTile(target);
    }

    public bool hasTilledTile(Vector3Int target)
    {
        return currentTilledTilemap != null && currentTilledTilemap.HasTile(target);
    }    

    public void tillTile(Vector3Int targetTile)
    {
        if (currentTillableTilemap == null
        || currentTilledTilemap == null
        || currentWateredTilemap == null
        || !currentTillableTilemap.HasTile(targetTile) 
        || currentTilledTilemap.HasTile(targetTile) 
        || currentWateredTilemap.HasTile(targetTile)) return;

        currentTilledTilemap.SetTile(targetTile, tilledDirtTile);
        Byte3 locationKey = new Byte3(targetTile);
        locationKey.z = sm.getPlayerLevel();

        cropPersistentData.setDugTile(locationKey);

    }

    //TODO move to crop manager
    public void waterTile(Vector3Int targetTile)
    {
        if (currentTillableTilemap == null
        || currentTilledTilemap == null
        || currentWateredTilemap == null
        || !currentTilledTilemap.HasTile(targetTile)
        || currentWateredTilemap.HasTile(targetTile)) return;

        if (currentTilledTilemap.HasTile(targetTile) && !currentWateredTilemap.HasTile(targetTile))
        {
            currentWateredTilemap.SetTile(targetTile, wateredDirtTile);

            Byte3 locationKey = new Byte3(targetTile);
            locationKey.z = sm.getPlayerLevel();

            //update the crop persistent data
            cropPersistentData.setWateredTile(locationKey);
        }
    }

    //TODO move to crop manager
    public void plantSeed(string seedName, Vector3Int targetTile)
    {
        byte playerLevel = sm.getPlayerLevel();
        
        //check that the soil is tilled
        if (currentTilledTilemap == null || !currentTilledTilemap.HasTile(targetTile)) return;

        Vector3Int cropHashKey = new Vector3Int(targetTile.x, targetTile.y, playerLevel);       
        
        //check if a crop already exists here
        if (cropMap.ContainsKey(cropHashKey)) return;
       

        Transform level = lm.getLevel(playerLevel);

        sbyte offsetXPixels = (seedOffsets[sm.getRandom(seedOffsets.Length)]);
        sbyte offsetYPixels = (seedOffsets[sm.getRandom(seedOffsets.Length)]);

        Vector3 gamePosition = new Vector3(targetTile.x + 0.5f + (offsetXPixels / 16f), targetTile.y + 0.5f + (offsetYPixels / 16f), 0);

        Vector3Int hashKey = new Vector3Int(targetTile.x, targetTile.y, playerLevel);


        Crop newCrop = instantiateCrop(gamePosition, playerLevel);

        cropMap.Add(hashKey, newCrop);
        Byte3 b3 = new Byte3(hashKey);
        cropPersistentData.addCrop(b3, newCrop.serializeCrop(b3, offsetXPixels, offsetYPixels));

    }

    public bool tryHarvest(Vector3Int targetTile)
    {
        byte playerLevel = sm.getPlayerLevel();
        
        Vector3Int cropHashKey = new Vector3Int(targetTile.x, targetTile.y, playerLevel);

        if (!cropMap.ContainsKey(cropHashKey)) return false;

        Crop toHarvest = cropMap[cropHashKey];

        if (toHarvest == null || !toHarvest.isHarvestable()) return false;


        //TODO layer shall be dynamic, crops map will be a vector 3 where z = level!!
        Transform parent = lm.getLevel(playerLevel).Find(playerLevel.ToString() + " Object");
        Vector3 position = new Vector3(targetTile.x, targetTile.y, 0);

        GameObject harvest = GameObject.Instantiate(toHarvest.getData().getPrefab(), position, Quaternion.identity, parent);
        lm.relayerMe(harvest.GetComponent<SpriteRenderer>(), parent.name);
        harvest.GetComponent<Pickup_Item>().pop();

        //check if this crop has multiple harvests
        if (!toHarvest.multiYield())
        {
            cropPersistentData.removeCrop(new Byte3(cropHashKey));
            cropMap.Remove(cropHashKey);
            Destroy(toHarvest.gameObject);
        }

        return true;

    }

    public void advanceDay()
    {
        foreach(Crop crop in cropMap.Values)
        {
            crop.updateAge();
        }
        //reflect the change in the SO in real time. TODO Maybe not neccessary in the future when age is automatic on day change?
        cropPersistentData.ageCrops();
    }

}


