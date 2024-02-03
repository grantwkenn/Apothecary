using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "crop_pers_data", menuName = "Scene/Crop Persistence Data")]
public class Crop_Persistent_Data : ScriptableObject
{
    //TODO use this container to store the sprites for: till, watered
    [SerializeField]
    private TileBase tilledSoilTile, wateredSoilTile;

    [SerializeField] static Vector2[] seedOffsets;

    //this SO stores the location of tilled tiles, watered tiles, and crops at each level
    //the z value of each V3Int stores the level number

    Dictionary<Vector4, Crop> crops;

    HashSet<Vector3Int> tilledTiles;
    HashSet<Vector3Int> wateredTiles;

    public TileBase getTilledSoilTile() { return this.tilledSoilTile; }
    public TileBase getWateredSoilTile() { return this.wateredSoilTile; }

    public void setDugTile(Vector3Int point)
    {
        tilledTiles.Add(point);
    }

    public void setWateredTile(Vector3Int point)
    {
        wateredTiles.Add(point);
    }

    public List<Vector3Int> getTilledTiles() { return new List<Vector3Int>(this.tilledTiles != null ? this.tilledTiles : new List<Vector3Int>()); }
    public List<Vector3Int> getWateredTiles() { return new List<Vector3Int>(this.wateredTiles != null ? this.wateredTiles : new List<Vector3Int>()); }

    public Crop checkCrop(Vector3 key) { if (crops.ContainsKey(key)) return crops[key]; return null; }

    public Vector2 getSeedOffsets(int index) { return seedOffsets[index]; }
    public int getSeedOffsetSize() { return seedOffsets.Length; }

    public void addCrop(Vector3 location, Crop crop)
    {
        crops.Add(location, crop);
    }

    public List<KeyValuePair<Vector4, Crop>> getCrops()
    {
        List<KeyValuePair<Vector4, Crop>> result = new List<KeyValuePair<Vector4, Crop>>();

        foreach(Vector4 key in crops.Keys)
        {
            result.Add(new KeyValuePair<Vector4, Crop>(key, crops[key]));
        }
        return result;
    }
     
}
