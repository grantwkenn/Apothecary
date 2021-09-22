using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Bar : MonoBehaviour
{
    public bool autoHide = false;
    
    RectTransform rt;
    Input_Manager inputManager;
    Inventory_Manager inventoryManager;
    GameObject gameManager;

    Image[] images;
    Transform selector;

    Item[] items;


    Vector3[] slotPositions;

    Vector3 barPosition;

    
    // Start is called before the first frame update
    void Start()
    {
        //get object references //////////////////////////////////////////
        rt = GetComponentInParent<RectTransform>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        inputManager = gameManager.GetComponent<Input_Manager>();
        inventoryManager = gameManager.GetComponent<Inventory_Manager>();
        //////////////////////////////////////////////////////////////////

        
        images = new Image[11];

        //items = new Item[11];

   
        selector = this.transform.Find("Selection");

        //calculate slot positions on bar
        slotPositions = new Vector3[11];
        for(int i=0; i<11; i++)
        {
            slotPositions[i] = new Vector3(3 + i * 18, 3, 0);
        }

        barPosition = new Vector3(-103, 4, 0);

    }


    // Update is called once per frame
    void Update()
    {

        if (autoHide)
        {
            //Bar wake and sleep /////////////////////////
            if (inputManager.getInventoryBarWakeStatus())
            {
                //if not yet at top
                if (rt.anchoredPosition.y < 4)
                {
                    //move up 1
                    rt.Translate(new Vector3(0, 2, 0));
                }

            }
            else
            {
                if (rt.anchoredPosition.y > -25)
                    rt.Translate(new Vector3(0, -3, 0));
            }
            ///////////////////////////////////////////////
        }

        else
            rt.anchoredPosition = barPosition;




        //Bar Selector Scrolls with Input//////////////////////////////////////////////
        selector.transform.localPosition = slotPositions[inventoryManager.getSelectionNumber()];
        ////////////////////////////////////////////////////////////////////////////////
        

        for (int i = 0; i < 11; i++)
        {
            //Find the transform "Slot0 ..."
            string s = "Slot" + i;
            Transform slot = this.transform.Find(s);
            //Get Image Component from each Slot
            images[i] = slot.GetComponentInChildren<Image>();
        }


        items = inventoryManager.getItems();


        for (int i=0; i< items.Length; i++)
        {
            
            if (items[i].sprite != null)
            {
                images[i].enabled = true;
                images[i].sprite = items[i].sprite;
            }
            else
                images[i].enabled = false;
        }


    }
}
