using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Persistence", menuName = "Persistence")]
public class Scene_Persistence : ScriptableObject
{

    public int health;

    public Item[] inventory;
    public int inventorySize;


    private void OnEnable()
    {
        inventorySize = inventory.Length;
    }
}
