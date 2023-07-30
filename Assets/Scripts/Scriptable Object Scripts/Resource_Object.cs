using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Resources Object", menuName = "Resources Object")]
public class Resource_Object : ScriptableObject
{

    [SerializeField]
    List<Sprite> sprites;

    [SerializeField]
    Sprite[] questSymbols;

    [SerializeField]
    GameObject[] prefabs;

    Dictionary<string, int> prefabMap;

    public Sprite getSprite(int index)
    {
        if (index < 0 || index >= sprites.Count) return null;
        return sprites[index];
    }

    public Sprite getResource(string name, int index)
    {
        

        return null;
    }

    public GameObject getPrefab(string name) { return prefabs[prefabMap[name]]; }

    public void OnValidate()
    {
        //re map all prefabs by name when change is made in Editor
        if (prefabMap == null) prefabMap = new Dictionary<string, int>();
        prefabMap.Clear();
        for(int i=0; i<prefabs.Length; i++)
        {
            if (!prefabMap.TryAdd(prefabs[i].name, i)) Debug.Log("Mapping Error");
        }

    }

    public Sprite[] getQuestSymbols()
    {
        return this.questSymbols;
    }


}
