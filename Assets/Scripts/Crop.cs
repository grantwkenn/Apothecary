using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    
    // float used for the effect of fertilizer
    float age;
    byte phase;
    Crop_Data data;
    bool harvestable = false;

    SpriteRenderer sr;

    float fertilizerCoefficient;

    private void OnEnable()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //set the sprite according to age
        //if less than

        //round down to the latest integer age
        phase = data.getPhase((byte) age);
        Sprite sprt = data.getSprite(phase);
        
        //Check Ready to Harvest
        if(phase == 5)
        {
            harvestable = true;
        }

    }

    public bool isHarvestable()
    {
        return harvestable;
    }

    //If it can fruit again, return true
    bool Harvest()
    {
        if (data.hasMultipleHarvests())
        {
            this.phase = 4;
            this.age = data.getAgeOfPhase(this.phase);
            sr.sprite = data.getSprite(this.phase);
            return true;
        }
        else return false;
        
    }

    public void updateAge()
    {
        this.age += 1 * fertilizerCoefficient;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
