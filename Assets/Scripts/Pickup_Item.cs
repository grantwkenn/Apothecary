using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
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
        this.GetComponentInParent<SpriteRenderer>().sprite = item_Data.getSprite();

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
