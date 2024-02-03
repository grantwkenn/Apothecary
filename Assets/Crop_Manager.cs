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

    //TODO assign these sprites automatically using resource management?
    TileBase tilledDirtTile, wateredDirtTile;

    Tilemap currentTillableTilemap;
    Tilemap currentTilledTilemap;
    Tilemap currentWateredTilemap;

    [SerializeField] bool debug;


    private void OnEnable()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");

        //find all the tillable tilemaps in the scene
        lm = GameManager.GetComponent<Layer_Manager>();
        tm = GameManager.GetComponent<Tile_Manager>();
        sm = GameManager.GetComponent<Scene_Manager>();
        rm = this.GetComponent<Resource_Manager>();


        tillableTilemaps = new Dictionary<byte, Tilemap>();
        tilledTilemaps = new Dictionary<byte, Tilemap>();
        wateredTilemaps = new Dictionary<byte, Tilemap>();


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
                tilledTilemaps.Add(levelNo, tillable.GetComponent<Tilemap>());
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

        foreach(KeyValuePair<Vector4, Crop> pair in cropPersistentData.getCrops())
        {
            //instantiate the crop here
            Transform level = lm.getLevel((byte)pair.Key.z);
            //TODO set the parent to be a crop container instead of the root of "Level 0" for ex
            GameObject newCrop = GameObject.Instantiate(rm.getPrefab("New Crop"), (Vector3)pair.Key, Quaternion.identity, level);


            //TODO make a level script with a function for finding the object child object without needing to find with text..?
            Transform layer = level.Find(level.name.Split(" ")[1] + " Object");


            lm.relayerMe(newCrop.GetComponent<SpriteRenderer>(), layer.name);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        loadCropPersistenceData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updatePlayerLevel(byte level)
    {
        byte playerLevel = sm.getPlayerLevel();
        tillableTilemaps.TryGetValue(playerLevel, out currentTillableTilemap);
        tilledTilemaps.TryGetValue(playerLevel, out currentTilledTilemap);
        wateredTilemaps.TryGetValue(playerLevel, out currentWateredTilemap);

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
        || !currentTillableTilemap.HasTile(targetTile)
        || currentTilledTilemap.HasTile(targetTile)
        || currentWateredTilemap.HasTile(targetTile)) return;

        if (currentTilledTilemap.HasTile(targetTile) && !currentWateredTilemap.ContainsTile(wateredDirtTile))
        {
            currentWateredTilemap.SetTile(targetTile, wateredDirtTile);

            //update the crop persistent data
            cropPersistentData.setWateredTile(new Vector3Int(targetTile.x, targetTile.y, sm.getPlayerLevel()));
        }
    }

    //TODO move to crop manager
    public void plantSeed(string seedName, Vector3Int targetTile)
    {

        //check if a crop already exists here
        if (cropPersistentData.checkCrop(targetTile) == null) return; 


        //check that the soil is tilled
        if (currentTilledTilemap == null || !currentTilledTilemap.HasTile(targetTile)) return;

        Transform layer = lm.getLevel(sm.getPlayerLevel()).Find(sm.getPlayerLevel().ToString() + " Object");

        Vector3 randomOffset = (Vector3)cropPersistentData.getSeedOffsets(sm.getRandom(cropPersistentData.getSeedOffsetSize()));

        Vector3 position = new Vector3(targetTile.x + 0.5f, targetTile.y + 0.5f, 0);
        position += randomOffset;


        GameObject newCrop = GameObject.Instantiate(rm.getPrefab("New Crop"), position, Quaternion.identity, layer);
        lm.relayerMe(newCrop.GetComponent<SpriteRenderer>(), layer.name);

        cropPersistentData.addCrop(position, newCrop.GetComponent<Crop>());

    }

}
