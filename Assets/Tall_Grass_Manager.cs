using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tall_Grass_Manager : MonoBehaviour
{
    HashSet<Vector2Int> wheatMap;

    public bool doReload;

    private void Awake()
    {
        wheatMap = new HashSet<Vector2Int>();
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
        if (wheatMap == null) wheatMap = new HashSet<Vector2Int>();
        
        wheatMap.Clear();
        
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

}
