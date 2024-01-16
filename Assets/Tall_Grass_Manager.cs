using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tall_Grass_Manager : MonoBehaviour, ICustomLayer
{
    HashSet<Vector2Int> wheatMap;

    public bool doReload;


    private void Awake()
    {
        wheatMap = new HashSet<Vector2Int>();
    }

    public void layer(string sortingLayerName)
    {
        reload();
    }


    // Start is called before the first frame update
    void Start()
    {
        reload();
    }


    // Update is called once per frame
    void Update()
    {
        if(doReload)
        {
            doReload = false;
            reload();
        }
    }
    
    void reload()
    {
        string sortingLayer = this.transform.parent.name;

        SpriteRenderer[] sprites = this.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sr in sprites)
        {
            sr.sortingLayerName = sortingLayer;
        }
        
        wheatMap = new HashSet<Vector2Int>();
               
        Wheat[] allWheat = this.GetComponentsInChildren<Wheat>();

        foreach(Wheat wheat in allWheat)
        {
            wheatMap.Add(wheat.getGridPosition());
        }

        foreach(Wheat wheat in allWheat)
        {
            wheat.reload();
        }
    }

    public bool checkWheat(Vector2Int v2)
    {
        return wheatMap.Contains(v2);
    }

    public void unmap(Vector2Int wheat)
    {
        if(wheatMap.Contains(wheat))
            wheatMap.Remove(wheat);
    }

}
