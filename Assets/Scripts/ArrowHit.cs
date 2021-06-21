using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D arrow = this.gameObject.GetComponent<Rigidbody2D>();

        if (collision.CompareTag("breakable"))
        {
            collision.GetComponent<Pot>().Smash();
        }

        if (collision.CompareTag("wall"))
        {
            
            
            if (arrow.velocity.x != 0)
            {
                arrow.MovePosition(new Vector2(
                    collision.ClosestPoint(arrow.position).x, arrow.position.y));
            }
            else
            {
                arrow.MovePosition(new Vector2(
                    arrow.position.x, collision.ClosestPoint(arrow.position).y));
            }

            if (arrow.velocity.y > 0)
            {
                //move the sprite up a bit
                //arrow.position += new Vector2(0, 1);
                
            }
            

            this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.gameObject.GetComponent<Animator>().SetBool("stuck", true);
            
            //change sprite, and reposition a little depending on its direction.
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("breakable"))
        {
            this.gameObject.SetActive(false);
        }
    }

}
