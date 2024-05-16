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

    Vector2 home;

    public float chaseRadius = 8f;
    float alertRadius;

    CircleCollider2D chaseCollider;

    private void OnEnable()
    {
        CircleCollider2D[] temp = this.GetComponentsInChildren<CircleCollider2D>();
        foreach(CircleCollider2D cc in temp)
        {
            if (cc.isTrigger) chaseCollider = cc;
            break;
        }
        if (chaseCollider != null)
            alertRadius = chaseCollider.radius;
    }

    // Start is called before the first frame update
    void Start()
    {

        animator = this.GetComponentInParent<Animator>();
        rb = this.GetComponentInParent<Rigidbody2D>();

        animator.Play("BatHang");

        home = this.rb.position;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (target != null && chasing)
        {
            animator.Play("BatFlap");
            chase();
        }

        else retreat();

    }

    void chase()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, target.transform.position, flySpeed * Time.fixedDeltaTime));
    }

    void retreat()
    {
        if (Vector2.Distance(home, rb.position) < (4f/16f))
        {
            rb.position = home;
            animator.Play("BatHang");
        }
        else
            rb.MovePosition(Vector2.MoveTowards(rb.position, home, flySpeed * (3f/2f) * Time.fixedDeltaTime));

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        target = other.gameObject;

        chasing = true;
        chaseCollider.radius = chaseRadius;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        target = null;
        chasing = false;
        chaseCollider.radius = alertRadius;
    }

}
