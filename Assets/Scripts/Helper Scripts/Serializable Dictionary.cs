using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys;

    [SerializeField]
    private List<TValue> values;

    public Dictionary<TKey, TValue> ToDictionary()
    {
        var dictionary = new Dictionary<TKey, TValue>();
        int count = Math.Min(keys.Count, values.Count);

        for (int i = 0; i < count; i++)
        {
            dictionary[keys[i]] = values[i];
        }

        return dictionary;
    }


}

[System.Serializable]
public class ItemDataJSON
{
    public string itemName, description;
    public int itemNo, value;

    public int stackLimit;

    public Sprite sprite;

}
