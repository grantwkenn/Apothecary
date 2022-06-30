using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//TODO:
// update map according to the current room

public class Tile_Manager : MonoBehaviour
{
    [SerializeField]
    private Tilemap bgMap;
    [SerializeField]
    private Tilemap dirtMap;

    Player player;

    public TileBase currentTile;

    public TileBase dirt;

    [SerializeField]
    Tilemap[] grassTiles;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }


    // Update is called once per frame
    void Update()
    {
        Vector3Int playerLocation = new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z);

        Vector3Int roundedLocation = new Vector3Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y), 0);

        playerLocation = bgMap.WorldToCell(roundedLocation);


        currentTile = bgMap.GetTile(playerLocation);

        

    }

    public void exposeDirt()
    {
        Vector2 target = player.transform.position;
        
        int facing = player.getFacing();
        if (facing == 0) target.y += 1;
        else if (facing == 1) target.x += 1;
        else if (facing == 2) target.y -= 1;
        else if (facing == 3) target.x -= 1;


        Vector3Int targ = new Vector3Int(0, 0, 0);

        targ.x = (int)target.x;
        targ.y = (int)target.y;


        if(target.x < 0) //round x up
        {
            targ.x -= 1;
        }
        if(target.y < 0)
        {
            targ.y -= 1;
        }
        
        
        targ = dirtMap.WorldToCell(targ);

        dirtMap.SetTile(targ, dirt);

        Debug.Log("player:" + player.transform.position);

        Debug.Log("target:" + targ);

    }


    void renderTallGrass()
    {
        Tilemap grass0 = grassTiles[0];

        Vector3Int playerLocation = new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z);

        playerLocation = grass0.WorldToCell(playerLocation);

        if (!grass0.HasTile(playerLocation)) return;


        //Render the tall grass section if player is more than 6 tiles from the top..
        
        //get the tile we are standing on.
        TileBase grass0Tile = grass0.GetTile(playerLocation);

        //how many pixels above the top of this cell?
        double colliderBottom = player.transform.position.y - (player.GetComponent<BoxCollider2D>().size.y / 2f) - (player.GetComponent<BoxCollider2D>().offset.y/2f);

        double difference = colliderBottom % (int) colliderBottom;

        int pixelsAbove = (int)difference * 16;


        if(pixelsAbove < 10)
        {
            //render tall grass
            if(grassTiles[1].HasTile(playerLocation))
            {
                //
            }
        }



        TileBase grass1Tile = grassTiles[1].GetTile(playerLocation);
        TileBase grass2Tile = grassTiles[2].GetTile(playerLocation);
        
        
        if (currentTile.name.Substring(0, 12) != "Grass_Tiles2")
            return;


        
        
        Vector2 position = player.transform.position;




    }


}
