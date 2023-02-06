using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum State
{
    //does shovel and sword need their own animation? can it be shared as uninteruptible?
    idle, run, sword, shovel, interact, freeze, knockBack
}


public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }


    [SerializeField]
    State currentState;

    GameObject gm;
    Pause_Manager pm;
    Tile_Manager tm;
    Text_Manager textManager;
    Scene_Manager sm;

    [SerializeField]
    protected float baseSpeed;
    [SerializeField]
    protected float sprintSpeed;

    [SerializeField]
    protected float speed;

    protected Animator animator;

    private Rigidbody2D myRigidbody;

    protected bool isWalking = false;

    protected Vector3 zero = Vector3.zero;
    Vector2 zero2 = Vector2.zero;

    public bool knockBack = false;    

    public Vector2 directionFacing;

    Inventory_Manager invManager;

    SpriteRenderer spriteRenderer;

    public Vector2 moveInput;

    AudioSource audioSource;

    Vector2 colliderBottomLeft;
    Vector2 colliderBottomRight;

    public float stairSlope;
    Vector2 stairYcomponent;


    int MAX_HEALTH = 10;
    int MAX_MANA = 10;
    int health;
    int mana;

    int facing;

    public int manaRegen;
    int manaRegenCounter;

    int manaBurn;

    //string[] shovel = { "Shovel_Up", "Shovel_Right", "Shovel_Down", "Shovel_Left" };
    //string[] run = { "run_up", "run_right", "run_down", "run_left" };
    //string[] idle = { "idle_up", "idle_right", "idle_down", "idle_left" };
    //string[] sword = { "sword_up", "sword_right", "sword_down", "sword_left" };


    string[] shovel = { "Shovel_Up", "Shovel_Right", "Shovel_Down", "Shovel_Left" };
    string[] run = { "Run_UP_V2", "Run_Right_V2", "Run_Down_V2", "Run_Left_V2" };
    string[] idle = { "Idle_UP_V2", "Idle_Right_V2", "Idle_Down_V2", "Idle_Left_V2" };
    string[] sword = { "sword_up", "sword_right", "sword_down", "sword_left" };

    Vector2 knockImpulse;

    Vector2 pausedVelocity;


    public AudioClip swordClip;

    public AudioClip[] footstepClips;
    int footStepClip = 0;


    BoxCollider2D bc;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

    }

    private void OnEnable()
    {
        speed = baseSpeed;
    }


    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager");
        pm = gm.GetComponent<Pause_Manager>();
        tm = gm.GetComponent<Tile_Manager>();
        textManager = gm.GetComponent<Text_Manager>();
        sm = gm.GetComponent<Scene_Manager>();
        bc = this.GetComponent<BoxCollider2D>();


        //this is to ensure the player cannot respond to input until 
        //the Scene Manager relinquishes and unfreezes player after Fade in
        freeze();

        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();

        //directionFacing = Vector2.down;
        //facing = 2;

        

        invManager = gm.GetComponent<Inventory_Manager>();

        health = sm.getHealth();
        mana = MAX_MANA;
        mana = 0;
        manaRegenCounter = 0;

        //currentState = State.idle;

        spriteRenderer = this.GetComponent<SpriteRenderer>();

        audioSource = this.GetComponent<AudioSource>();

        stairYcomponent = new Vector2(0,0);

        Player_Persistence pp = sm.getPlayerPersistence();

        this.transform.position = sm.getEntrance().transform.position;
        this.facing = sm.getEntrance().getURDL();
        this.health = sm.getPlayerPersistence().getHealth();

    }



    public void freeze()
    {       
        currentState = State.freeze;
    }

    public void unfreeze()
    {
        currentState = State.idle;
    }

    public void setDirectionFacing(Vector2 direction)
    {
        if (direction == Vector2.up) facing = 0;
        else if (direction == Vector2.right) facing = 1;
        else if (direction == Vector2.down) facing = 2;
        else facing = 3;
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
    }


    private void FixedUpdate()
    {         
        Move();
        
        //manaUpdate();
    }

    void manaUpdate()
    {
        if (mana < MAX_MANA)
        {
            manaRegenCounter += 1;
            if (manaRegenCounter >= manaRegen)
            {
                mana += 1;
                if (mana > MAX_MANA) mana = MAX_MANA;
                manaRegenCounter = 0;
            }
        }
        else
            manaRegenCounter = 0;
    }

    

    public void executePlayerFn(byte code)
    {
        if (code == 0) Sword();
        if (code == 1) Shovel();
        if (code == 2) water();
    }

    public void heal(byte hp)
    {
        health += hp;
        if (health > MAX_HEALTH)
            health = MAX_HEALTH;
    }

    void water()
    {
        tm.waterTile();
    }

    void Sword()
    {
        if (!interruptibleState()) return;

        currentState = State.sword;

        swordSound();
        
    }

    void Shovel()
    {
        if (!interruptibleState()) return;
        
        currentState = State.shovel;
    }

    void AnimationUpdate()
    {
        //check input
        //load proper animation state

        if (currentState == State.idle || currentState == State.freeze)
        {
            animator.Play(idle[facing]);
        }
        else if (currentState == State.run)
        {
            animator.Play(run[facing]);
        }
        else if (currentState == State.shovel)
        {
            animator.Play(shovel[facing]);
        }
        else if (currentState == State.sword) animator.Play(sword[facing]);


    }

    public void Move()
    {

        //simply movement
        if (currentState == State.idle)
        {
            myRigidbody.velocity = zero2;
        }
        else if (currentState == State.run)
        {            
            myRigidbody.velocity = moveInput.normalized * speed;

            stairYcomponent.y = myRigidbody.velocity.x * stairSlope;

            myRigidbody.velocity -= stairYcomponent;
        }
        else if (currentState == State.knockBack) { }
        else if (currentState != State.run)
        {
            myRigidbody.velocity = Vector2.zero;
        }

        //layer sort
        //spriteRenderer.sortingOrder = 0 - (int)this.transform.position.y;
    }

    public void KnockBack(Vector3 pos, float thrust)
    {
        if (currentState == State.knockBack) return;
        currentState = State.knockBack;

        knockImpulse = ((transform.position - pos).normalized) * thrust;

        myRigidbody.velocity = zero2;
        myRigidbody.AddForce(knockImpulse, ForceMode2D.Impulse);

        takeDamage(1);
        StartCoroutine(KnockCo());        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != State.sword) return;

        if (other.gameObject.CompareTag("Enemy"))
        {
            //damage enemy, knock back

            other.GetComponentInParent<Enemy>().KnockBack(myRigidbody.position, 10);           
        }

    }

    private IEnumerator KnockCo()
    {
        yield return new WaitForSeconds(.25f);
        currentState = State.idle;
    }

    public int getHealth()
    {
        return health;
    }

    public int getMana()
    {
        return mana;
    }

    public void takeDamage(int damage)
    {
        if (damage > health)
            health = 0;
        else        
            health -= damage;
    }

    public void toggleSprint(bool input)
    {
        if (input)
            speed = sprintSpeed;
        else
            speed = baseSpeed;
    }

    public Vector2 getDirectionFacing()
    {
        return directionFacing;
    }

    public int getFacing()
    {
        return facing;
    }


    public void inputUpdate(Vector2 input)
    {
        if (currentState == State.freeze) return;
        
        moveInput = input;
        if (interruptibleState())
        {
            if (input == Vector2.zero)
                currentState = State.idle;
            else
            {
                currentState = State.run;
                directionFacing = moveInput;
                if (moveInput.x > 0.9) facing = 1;
                else if (moveInput.x < -0.9) facing = 3;
                else if (moveInput.y > 0.9) facing = 0;
                else if (moveInput.y <-0.9) facing = 2;
            }
                
        }
    }

    bool interruptibleState()
    {
        if (currentState == State.shovel) return false;
        if (currentState == State.sword) return false;
        if (currentState == State.knockBack) return false;
        if (currentState == State.freeze) return false;
        
        return true;
    }

    public void setAnimationIdle()
    {
        currentState = State.idle;
    }

    public float getVelocity() { return myRigidbody.velocity.magnitude; }

    public void dig() { tm.dig(); }

    public bool isFacing(Transform transform)
    {
        Vector2 delta = (Vector2)transform.position - myRigidbody.position;
        return NPC.cardinalToInt(NPC.cardinal(delta)) == facing;
    }


    public void footStep()
    {
        audioSource.clip = footstepClips[footStepClip];
        
        audioSource.Play();

        footStepClip = Random.Range(0, footstepClips.Length);
    }

    public void swordSound()
    {       
        audioSource.clip = swordClip;

        audioSource.time = audioSource.clip.length * 0.35f;

        audioSource.Play();
    }

    public Vector2 getColliderBottomLeft()
    {
        colliderBottomLeft.x = this.transform.position.x - (bc.size.x / 2f) + bc.offset.x;
        colliderBottomLeft.y = this.transform.position.y - (bc.size.y / 2f) + bc.offset.y;
        return colliderBottomLeft;
    }

    public Vector2 getColliderBottomRight()
    {
        colliderBottomRight.x = this.transform.position.x + (bc.size.x / 2f) + bc.offset.x;
        colliderBottomRight.y = this.transform.position.y + (bc.size.y / 2f) + bc.offset.y;
        return colliderBottomRight;
    }

    public void setStairSlope(float co) { this.stairSlope = co; }


}
