using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Reference_Manager : MonoBehaviour
{
    [SerializeField]
    private Reference_Object refs;

    Dictionary<string, int> prefabMap;

    private void OnEnable()
    {
        //populate the prefabMap
        prefabMap = new Dictionary<string, int>();
        for (int i = 0; i < refs.prefabs.Length; i++)
        {
            if (!prefabMap.TryAdd(refs.prefabs[i].name, i)) Debug.Log("Mapping Error");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Data_Persistence getDataPersistence() { return refs.dataPersistence; }

    public Sprite getSprite(int index)
    {
        if (index < 0 || index >= refs.sprites.Count) return null;
        return refs.sprites[index];
    }

    public GameObject getPrefab(string name) 
    {
        return refs.prefabs[prefabMap[name]]; 
    }

    public Sprite[] getQuestSymbols()
    {
        return refs.questSymbols;
    }

    public List<TileBase> cropManagerInit()
    {
        List<TileBase> list = new List<TileBase>();
        list.Add(refs.tilledTileBase);
        list.Add(refs.wateredTileBase);
        return list;
    }


}
