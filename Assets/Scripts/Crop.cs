using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crop : MonoBehaviour
{
    
    // float used for the effect of fertilizer
    [SerializeField]
    float age;
    [SerializeField]
    byte phase;
    [SerializeField]
    Crop_Data data;
    bool harvestable = false;


    SpriteRenderer sr;


    public bool run;


    private void OnEnable()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

    void refresh()
    {
        updatePhase();
    }

    // Start is called before the first frame update
    void Start()
    {
        //set the sprite according to age
        //if less than

        updatePhase();


    }

    void updatePhase()
    {
        //round down to the latest integer age
        phase = data.getPhase((byte)age);
        sr.sprite = data.getSprite(phase);

        //Check Ready to Harvest
        if (phase == 5)
        {
            harvestable = true;
        }
    }

    public bool isHarvestable()
    {
        return harvestable;
    }

    //If it can fruit again, return true
    public bool multiYield()
    {
        if (!this.data.hasMultiYield()) return false;

        harvestable = false;
        this.phase = 2;
        this.age = data.getAgeOfPhase(this.phase);
        sr.sprite = data.getSprite(this.phase);

        return true;
        
    }

    public void updateAge()
    {
        this.age += 1;
        updatePhase();
    }

    // Update is called once per frame
    void Update()
    {
        if(run)
        {
            run = false;
            refresh();
        }
    }

    public Crop_Data getData() { return this.data; }

}
