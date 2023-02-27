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

    Item item;

    private void OnEnable()
    {
        this.GetComponentInParent<SpriteRenderer>().sprite = item_Data.getSprite();

        item = new Item(item_Data, quantity);

    }



    //check collisoin with Player
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<Inventory_Manager>().offerItem(item))
                Destroy(this.gameObject);
            

        }
    }
}
