using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Crop", menuName = "Crop/New Crop")]
public class Crop_Data : ScriptableObject
{


    [SerializeField]
    string cropName;

    [SerializeField]
    byte cropID;

    
    //The age at which a crop starts this phase
    [SerializeField]
    byte sproutAge, seedlingAge, juvenileAge, floweringAge, fruitingAge;


    [SerializeField]
    Sprite seed, sprout, seedling, juvenile, flowering, fruiting;

    [SerializeField]
    bool multipleHarvests;

    public int getCropID() { return this.cropID; }

    public string getName() { return this.cropName; }

    public byte getPhase(byte age)
    {       
        if (age < sproutAge) return 0;
        if (age < seedlingAge) return 1;
        if (age < juvenileAge) return 2;
        if (age < floweringAge) return 3;
        if (age < fruitingAge) return 4;
        return 5;
    }

    public Sprite getSprite(byte phase)
    {
        if (phase == 0) return seed;
        if (phase == 1) return sprout;
        if (phase == 2) return seedling;
        if (phase == 3) return juvenile;
        if (phase == 4) return flowering;
        return fruiting;
    }

    public bool hasMultipleHarvests() { return multipleHarvests; }

    public byte getAgeOfPhase(byte phase)
    {
        if (phase == 1) return sproutAge;
        if (phase == 2) return seedlingAge;
        if (phase == 3) return juvenileAge;
        if (phase == 4) return floweringAge;
        if (phase == 5) return fruitingAge;
        return 0;
    }

}

//Each Crop has 6 phases: Seed, Sprout, Seedling, Juvenile, Flowered, Fruiting, (may go back to Fruiting depending on crop)

//Any special properties?
// orientation of children: vines can propagate to adjacent 3 tiles? This could be handled in the tile or crop manager since almost no tiles will implement this anyway..

//Do we need the reference to crop number? or is this handled in code? Name will match if we follow convention: Watermelon_Crop