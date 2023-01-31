using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Crop", menuName = "Crop/New Crop")]
public class Crop_Data : ScriptableObject
{


    [SerializeField]
    string cropName, description;

    [SerializeField]
    byte cropID;

    [SerializeField]
    byte seedDays, sproutDays, seedlingDays, juvenileDays, floweredDays, fruitingDays;


    [SerializeField]
    Sprite seed, sprout, seedling, juvenile, flowered, fruiting;



    public int getCropID() { return this.cropID; }

    public string getName() { return this.cropName; }


}

//Each Crop has 6 phases: Seed, Sprout, Seedling, Juvenile, Flowered, Fruiting, (may go back to Fruiting depending on crop)

//Any special properties?
// orientation of children: vines can propagate to adjacent 3 tiles? This could be handled in the tile or crop manager since almost no tiles will implement this anyway..

//Do we need the reference to crop number? or is this handled in code? Name will match if we follow convention: Watermelon_Crop