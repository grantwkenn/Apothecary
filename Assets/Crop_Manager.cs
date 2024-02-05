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

    float[] seedOffsets = { 0f, 0f, 0.0625f, -0.0625f, 0.125f, -0.125f };

    Dictionary<Vector3Int, Crop> cropMap;


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

    void loadCropPersistenceData()
    {
        if (debug) return;

        cropPersistentData.init();

        //z values of the V3Int represents the level it belongs to. Ignore the "z" coordinate of 
        // the tiles by setting it to zero, relative to the rest of the tilemap (won't need this?)
        //set all the tilledTiles
        foreach(Vector3Int tileLoc in cropPersistentData.getTilledTiles())
        {
            tilledTilemaps[(byte)tileLoc.z].SetTile(new Vector3Int(tileLoc.x, tileLoc.y, 0), tilledDirtTile);
        }
        //set all watered tiles
        foreach(Vector3Int tileLoc in cropPersistentData.getWateredTiles())
        {
            wateredTilemaps[(byte)tileLoc.z].SetTile(new Vector3Int(tileLoc.x, tileLoc.y, 0), wateredDirtTile);
        }

        Dictionary<Vector3Int, SerializableCrop> sCrops = cropPersistentData.getSCrops();

        //TODO map new crop instantiated objects from serializable crops stored in cropPersistentData

        foreach (Vector3Int v3Int in sCrops.Keys)
        {
            Vector3 offset = sCrops[v3Int].getOffset();
            Vector3 position = new Vector3(v3Int.x + offset.x + 0.5f, v3Int.y + offset.y + 0.5f, v3Int.y + offset.y + 0.5f);

            Transform level = lm.getLevel((byte)v3Int.z);

            Crop newCrop = instantiateCrop(position, level);

            newCrop.setData(sCrops[v3Int]);

            cropMap.Add(v3Int, newCrop);

        }
    }

    Crop instantiateCrop(Vector3 position, Transform level)
    {
        //TODO pass the crop parameter and dynamically load the correct prefab crop (Another data mapping for Resource Man)
        
        //instantiate the crop here

        //TODO set the parent to be a crop container instead of the root of "Level 0" for ex
        GameObject newCrop = GameObject.Instantiate(rm.getPrefab("New Crop"), position, Quaternion.identity, level);


        //TODO make a level script with a function for finding the object child object without needing to find with text..?
        Transform layer = level.Find(level.name.Split(" ")[1] + " Object");
        lm.relayerMe(newCrop.GetComponent<SpriteRenderer>(), layer.name);

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
/*        BoundsInt bounds = currentTillableTilemap.cellBounds;
        
        foreach(Vector3Int position in bounds.allPositionsWithin)
        {
            if(currentTillableTilemap.HasTile(position))
            {
                TileBase tile = currentTillableTilemap.GetTile(position);
                Debug.Log(position);
                return false;
            }
        }*/
        
        
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
        cropPersistentData.setDugTile(new Vector3Int(targetTile.x, targetTile.y, sm.getPlayerLevel()));

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

            //update the crop persistent data
            cropPersistentData.setWateredTile(new Vector3Int(targetTile.x, targetTile.y, sm.getPlayerLevel()));
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

        float offsetX = (seedOffsets[sm.getRandom(seedOffsets.Length)]);
        float offsetY = (seedOffsets[sm.getRandom(seedOffsets.Length)]);

        Vector3 gamePosition = new Vector3(targetTile.x + 0.5f + offsetX, targetTile.y + 0.5f + offsetY, 0);

        Vector3Int hashKey = new Vector3Int(targetTile.x, targetTile.y, playerLevel);


        Crop newCrop = instantiateCrop(gamePosition, level);

        cropMap.Add(hashKey, newCrop);
        
        cropPersistentData.addCrop(hashKey, newCrop.serializeCrop((sbyte)(offsetX * 16), (sbyte)(offsetY * 16)));

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
            cropPersistentData.removeCrop(cropHashKey);
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


