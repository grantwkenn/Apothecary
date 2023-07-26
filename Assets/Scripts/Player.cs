using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum State
{
    //does shovel and sword need their own animation? can it be shared as uninteruptible?
    idle, run, sword, hoe, water, interact, freeze, knockBack
}


public class Player : MonoBehaviour
{
    public bool debugMode;

    public static Player Instance { get; private set; }


    [SerializeField]
    State currentState;

    GameObject gm;
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

    SpriteMask tallGrassMask;

    //string[] shovel = { "Shovel_Up", "Shovel_Right", "Shovel_Down", "Shovel_Left" };
    //string[] run = { "run_up", "run_right", "run_down", "run_left" };
    //string[] idle = { "idle_up", "idle_right", "idle_down", "idle_left" };
    //string[] sword = { "sword_up", "sword_right", "sword_down", "sword_left" };


    string[] hoe = { "hoe_U", "hoe_R", "hoe_D", "hoe_L" };
    string[] run = { "run_U", "run_R", "run_D", "run_L" };
    string[] idle = { "idle_U", "idle_R", "idle_D", "idle_L" };
    string[] sword = { "sword_U", "sword_R", "sword_D", "sword_L" };
    string[] water = { "water_U", "water_R", "water_D", "water_L" };

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

        tallGrassMask = this.transform.Find("Tall Grass Mask").GetComponent<SpriteMask>();

    }

    private void OnEnable()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager");
        tm = gm.GetComponent<Tile_Manager>();
        textManager = gm.GetComponent<Text_Manager>();
        sm = gm.GetComponent<Scene_Manager>();
        bc = this.GetComponent<BoxCollider2D>();
        Player_Persistence pp = sm.getPlayerPersistence();

        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        invManager = gm.GetComponent<Inventory_Manager>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        audioSource = this.GetComponent<AudioSource>();

        stairYcomponent = new Vector2(0, 0);

        speed = baseSpeed;

        freeze();
    }


    // Start is called before the first frame update
    void Start()
    {


        mana = MAX_MANA;
        mana = 0;
        manaRegenCounter = 0;



        loadPersistenceData();


    }



    void loadPersistenceData()
    {       
        if(!debugMode)
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
        if (!interruptibleState()) return;
        
        if (code == 0) Sword();
        if (code == 1) Hoe();
        if (code == 2) Water();
    }

    public void heal(byte hp)
    {
        health += hp;
        if (health > MAX_HEALTH)
            health = MAX_HEALTH;
    }

    void Water()
    {
        currentState = State.water;
    }

    //used by animation
    void waterTile()
    {
        tm.waterTile();
    }

    void Sword()
    {
        currentState = State.sword;

        //swordSound();       
    }

    void Hoe()
    {
        
        currentState = State.hoe;
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
        else if (currentState == State.hoe)
        {
            animator.Play(hoe[facing]);
        }
        else if (currentState == State.water)
        {
            animator.Play(water[facing]);
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
        //this could use a map for increased speed?

        //do we need different state for each tool? hoe, water, etc.?
        if (currentState == State.hoe) return false;
        if (currentState == State.sword) return false;
        if (currentState == State.water) return false;
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


    public void setStairSlope(float co) { this.stairSlope = co; }



}
