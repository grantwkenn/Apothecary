using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    up, down, left, right
}

public enum CritterState
{
    wait, idle, eating, walking
}


public class Being : MonoBehaviour
{

    Animator animator;

    Rigidbody2D rb;

    ushort directionIndex;

    Vector2 target;

    [SerializeField]
    float walkSpeed;

    [SerializeField]
    Transform[] points;

    Vector2[] targets;

    [SerializeField]
    AnimationClip[] walkAnimations;

    [SerializeField]
    AnimationClip[] eatAnimations;

    [SerializeField]
    AnimationClip[] idleAnimations;


    //bool onTarget = false;

    [SerializeField]
    CritterState state;
    [SerializeField]
    CritterState prevState;

    int chanceEatAgain = 30;
    int waitRangeMin = 3;
    int waitRangeMax = 5;
    int chanceToMove = 60;

    Vector2 prevTarget;


    private void OnEnable()
    {
        this.animator = this.GetComponent<Animator>();
        this.rb = this.GetComponent<Rigidbody2D>();

        this.directionIndex = 2;

        this.state = CritterState.idle;
        this.prevState = CritterState.idle;

    }


    // Start is called before the first frame update
    void Start()
    {
        targets = new Vector2[points.Length];

        for(int i = 0; i<points.Length; i++)
        {
            targets[i] = points[i].position;
        }

        prevTarget = targets[0];
        
        
        animate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        doBehavior();
    }

    void doBehavior()
    {
        if (state == CritterState.walking) move();


        if(state == CritterState.idle)
        {
            //Decide next behavior

            float rand = Random.Range(0, 100);

            if(prevState == CritterState.eating)
            {
                if(rand < chanceEatAgain)
                {
                    prevState = state;
                    state = CritterState.eating;
                    eat();
                    return;
                }
            }

            if(prevState == CritterState.walking)
            {
                prevState = state;
                state = CritterState.eating;
                eat();
                return;
            }

            
            if(rand < chanceToMove)
            {
                
                
                prevState = state;
                this.state = CritterState.walking;

                int targetIndex = Random.Range(0, targets.Length);

                prevTarget = target;
                this.target = (Vector2)targets[targetIndex];

                if (target == (Vector2)this.transform.position || target == prevTarget)
                {
                    prevTarget = target;
                    targetIndex = (targetIndex + 1) % targets.Length;
                    target = (Vector2)targets[targetIndex];
                }
                return;
            }

            else
            {
                prevState = state;
                this.state = CritterState.wait;
                Debug.Log("wait started");
                StartCoroutine(wait());
            }







        }

        //Maybe Find new place


    }

    void animate()
    {
        if(state == CritterState.walking)
        {
            animator.Play(walkAnimations[directionIndex].name);
        }
        
        
        if(state == CritterState.idle || state == CritterState.wait)
        {
           

        }


    }

    public void move()
    {
        if (target == (Vector2) this.transform.position) return;
        
        

        rb.MovePosition(Vector2.MoveTowards(rb.position, target, walkSpeed * Time.fixedDeltaTime));
        directionUpdate();


        if (Vector2.Distance(rb.position, target) < (walkSpeed * Time.fixedDeltaTime * 1.5f))
        {
            rb.MovePosition(target);
            prevState = state;
            state = CritterState.idle;
        }
    }

    public void finishedEating()
    {
        prevState = state;
        state = CritterState.idle;
        animator.Play(idleAnimations[directionIndex].name);
    }

    void eat()
    {
        prevState = state;
        state = CritterState.eating;
        animator.Play(eatAnimations[directionIndex].name);
    }

    IEnumerator wait()
    {
        float waitTime = Random.Range(waitRangeMin, waitRangeMax);
        yield return new WaitForSeconds(waitTime);
        Debug.Log("wait finished");
        this.prevState = state;
        this.state = CritterState.idle;
    }

    void directionUpdate()
    {
        ushort prevDirection = directionIndex;
        
        Vector2 direction = target - (Vector2)this.transform.position;


        float degrees = Vector2.SignedAngle(Vector2.down, direction);

        if (degrees <= -135 || degrees >= 135) directionIndex = 0;

        else if (degrees <= -45) directionIndex = 3;

        else if (degrees <= 45) directionIndex = 2;

        else if (degrees <= 135) directionIndex = 1;

        else directionIndex = 0;


        if(prevDirection != directionIndex)
        {
            animator.Play(walkAnimations[directionIndex].name);
        }
    }


}
