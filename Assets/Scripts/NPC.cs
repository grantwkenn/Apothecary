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
    GameObject gm;
    Dialogue_Manager DM;
    Messager messager;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    public Vector2 moveVelocity;
    Vector2 prevPosition;

    public float walkSpeed;

    [SerializeField]
    NPC_Behavior npcBehavior;

    
    public Vector2 destination;
    public Vector2 direction;

    public NPCState currentState;
    NPCState interruptedState = NPCState.idle;


    protected Animator animator;


    int animationDirection;

    static string[] idle = { "Idle_U", "Idle_R", "Idle_D", "Idle_L" };
    static string[] walk = { "Walk_U", "Walk_R", "Walk_D", "Walk_L" };

    Vector2 prevDirection;

    Vector2 home;

    public bool dontMove;

    //make these private and only controlled by a cashier's switch.

    public bool shopOpen;
    public byte shopID;

    int height;

    public int angle;

    static float engagedRadius = 2f;
    static float roamingRadius = 1f;

    private void OnEnable()
    {
        //get message from the DM / QM
        //messager.setMessage(DM.getMessage(NPC_ID));
        this.GetComponent<CircleCollider2D>().radius = roamingRadius;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.FindGameObjectWithTag("GameManager");
        DM = gm.GetComponent<Dialogue_Manager>();
        messager = this.GetComponentInParent<Messager>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        rb = this.GetComponent<Rigidbody2D>();

        Resource_Manager rman = gm.GetComponent<Resource_Manager>();

        if (npcBehavior != null)
        {
            npcBehavior.init();
            getNextBehavior();
        }
        else
            dontMove = true;
        


        //Manually Round location to integer X and Y (5.44, 10.7) --> (5, 11)
        Vector2 floorXY = new Vector2(Mathf.RoundToInt(rb.position.x), Mathf.RoundToInt(rb.position.y));
        rb.position = floorXY;


        animator = GetComponent<Animator>();



        home = rb.position;

        prevPosition = rb.position;


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
            direction = player.transform.position;
            updateHeading();
        }
        else if (!dontMove)
            simpleAI();
    }

    void getNPCData()
    {

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
        //TODO redo this function completely

        if (currentState == NPCState.walk)
        {
            move();
        }            

    }

    void getNextBehavior()
    {
        destination = npcBehavior.getNextDestination();

        if(destination == rb.position || destination == Vector2.zero)
        {
            currentState = NPCState.idle;
            animationDirection = npcBehavior.getURDL();
        }

        else
        {
            currentState = NPCState.walk; 
            direction = destination;
            updateHeading();
        }
    }

    void move()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, destination, Time.fixedDeltaTime * walkSpeed));

        if (destination == rb.position) //arrived to target
        {
            getNextBehavior();
        }

        else //still moving towards target
        {
            moveVelocity = rb.position - prevPosition;           
        }

        prevPosition = rb.position;

    }


    void updateHeading()
    {
        //TODO round the angle to avoid flickering on float math
        
        angle = (int)System.Math.Round(Vector2.SignedAngle(direction - rb.position, Vector2.up));

        if(angle >= 0)
        {
            if (angle <= 45) animationDirection = 0;
            else if (angle <= 135) animationDirection = 1;
            else if (angle <= 180) animationDirection = 2;
        }
        else if(angle < 0)
        {
            if (angle <= -135) animationDirection = 2;
            else if (angle <= -45) animationDirection = 3;
            else animationDirection = 0;
        }
        
        
    }
   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            this.GetComponent<CircleCollider2D>().radius = engagedRadius;
            engagePlayer();
        }        

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && currentState == NPCState.engage)
        {
            disengagePlayer();
            this.GetComponent<CircleCollider2D>().radius = roamingRadius;
        }    

    }

    void disengagePlayer()
    {
        currentState = interruptedState;

        if(currentState == NPCState.walk)
        {
            direction = destination;
            updateHeading();
        }

        else if (currentState == NPCState.idle)
        {
            animationDirection = npcBehavior.getURDL();
        }
        
    }


    void engagePlayer()
    {
        //stop walking but remember path
        interruptedState = currentState;
        currentState = NPCState.engage;
        direction = player.transform.position;
        updateHeading();
    }



/*//////////////////PLAYER References these, not ideal place for these functons to live. 
 * Character super class??
 */

    public static Vector2 cardinal(Vector2 input)
    {
        //return Vector2.up or down or left or right
        //TODO can this code merge with the animation direction system?
        
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
        currentState = NPCState.walk;
    }

    public int getNPCID()
    {
        return NPC_ID;
    }

    public bool isOpen()
    {
        return this.shopOpen;
    }

    public byte getShopID() { return this.shopID; }

    public static Vector2 URDLtoVector2(int urdl)
    {
        if (urdl == 0) return Vector2.up;
        if (urdl == 1) return Vector2.left;
        if (urdl == 2) return Vector2.down;
        return Vector2.right;
    }
}


