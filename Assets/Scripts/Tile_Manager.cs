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

        //bgMap.SetTile(playerLocation, dirt);

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

}
