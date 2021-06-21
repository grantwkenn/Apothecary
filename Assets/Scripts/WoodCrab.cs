using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodCrab : Enemy
{
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public float contactRadius;
    public float wakeRadius;
    public Transform homePosition;

    protected Animator animator;

    private float distanceFromTarget;

    private Rigidbody2D rigidBody;



    public enum animState
    {
        crab_hide, crab_emerge, crab_charge, crab_idle, Wander
    }

    public animState currentState = animState.crab_hide;


    void changeAnimationState(animState newState)
    {
        if (newState == currentState) return;
        
        currentState = newState;

        animator.Play(currentState.ToString());

        Debug.Log(currentState.ToString());

    }

    
    // Start is called before the first frame update
    void Start()
    {
        home = transform.position;
        
        rigidBody = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
        CheckContact();

        //FIXED UPDATE

        //if in charging animation state
        if (currentState == animState.crab_charge && !knockBack)
        {
            charge();
        }
            
    }

    void CheckDistance()
    {
        distanceFromTarget = Vector3.Distance(target.position, transform.position);


        //wakeup
        //TODO: set to dynamic from here
        if (currentState == animState.crab_hide && distanceFromTarget <= wakeRadius)
        {
            changeAnimationState(animState.crab_emerge);
            return;
        }
      
        //can chase
        if(distanceFromTarget <= chaseRadius && distanceFromTarget > contactRadius)
        {
            if(currentState == animState.crab_idle)
                changeAnimationState(animState.crab_charge);
        }

        //cannot chase
        else
        {
            if (distanceFromTarget > chaseRadius)
            {
                if (currentState == animState.crab_charge)
                {
                    changeAnimationState(animState.crab_idle);
                    rigidBody.bodyType = RigidbodyType2D.Kinematic;
                    
                }
                    

            }
        }
    }
    

    void CheckContact()
    {
        if (distanceFromTarget <= contactRadius)
        {
            //set to kinematic
            //changeAnimationState(animState.crab_idle);
            rigidBody.bodyType = RigidbodyType2D.Kinematic;

            GameObject.FindWithTag("Player").GetComponent<Player>().KnockBack(rigidBody.position, 10);

        }
        else if (rigidBody.bodyType == RigidbodyType2D.Kinematic)
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
    }

    void charge()
    {        
        //transform position if far enough
        if(distanceFromTarget > contactRadius)
        {
            Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.fixedDeltaTime);

            rigidBody.MovePosition(temp);      
        }

    }



    public override void KnockBack(Vector3 pos, float thrust)
    {
        if (currentState != animState.crab_charge) return;

        rigidBody.bodyType = RigidbodyType2D.Dynamic;

        Vector2 difference = transform.position - pos;
        difference = difference.normalized * thrust;

        rigidBody.AddForce(difference, ForceMode2D.Impulse);
        knockBack = true;
        StartCoroutine(KnockCo());
    }

    private IEnumerator KnockCo()
    {
        yield return new WaitForSeconds(.25f);
        rigidBody.velocity = Vector2.zero;
        knockBack = false;
    }

    private void OnDisable()
    {
        //TODO replenish health
        
        transform.position = home;
        rigidBody.velocity = Vector2.zero;
        currentState = animState.crab_hide;
    }

}
