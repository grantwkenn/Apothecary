using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;


//TODO keep maps of all tilled, watered, etc. dirt tiles

public class Tile_Manager : MonoBehaviour
{
    public bool debugMode;
    
    Player player;
    //RoomManager rm;

    /// USED FOR DIRT ///////////////////////

    [SerializeField]
    private Tilemap dirtMap, wateredMap;
    

    Tilemap tillableTiles;


    TileBase tilledDirtTile, wateredDirtTile;

    ////////////////////////////////////

    //USED FOR GRASS OBJECTS

    [SerializeField]
    Tilemap grassTileMap;
       

    [SerializeField]
    GameObject grassPreFab;


    /// ///////////////////////////////////

    //USED FOR GRASS ANIMATION

    [SerializeField]
    Sprite defaultGrassSprite;

    [SerializeField]
    AnimationClip grassLeft;

    [SerializeField]
    AnimationClip grassRight;

    Dictionary<String, byte> grassDict;

    public bool instantiate_Grass;

    Dictionary<Vector2Int, bool> tilledTiles;
    Dictionary<Vector2Int, bool> wateredTiles;
    HashSet<Vector2Int> wheat;

    Dictionary<Vector2Int, Crop> crops;

    Scene_Persistence sp;

    GameObject selection_hilight;

    Vector3Int targetTile;
    Vector3Int cellTarget;

    bool hilightActive = false;

    [SerializeField]
    Color watered;

    //TODO
    //Dictionary of Vector2Int coords to crop object?
    //Crop object: daysOld, orientation/children, location? 

    ////////////////////////////////////////

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        sp = this.GetComponent<Scene_Manager>().getSP();

        //TODO these do not need to be loaded for indoor scenes without dirt (most scenes!)
        //maybe there should be a specific manager for farm related, Farm Manager? Crop Manager?

        //TODO replace these resources loads with SOs
        
        tilledDirtTile = Resources.Load<RuleTile>("Dirt Rule Tile");
        wateredDirtTile = Resources.Load<RuleTile>("Watered Dirt Rule Tile");

        Transform tillableT = GameObject.FindGameObjectWithTag("Grid").transform.Find("Tillable");
        if(tillableT != null)
            tillableTiles = tillableT.GetComponent<Tilemap>();

        populateGrassDict();

        targetTile = new Vector3Int();

        wateredTiles = new Dictionary<Vector2Int, bool>();
        

        selection_hilight = this.transform.Find("Tile Selection").gameObject;

        Transform grassMap = GameObject.Find("Grid").transform.Find("Grass");
        if (grassMap != null)
        {
            grassTileMap = grassMap.GetComponent<Tilemap>();
            if (instantiate_Grass)
            {
                instantiate_Grass = false;
                instantiateGrass();
            }

        }

