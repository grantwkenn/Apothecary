using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
        //TODO get rid of this
        if(run)
        {
            run = false;
            refresh();
        }
    }

    public Crop_Data getData() { return this.data; }

    public SerializableCrop serializeCrop(Byte3 locationKey, sbyte offsetX, sbyte offsetY)
    {
        return new SerializableCrop(this.data.getName(), locationKey, this.age, offsetX, offsetY);
    }

    public void setData(SerializableCrop sc, Crop_Data cd)
    {
        this.age = sc.getAge();
        this.data = cd;
    }

    public float getAge()
    {
        return this.age;
    }


}

[System.Serializable]
public class SerializableCrop
{
    [SerializeField]
    string name;
    [SerializeField]
    float age;
    [SerializeField]
    sbyte offsetX;
    [SerializeField]
    sbyte offsetY;
    Byte3 location;

    public SerializableCrop(string name, Byte3 locationKey, float age, sbyte offsetX, sbyte offsetY)
    {
        this.age = age; this.offsetX = offsetX; this.offsetY = offsetY;
        this.location = locationKey;
        this.name = name;
    }

    public void updateAge(float amount)
    {
        this.age+= amount;
    }

    public void setAge(float age)
    {
        this.age = age;
    }

    public float getAge() { return this.age; }
    public string getName() { return this.name; }
    public Vector3 getOffset() { return new Vector3(offsetX / 16f, offsetY / 16f, 0); }
    public Byte3 getLocationKey() { return this.location; }
    

}
