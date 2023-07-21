using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Resources Object", menuName = "Resources Object")]
public class Resource_Object : ScriptableObject
{

    [SerializeField]
    List<Sprite> sprites;

    public Sprite getSprite(int index)
    {
        if (index < 0 || index >= sprites.Count) return null;
        return sprites[index];
    }


}
