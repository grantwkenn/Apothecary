using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bat : MonoBehaviour
{
    Animator animator;

    GameObject target;

    bool chasing = false;

    Rigidbody2D rb;

    float flySpeed = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponentInParent<Animator>();
        rb = this.GetComponentInParent<Rigidbody2D>();

        animator.Play("BatHang");

        
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null && chasing)
        {
            animator.Play("BatFlap");
            chase();
        }

    }

    void chase()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, target.transform.position, flySpeed * Time.fixedDeltaTime));
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        target = other.gameObject;

        chasing = true;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        target = null;
        chasing = false;
    }

}
