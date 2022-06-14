using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Item : MonoBehaviour
{
    [SerializeField]
    Item_Data item_Data;

    [SerializeField]
    int quantity;


    private GameObject gameManager;

    private void OnEnable()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
    }


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponentInParent<SpriteRenderer>().sprite = item_Data.getSprite();

        

    }

    // Update is called once per frame
    void Update()
    {

    }

    //check collisoin with Player
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            //if inventory not full
            if(!gameManager.GetComponent<Inventory_Manager>().inventoryFull())
            {
                Item item = new Item(item_Data, quantity);
                
                //Add to Inventory
                gameManager.GetComponent<Inventory_Manager>().offerItem(item);


                //Destroy
                Destroy(this.gameObject);
            }

        }
    }
}
