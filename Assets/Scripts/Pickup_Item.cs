using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pickup_Item : MonoBehaviour
{
    [SerializeField]
    Item_Data item_Data;
    Layer_Manager layMan;

    [SerializeField]
    int quantity;

    Item item;

    bool popped;
    
    static Vector2 startingVelocity = new Vector2(30, 250);
    static Vector2 gravity = new Vector2(0, -25);

    float localFloor;
    bool bounced;
    Vector2 velocity;

    public bool popIt;

    BoxCollider2D bc;
    

    private void OnEnable()
    {
        bc = this.GetComponent<BoxCollider2D>();
        this.GetComponentInParent<SpriteRenderer>().sprite = item_Data.getSprite();

        item = new Item(item_Data, quantity);
    }


    private void FixedUpdate()
    {
        if(popIt)
        {
            popIt = false;
            pop();
        }    
        
        if(popped)
        {
            Vector3 newPosition = this.transform.localPosition + ((Vector3)velocity * Time.fixedDeltaTime);
            this.transform.SetLocalPositionAndRotation(newPosition, Quaternion.identity);

            if (velocity.y < 0 && transform.localPosition.y < localFloor)
            {
                if(bounced)
                {
                    bounced = false;
                    //done popping
                    popped = false;
                    velocity = Vector2.zero;
                    this.bc.enabled = true;
                }
                
                //reverse y velocity for a bounce
                newPosition = new Vector2(this.transform.localPosition.x, localFloor);
                this.transform.SetLocalPositionAndRotation(newPosition, Quaternion.identity);

                velocity = new Vector2(velocity.x, (0-velocity.y)*0.65f );
                bounced = true;
            }

            else
            {
                velocity = velocity + (gravity * Time.fixedDeltaTime);
            }
        }
    }

    public void pop()
    {
        this.bc.enabled = false;
        
        popped = true;
        bounced = false;
        System.Random r = new System.Random();
        velocity = startingVelocity * Time.fixedDeltaTime;
        localFloor = this.transform.localPosition.y;

        if (r.Next(0, 10) < 5) velocity = new Vector2(-velocity.x, velocity.y);
        
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
