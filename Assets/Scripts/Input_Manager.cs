using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Manager : MonoBehaviour
{

    //get reference to the quick bar
    bool inventoryBarWake = false;
    int inventoryTimer;

    int barSelection = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        inventoryTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        
        //get scroll input
        if (scroll != 0)
        {
            //wake the inventory bar
            inventoryBarWake = true;
            inventoryTimer = 0;

            if (scroll > 0)
                barSelection += 1;
            else if (scroll < 0)
                barSelection -= 1;

            if (barSelection > 10) barSelection = 0;
            if (barSelection < 0) barSelection = 10;

        }



    }

    private void FixedUpdate()
    {
        if(inventoryBarWake == true)
        {
            inventoryTimer++;
            if(inventoryTimer > 150) // 5 seconds at 30Hz
            {
                inventoryBarWake = false;
                inventoryTimer = 0;
            }
        }
        
    }

    public bool getInventoryBarWakeStatus()
    {
        return inventoryBarWake;
        
    }

    public int getBarSelection() { return barSelection; }
}
