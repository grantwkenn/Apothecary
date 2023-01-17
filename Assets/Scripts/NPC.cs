using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum NPCState
{
    idle, walk, run, engage
}


public class NPC : MonoBehaviour
{
    [SerializeField]
    private int NPC_ID;
    
    GameObject player;
    Dialogue_Manager DM;
    Messager messager;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    public float walkSpeed;

    public Vector2 currentTarget;

    public NPCState currentState;
    NPCState startingState;

    protected Animator animator;


    int animationDirection;
    Vector2 direction;

    public string[] idle;
    public string[] walk;


    Vector2[] path;
    int pathSize;
    int pathIndex;


    Vector2 prevDirection;

    Vector2 home;

    public bool dontMove;

    public Transform[] targets;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        DM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Dialogue_Manager>();
        messager = this.GetComponentInParent<Messager>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        rb = this.GetComponent<Rigidbody2D>();


        //Manually Round location to integer X and Y (5.44, 10.7) --> (5, 11)
        Vector2 floorXY = new Vector2(Mathf.RoundToInt(rb.position.x), Mathf.RoundToInt(rb.position.y));
        rb.position = floorXY;

        direction = Vector2.down;

        animator = GetComponent<Animator>();

        startingState = NPCState.walk;
        currentState = startingState;

        pathSize = 4;
        path = new Vector2[pathSize];
        path[0] = new Vector2(rb.transform.position.x, rb.transform.position.y - 4);
        path[1] = new Vector2(rb.transform.position.x-16, rb.transform.position.y - 4);
        path[2] = new Vector2(rb.transform.position.x-16, rb.transform.position.y);
        path[3] = new Vector2(rb.transform.position.x, rb.transform.position.y);

        path[0] = targets[0].position;
        path[1] = targets[1].position;
        path[2] = targets[2].position;
        path[3] = targets[3].position;

        pathIndex = 2;

        currentTarget = path[pathIndex];



        directionFromTarget();

        home = rb.position;

    }

    // Update is called once per frame
    void Update()
    {
        //simpleAI();

        if(!dontMove)
        AnimationUpdate();
    }

    private void FixedUpdate()
    {
        //simpleAI();
        if (currentState == NPCState.engage)
        {
            direction = cardinal(((Vector2)player.transform.position - rb.position).normalized);

            animationDirection = cardinalToInt(direction);
        }
        else if (!dontMove)
            simpleAI();
    }


    void AnimationUpdate()
    {

        if (currentState == NPCState.idle || currentState == NPCState.engage)
        {
            animator.Play(idle[animationDirection]);
        }
        else if (currentState == NPCState.walk)
        {
            animator.Play(walk[animationDirection]);
        }

    }




    void simpleAI()
    {
        
        if (currentTarget == rb.position) //arrived to target
        {            
            //find new target
            pathIndex = (pathIndex + 1) % pathSize;
            currentTarget = path[pathIndex];
            //adjust direction
            directionFromTarget();
        }
        else if (currentState == NPCState.walk)
        {
            move();
        }
            

    }

    void move()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, currentTarget, Time.fixedDeltaTime * walkSpeed));

    }


    void directionFromTarget()
    {
        if (rb.position.x > currentTarget.x)
        {
            direction = Vector2.left;
            animationDirection = 3;
        }
        else if (rb.position.x < currentTarget.x)
        {
            direction = Vector2.right;
            animationDirection = 1;
        }
        else if (rb.position.y < currentTarget.y)
        {
            direction = Vector2.up;
            animationDirection = 0;
        }
        else if (rb.position.y > currentTarget.y)
        {
            direction = Vector2.down;
            animationDirection = 2;
        }
        else direction = Vector2.zero;
    }











    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            engagePlayer();
            this.GetComponent<CircleCollider2D>().radius = 2;
        }
        
        

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && currentState == NPCState.engage)
        {
            currentState = NPCState.walk;
            directionFromTarget();
            this.GetComponent<CircleCollider2D>().radius = 0.5f;
        }    

    }


    void engagePlayer()
    {
        //stop walking but remember path
        currentState = NPCState.engage;

    }



/*//////////////////PLAYER References these, not ideal place for these functons to live. 
 * Character super class??
 */

    public static Vector2 cardinal(Vector2 input)
    {
        //return Vector2.up or down or left or right
        
        float degrees = Vector2.SignedAngle(Vector2.down, input);

        if (degrees <= -135 || degrees >= 135) return Vector2.up;

        if (degrees <= -45) return Vector2.left;

        if (degrees <= 45) return Vector2.down;

        if (degrees <= 135) return Vector2.right;        

        return Vector2.down;
    }

    public static int cardinalToInt(Vector2 v)
    {
        if (v == Vector2.right) return 1;
        if (v == Vector2.down) return 2;
        if (v == Vector2.left) return 3;
        return 0;
    }

    private void OnDisable()
    {
        //TODO replenish health

        transform.position = home;
        pathIndex = 3;
        currentState = NPCState.walk;
    }


    private void OnEnable()
    {
        //get message from the DM / QM
        //messager.setMessage(DM.getMessage(NPC_ID));



    }

    public int getNPCID()
    {
        return NPC_ID;
    }

    public Vector2 intToCardinal(int URDL)
    {
        if (URDL == 1) return Vector2.right;
        if (URDL == 2) return Vector2.down;
        if (URDL == 3) return Vector2.left;
        return Vector2.up;
    }


}