        selection_hilight.SetActive(false);
    }

    public void waterTile()
    {
        if(dirtMap.HasTile(targetTile) && !wateredTiles.ContainsKey((Vector2Int)targetTile))
        {
            //dirtMap.SetTile(targetTile, wateredDirtTile);
            wateredMap.SetTile(targetTile, wateredDirtTile);
            //dirtMap.SetColor(targetTile, watered);
            wateredTiles.Add((Vector2Int)targetTile, true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.crops = new Dictionary<Vector2Int, Crop>();
        
        if(debugMode)
        {
            if(dirtMap != null)
            {
                dirtMap.ClearAllTiles();
            }
        }
        
        if(!debugMode || Time.time > 0.1f)
            loadScenePersistence();

    }


    void loadScenePersistence()
    {
        //get crops from Scene Persistence
        if (sp == null) return;

        if(sp.getCrops() != null)
        {
            foreach (KeyValuePair<Vector2Int, Crop> pair in sp.getCrops())
            {
                this.crops.Add(pair.Key, pair.Value);
            }
        }

        if (dirtMap == null) return;

        if(dirtMap != null)
        {
            //get all dug Tiles
            foreach (Vector2Int point in sp.getDugTiles())
            {
                //place a dirt tile onto the map here
                dirtMap.SetTile((Vector3Int)point, tilledDirtTile);

            }
        }    
        


    }



    // Update is called once per frame
    void Update()
    {
        //TODO Re impolement expose dirt
        //playerLocation = bgMap.WorldToCell(playerLocation);

        //check for grass tile

        targetTile.Set((int)player.transform.position.x, (int)player.transform.position.y, 0);

        int facing = player.getFacing();
        if(facing == 0)
        {
            double distance = player.transform.position.y - targetTile.y;
            
            if(distance > 0.25)
                targetTile.y += 1;
        }
        else if (facing == 1)
        {
            double distance = player.transform.position.x - targetTile.x;
            if (distance > 0.25)
                targetTile.x += 1;
        }
        else if (facing == 2)
        {
            double distance = player.transform.position.y - targetTile.y;

            if (distance < 0.75)
                targetTile.y -= 1;
        }
        else if (facing == 3)
        {
            double distance = player.transform.position.x - targetTile.x;

            if (distance < 0.75)
                targetTile.x -= 1;
        }


        //Check if there is a valid tile in the diggable tilemap
        if (hilightActive && tillableTiles != null && tillableTiles.HasTile(targetTile))
        {
            selection_hilight.transform.position = targetTile;
            selection_hilight.SetActive(true);
        }
        else
            selection_hilight.SetActive(false);

    }

    public void dig()
    {
        if (dirtMap == null || tillableTiles == null 
            || dirtMap.HasTile(targetTile) 
            || wateredTiles.ContainsKey((Vector2Int)targetTile)) return;
        
        //Check if there is a valid tile in the diggable tilemap
        if(tillableTiles.HasTile(targetTile))
        {
            dirtMap.SetTile(targetTile, tilledDirtTile);

            //save to the persistence scriptable object
            sp.setDugTile(new Vector2Int(targetTile.x, targetTile.y));
        }
            
    }


    void populateGrassDict()
    {
        grassDict = new Dictionary<string, byte>();
        
        grassDict.Add("Grass_Rule_Tile_0", 2);
        grassDict.Add("Grass_Rule_Tile_1", 3);
        grassDict.Add("Grass_Rule_Tile_2", 1);
        grassDict.Add("Grass_Rule_Tile_3", 1);
        grassDict.Add("Grass_Rule_Tile_4", 1);
        grassDict.Add("Grass_Rule_Tile_5", 2);
        grassDict.Add("Grass_Rule_Tile_6", 3);
        grassDict.Add("Grass_Rule_Tile_7", 1);
        grassDict.Add("Grass_Rule_Tile_8", 3);
        grassDict.Add("Grass_Rule_Tile_9", 3);
        grassDict.Add("Grass_Rule_Tile_10", 3);
        grassDict.Add("Grass_Rule_Tile_12", 1);
        grassDict.Add("Grass_Rule_Tile_13", 1);
        grassDict.Add("Grass_Rule_Tile_14", 1);
        grassDict.Add("Grass_Rule_Tile_15", 3);
        grassDict.Add("Grass_Rule_Tile_16", 1);
    }





    public void animateGrass(Animator anim, bool isLeft)
    {
        anim.enabled = true;
        if(isLeft)
        {
            anim.Play(grassLeft.name);
            StartCoroutine(grassAnimation(anim, grassLeft.length));
        }
        else
        {
            anim.Play(grassRight.name);
            StartCoroutine(grassAnimation(anim, grassRight.length));
        }
    }

    IEnumerator grassAnimation(Animator anim, float length)
    {
        yield return new WaitForSeconds(length);

        anim.enabled = false;
        anim.GetComponent<SpriteRenderer>().sprite = defaultGrassSprite;


    }





    void grassTest()
    {

        int height = 60;
        int length = 60;

        Transform parent = GameObject.Find("Grass Container").transform;

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 place = new Vector3(player.transform.position.x + (i), player.transform.position.y + (j), 0);
                Instantiate(grassPreFab, place, new Quaternion(0, 0, 0, 0), parent);

                place.x += 0.5f;
                place.y += 0.5f;
                Instantiate(grassPreFab, place, new Quaternion(0, 0, 0, 0), parent);
            }
        }
    }

    public void setSelectionHilight(bool val) { this.hilightActive = val; }

    void instantiateGrass()
    {
        Vector3Int index = new Vector3Int(0, 0, 0);
        Quaternion q = new Quaternion(0, 0, 0, 0);

        Transform parent = transform.Find("Grass Container");

        if (parent != null) return;

        GameObject container = new GameObject("Grass Container");
        container.transform.parent = this.transform.parent;

        parent = container.transform;


        foreach (Vector3Int position in grassTileMap.cellBounds.allPositionsWithin)
        {
            if (!grassTileMap.HasTile(position)) continue;


            Vector2 loc = grassTileMap.CellToWorld(position);
            loc.x += 0.5f;
            loc.y += 0.5f;

            Sprite sprite = grassTileMap.GetSprite(position);
            if (sprite == null) continue;
            string name = grassTileMap.GetSprite(position).name;
            if (!grassDict.ContainsKey(name)) continue;

            byte mode = grassDict[name];

            if (mode == 1)
            {
                Instantiate(grassPreFab, loc, q, parent);
            }

            else if (mode == 2)
            {
                loc.x += 0.5f;
                loc.y -= 0.5f;
                Instantiate(grassPreFab, loc, q, parent);
            }

            else if (mode == 3)
            {
                Instantiate(grassPreFab, loc, q, parent);
                loc.x += 0.5f;
                loc.y -= 0.5f;
                Instantiate(grassPreFab, loc, q, parent);
            }
        }



        grassTileMap.gameObject.SetActive(false);


    }

}
