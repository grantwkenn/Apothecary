using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Bar : MonoBehaviour
{
    RectTransform rt;
    Input_Manager inputManager;
    Inventory_Manager inventoryManager;
    GameObject gameManager;

    bool wakeStatus;

    int currentSelection = 0;

    Image[] images;
    Transform selector;

    Item[] items;

    Vector3[] slotPositions;


    
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponentInParent<RectTransform>();


        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        inputManager = gameManager.GetComponent<Input_Manager>();
        inventoryManager = gameManager.GetComponent<Inventory_Manager>();
        

        images = new Image[11];
        items = new Item[11];
        //items = inventoryManager.getItems();


        for (int i =0; i<11; i++)
        {           
            string s = "slot";
            s += i;
            Transform slot = this.transform.Find(s);
            images[i] = slot.GetComponent<Image>();
        }


        selector = this.transform.Find("Selection");


        slotPositions = new Vector3[11];
        for(int i=0; i<11; i++)
        {
            slotPositions[i] = new Vector3(3 + i * 18, 3, 0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        items = inventoryManager.getItems();

        //Bar wake and sleep /////////////////////////
        if (inputManager.getInventoryBarWakeStatus())
        {
            //if not yet at top
            if(rt.anchoredPosition.y < 4)
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
        
        selector.transform.localPosition = slotPositions[inputManager.getBarSelection()];

        Debug.Log(items.Length);
        
        
        for(int i = 0; i<11; i++)
        {
            images[i].sprite = items[i].sprite;
        }

    }
}
