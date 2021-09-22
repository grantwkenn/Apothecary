using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public int RoomNumber;

    public Vector2 localMin;
    public Vector2 localMax;

    private Vector2 worldMin;
    private Vector2 worldMax;

    //RoomSwitch[] switches;
    //List<RoomSwitch> switches;

    
    // Start is called before the first frame update
    void Start()
    {
        worldMin = new Vector2(localMin.x + this.transform.position.x, localMin.y + this.transform.position.y);
        worldMax = new Vector2(localMax.x + this.transform.position.x, localMax.y + this.transform.position.y);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 getMinPos() { return worldMin; }
    public Vector2 getMaxPos() { return worldMax; }
}
