using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup_Item : MonoBehaviour
{
    public Item item;
    private GameObject gameManager;


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponentInParent<SpriteRenderer>().sprite = item.sprite;

        gameManager = GameObject.FindGameObjectWithTag("GameManager");
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
                //Add to Inventory
                gameManager.GetComponent<Inventory_Manager>().addItem(this.item);


                //Destroy
                Destroy(this.gameObject);
            }

        }
    }
}
