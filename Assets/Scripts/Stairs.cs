using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public float slope;

    PolygonCollider2D pc;

    Player player;

    //Scene_Manager sm;
    Layer_Manager lm;

    bool slopeSet = false;

    public bool vertical;

    private void Start()
    {
        pc = this.GetComponent<PolygonCollider2D>();
        lm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Layer_Manager>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }


    private void OnTriggerEnter2D(Collider2D _player)
    {
        if (!_player.CompareTag("Player")) return;
        
        if(vertical)
        {
            //if moving up
            if(_player.GetComponent<Rigidbody2D>().velocity.y > 0)
            {
                lm.incrementPlayerLayer(player.gameObject, 1);
            }

            return;
        }
        
        //If LH Stair
        if(slope > 0)
        {
            //entering bottom of stair
            if (_player.attachedRigidbody.velocity.x < 0f) 
            {



            }

            else //entering top of stair
            {

                //decrement
                lm.incrementPlayerLayer(player.gameObject, -1);
            }

        }

        //RH Stair
        else
        {
            //entering bottom of stair
            if (_player.attachedRigidbody.velocity.x < 0f)
            {


            }

            else
            {

            }

        }
    }

    private void OnTriggerExit2D(Collider2D _player)
    {
        if (!_player.CompareTag("Player")) return;

        if(vertical)
        {
            //decrement layer if moving down
            if(_player.GetComponent<Rigidbody2D>().velocity.y < 0)
                lm.incrementPlayerLayer(player.gameObject, -1);
            return;

        }

        //If LH Stair
        if (slope > 0)
        {
            //leaving top of stair
            if (_player.attachedRigidbody.velocity.x < 0f)
            {
                //Increment Layer
                lm.incrementPlayerLayer(player.gameObject, 1);
            }

            else //leaving bottom of stair
            {


            }

        }

        //RH Stair
        else
        {
            //entering bottom of stair
            if (_player.attachedRigidbody.velocity.x < 0f)
            {


            }

        }
    }

    private void OnTriggerStay2D(Collider2D _player)
    {
        //check if player center point is in the box

        if (vertical) return;

        if (!_player.CompareTag("Player")) return;

        //would need to adjust for collider offset if sort point is not center of sprite!
        if (pc.OverlapPoint(_player.transform.position))
        {
            if(!slopeSet)
            {
                player.setStairSlope(slope);
                slopeSet = true;
            }

        }

        else
        {
            if(slopeSet)
            {
                player.setStairSlope(0f);
                slopeSet = false;
            }

        }
    }

}
