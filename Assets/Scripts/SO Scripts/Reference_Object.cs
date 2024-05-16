using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//SCRIPT EXECUTION ORDER: it is best to go early since it is only responsible for holding references and
// can be used by any scripts

[CreateAssetMenu(fileName = "Reference Object", menuName = "Reference Object")]
public class Reference_Object : ScriptableObject
{
    //TODO setup a Dictionary<type, Dictionary<string, object>>
    //so I can call one function to search all resources by just the type, and the name of the asset

    [SerializeField]
    public List<Sprite> sprites;

    [SerializeField]
    public Sprite[] questSymbols;

    [SerializeField]
    public GameObject[] prefabs;

    [SerializeField]
    public Data_Persistence dataPersistence;

    [SerializeField]
    public TileBase tilledTileBase, wateredTileBase;

}
