using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Persistence", menuName = "Scene Persistence")]
public class Scene_Persistence : ScriptableObject
{
    Dictionary<Vector2Int, Crop> crops;


    HashSet<Vector2Int> dugTiles;

    public Vector2Int[] tiles;

    public int iny;

    public void initialize()
    {
        //populate the HashSet from the stored List
        dugTiles = new HashSet<Vector2Int>();
        if(tiles != null)
        {
            foreach(Vector2Int point in tiles)
            {
                dugTiles.Add(point);
            }
        }
    }


    public Dictionary<Vector2Int, Crop> getCrops() 
    { 
        if(crops != null)
            return crops;
        return null;
    }

    public void setDugTile(Vector2Int point)
    {
        dugTiles.Add(point);
    }

    public HashSet<Vector2Int> getDugTiles()
    {
            
        return this.dugTiles;
    }



    public void exitScene()
    {
        tiles = new Vector2Int[dugTiles.Count];
        int i = 0;
        foreach(Vector2Int point in dugTiles)
        {
            tiles[i] = point;
            i++;
        }
    }
}
