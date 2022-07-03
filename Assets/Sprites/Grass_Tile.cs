using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grass_Tile : Tile
{
    [SerializeField]
    Sprite[] sprites;
    
    [SerializeField]
    Tile tallGrass;

    [SerializeField]
    Tile tips;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Tile getTallGrass()
    {
        return tallGrass;
    }
}
