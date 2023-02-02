using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Persistence", menuName = "Scene Persistence")]
public class Scene_Persistence : ScriptableObject
{
    Dictionary<Vector2Int, Crop> crops;

    public Dictionary<Vector2Int, Crop> getCrops() { return crops; }
}
