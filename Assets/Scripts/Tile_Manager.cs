using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;


//TODO keep maps of all tilled, watered, etc. dirt tiles


[ExecuteInEditMode]
public class Tile_Manager : MonoBehaviour
{
    public bool debugMode;
    
    Player player;
    //RoomManager rm;

    /// USED FOR DIRT ///////////////////////

    [SerializeField]
    private Tilemap bgMap;

    [SerializeField]
    private Tilemap dirtMap;

    Tilemap tillableTiles;

    TileBase currentTile;

    [SerializeField]
    TileBase dirt;

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

    Dictionary<Vector2Int, Crop> crops;

    Scene_Persistence sp;

    public GameObject selection_hilight;

    //TODO
    //Dictionary of Vector2Int coords to crop object?
    //Crop object: daysOld, orientation/children, location? 

    ////////////////////////////////////////

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        sp = this.GetComponent<Scene_Manager>().getSP();

        Transform tillableT = GameObject.FindGameObjectWithTag("Grid").transform.Find("Tillable");
        if(tillableT != null)
            tillableTiles = tillableT.GetComponent<Tilemap>();

        populateGrassDict();

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
        //Find this tile
        //TODO need to do proper rounding of tile instead of int cast
        Vector3Int playerLoc = dirtMap.WorldToCell(player.transform.position);
        if(dirtMap.HasTile(playerLoc))
        {
            dirtMap.SetTile(playerLoc, null);
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

        //grid = rm.getCurrentGrid();

        //bgMap = grid.transform.Find("Background").GetComponent<Tilemap>();


        

        //grassTest();
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

        //get all dug Tiles
        foreach(Vector2Int point in sp.getDugTiles())
        {
            //place a dirt tile onto the map here
            dirtMap.SetTile((Vector3Int)point, dirt);

        }

    }


    void tileAction()
    {

    }

    void checkTransparentTile()
    {
        //get player location
        Vector3Int location;
    }


    void instantiateGrass()
    {
        Vector3Int index = new Vector3Int(0,0,0);
        Quaternion q = new Quaternion(0, 0, 0, 0);

        Transform parent = transform.Find("Grass Container");

        if (parent != null) return;

        GameObject container = new GameObject("Grass Container");
        container.transform.parent = this.transform.parent;

        parent = container.transform;


        foreach(Vector3Int position in grassTileMap.cellBounds.allPositionsWithin)
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





    // Update is called once per frame
    void Update()
    {
        //TODO Re impolement expose dirt
        //playerLocation = bgMap.WorldToCell(playerLocation);

        //check for grass tile

        Vector2 target = player.transform.position;
        Vector3Int targ = new Vector3Int(0, 0, 0);
        targ.x = (int)target.x;
        targ.y = (int)target.y;

        int facing = player.getFacing();
        if(facing == 0)
        {
            targ.y += 1;
        }
        else if (facing == 1)
        {
            targ.x += 1;
        }
        else if (facing == 2)
        {
            targ.y -= 1;
        }
        else if (facing == 3)
        {
            targ.x -= 1;
        }



        //targ = dirtMap.WorldToCell(targ);

        //Check if there is a valid tile in the diggable tilemap
        if (tillableTiles.HasTile(targ))
        {
            selection_hilight.transform.position = targ;
            selection_hilight.SetActive(true);
        }
        else
            selection_hilight.SetActive(false);

    }

    public void dig()
    {
        if (dirtMap == null || tillableTiles == null) return;
        
        
        Vector2 target = player.transform.position;
        
        int facing = player.getFacing();
        if (facing == 0) target.y += 1;
        else if (facing == 1) target.x += 1;
        else if (facing == 2) target.y -= 1;
        else if (facing == 3) target.x -= 1;


        Vector3Int targ = new Vector3Int(0, 0, 0);

        targ.x = (int)target.x;
        targ.y = (int)target.y;

        //TODO calculate closest instead of rounding
        if(target.x < 0) //round x up
        {
            targ.x -= 1;
        }
        if(target.y < 0)
        {
            targ.y -= 1;
        }
               
        targ = dirtMap.WorldToCell(targ);

        //Check if there is a valid tile in the diggable tilemap
        if(tillableTiles.HasTile(targ))
        {
            dirtMap.SetTile(targ, dirt);

            //save to the persistence scriptable object
            sp.setDugTile(new Vector2Int(targ.x, targ.y));
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


}
