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

    BoxCollider2D bc;

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

    int chanceEatAgain = 50;
    int waitRangeMin = 3;
    int waitRangeMax = 5;
    int chanceToMove = 50;

    Vector2 prevTarget;

    Bounds horzColliderBounds;
    Vector2 vertColliderBounds;


    private void OnEnable()
    {
        this.animator = this.GetComponent<Animator>();
        this.rb = this.GetComponent<Rigidbody2D>();
        this.bc = this.GetComponent<BoxCollider2D>();

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

        if(bc.size.y < bc.size.x) // horizontal
        {
            //Vector3 center = this.rb
            //horzColliderBounds = new Bounds()
            //vertBox = new Vector2(bc.size.y, bc.size.x);
        }
        else
        {
            //vertBox = bc.size;
            //horzBox = new Vector2(bc.size.y, bc.size.x);
        }
        
        
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


        else if(state == CritterState.idle)
        {
            //Decide next behavior

            float rand = Random.Range(0, 100);

            if(prevState == CritterState.eating)
            {
                if(rand < chanceEatAgain)
                {
                    eat();
                    return;
                }
            }

            if(prevState == CritterState.walking)
            {
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

                animator.Play(walkAnimations[directionIndex].name);
                //StartCoroutine(playAnimation(walkAnimations[directionIndex]));

                return;
            }

            else
            {
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
        
        
        else if(state == CritterState.idle || state == CritterState.wait)
        {
            animator.Play(idleAnimations[directionIndex].name);

        }


    }

    public void move()
    {

        rb.MovePosition(Vector2.MoveTowards(rb.position, target, walkSpeed * Time.fixedDeltaTime));
        directionUpdate();


        if (Vector2.Distance(rb.position, target) < (walkSpeed * Time.fixedDeltaTime * 1.5f))
        {
            rb.MovePosition(target);
            prevState = state;
            state = CritterState.idle;
            animate();
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
        //animator.Play(eatAnimations[directionIndex].name);



        StartCoroutine(playAnimation(eatAnimations[directionIndex]));

    }

    IEnumerator wait()
    {
        prevState = state;
        this.state = CritterState.wait;
        
        float waitTime = Random.Range(waitRangeMin, waitRangeMax);
        yield return new WaitForSeconds(waitTime);
        this.prevState = state;
        this.state = CritterState.idle;
    }

    IEnumerator playAnimation(AnimationClip clip)
    {
        animator.Play(clip.name);

        yield return new WaitForSeconds(clip.length);

        prevState = state;
        state = CritterState.idle;

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

        //bc.size = (directionIndex == 0 || directionIndex == 2) ? bc.size = vertBox : bc.size = horzBox;


        //If moving in a new direction
        if(prevDirection != directionIndex)
        {
            
            animator.Play(walkAnimations[directionIndex].name);
        }
    }


}
