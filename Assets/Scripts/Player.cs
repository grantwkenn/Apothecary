using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    walk, attack, shovel, interact, drawn, freeze
}

public enum State
{
    idle, run, sword, shovel, interact, freeze, knockBack
}


public class Player : MonoBehaviour
{
    public bool startInBed;
    
    public PlayerState currentState;

    State currState;

    GameObject gm;

    [SerializeField]
    protected float baseSpeed;
    [SerializeField]
    protected float sprintSpeed;

    [SerializeField]
    protected float speed;

    protected Animator animator;

    private Rigidbody2D myRigidbody;

    protected Vector3 movement;

    protected bool isWalking = false;

    protected Vector3 zero = Vector3.zero;
    Vector2 zero2 = Vector2.zero;

    public bool knockBack = false;    

    public Vector2 directionFacing;

    Inventory_Manager invManager;

    Vector2 moveInput;

    int MAX_HEALTH = 10;
    int MAX_MANA = 10;
    int health;
    int mana;

    int facing;

    public int manaRegen;
    int manaRegenCounter;

    int manaBurn;

    string[] shovel = { "Shovel_Up", "Shovel_Right", "Shovel_Down", "Shovel_Left" };
    string[] run = { "run_up", "run_right", "run_down", "run_left" };
    string[] idle = { "idle_up", "idle_right", "idle_down", "idle_left" };
    string[] sword = { "sword_up", "sword_right", "sword_down", "sword_left" };

    Vector2 knockImpulse;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager");


        if (startInBed)
        {
            this.transform.position = 
            GameObject.FindGameObjectWithTag("Bed").transform.position;
        }

        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();

        currentState = PlayerState.walk;

        directionFacing = Vector2.down;
        facing = 2;

        speed = baseSpeed;

        invManager = gm.GetComponent<Inventory_Manager>();

        health = MAX_HEALTH;
        mana = MAX_MANA;
        mana = 0;
        manaRegenCounter = 0;

        currState = State.idle;

    }

    public void freeze()
    {
        currState = State.freeze;
        //myRigidbody.velocity = zero;
    }

    public void unfreeze()
    {
        currState = State.idle;
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
        //Layer Sorting
        this.GetComponent<SpriteRenderer>().sortingOrder = -1 * (int)this.transform.position.y;
    }

    private void FixedUpdate()
    {
              
        AnimationUpdate();

        Move2();

        manaUpdate();

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

    public void Use()
    {
        string itemName = invManager.getSelectedItem().itemName;

        if (itemName == "Sword")
            Sword();
        else if (itemName == "Shovel")
            Shovel();

        else if (itemName == "Red Potion" && health < MAX_HEALTH)
        {
            heal(3);
            invManager.discardSelection();
        }
    }
    
    void Sword()
    {
        if (!interruptibleState()) return;
        
        currState = State.sword;
        
    }

    void Shovel()
    {
        if(interruptibleState())
        {
            currState = State.shovel;
        }
    }


    /*
    private void WalkAnimationUpdate()
    {
        if (moveInput != zero2) //input present
        {
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveY", moveInput.y);
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }
        //???
        directionFacing.x = animator.GetFloat("moveX");
        directionFacing.y = animator.GetFloat("moveY");
        ///
    }*/

    /*
    private bool canMove()
    {
        if (currentState == PlayerState.walk) return true;
        if (currentState == PlayerState.drawn) return true;
        else
            myRigidbody.velocity = Vector2.zero;
        return false;

    }*/

    void AnimationUpdate()
    {
        //check input
        //load proper animation state
        if (currState == State.idle || currState == State.freeze)
        {
            animator.Play(idle[facing]);
        }
        else if (currState == State.run)
        {
            animator.Play(run[facing]);
        }
        else if (currState == State.shovel)
        {
            animator.Play(shovel[facing]);
        }
        else if (currState == State.sword) animator.Play(sword[facing]);
    }

    public void Move2()
    {
        //simply movement
        if(currState == State.idle)
        {
            myRigidbody.velocity = zero2;
        }
        if(currState == State.run)
        {
            myRigidbody.velocity = moveInput.normalized * speed;
        }
        else if(currState == State.knockBack)
        {
            myRigidbody.velocity = zero2;
            myRigidbody.AddForce(knockImpulse, ForceMode2D.Impulse);
        }
        else if(currState != State.run)
        {
            myRigidbody.velocity = Vector2.zero;
        }
            
    }

    /*
    public void Move()
    {
        if (knockBack) return;

        //get movement input
        movement = zero;
        //moveInput = zero2;

        if (canMove())
        {           
            
            // Clamp directions to 8 Cardinal directions
            
            float angle = Vector2.SignedAngle(moveInput, Vector2.up);
            
            if(angle == 0 && moveInput.magnitude != 0)
            {
                moveInput = Vector2.up;
                facing = 0;
            }

            /////////MAKE THIS NARROWER MORE RESPONSIVE GAMEPLAY AND ONLY SMALL REJECTION IN 4 DIRECTIONS
            ///D PAD Issues
            
            if(angle > 0) //check UR, R, DR, D
            {
                if (angle <= 20) { moveInput = Vector2.up; facing = 0; } // UP
                else if (20 < angle && angle <= 70) { moveInput = new Vector2(1, 1); facing = 1; } //UP-RIGHT
                else if (70 < angle && angle <= 110) { moveInput = new Vector2(1, 0); facing = 1; } // RIGHT
                else if (110 < angle && angle <= 160) { moveInput = new Vector2(1, -1); facing = 1; } // DOWN-RIGHT
                else if (160 < angle) { moveInput = Vector2.down; facing = 2; } //DOWN
            }

            else if(angle < 0)
            {
                angle = 0 - angle;
                if (angle <= 20) { moveInput = Vector2.up; facing = 0; }
                else if (20 < angle && angle <= 70) { moveInput = new Vector2(-1, 1); facing = 3; }
                else if (70 < angle && angle <= 110) { moveInput = new Vector2(-1, 0); facing = 3; }
                else if (110 < angle && angle <= 160) { moveInput = new Vector2(-1, -1); facing = 3; }
                else if (160 < angle) { moveInput = Vector2.down; facing = 2; }
            }
            


            //movement = moveInput;

            /*
            if (movement.x > 0) facing = 1;
            else if (movement.x < 0) facing = 3;
            else if (movement.y > 0) facing = 2;
            else if (movement.y < 0) facing = 0;
            


            myRigidbody.velocity = moveInput.normalized * speed;
            //myRigidbody.AddForce(move.normalized * speed);

            
        }
    } */

    public void KnockBack(Vector3 pos, float thrust)
    {
        if (currState == State.knockBack) return;
        currState = State.knockBack;

        knockImpulse = ((transform.position - pos).normalized) * thrust;

        takeDamage(1);
        StartCoroutine(KnockCo());        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currState != State.sword) return;

        if (other.gameObject.CompareTag("Enemy"))
        {
            //damage enemy, knock back

            other.GetComponentInParent<Enemy>().KnockBack(myRigidbody.position, 10);           
        }

    }

    private IEnumerator KnockCo()
    {
        yield return new WaitForSeconds(.25f);
        currState = State.idle;
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

    void heal(int hp)
    {
        health += hp;
        if (health > MAX_HEALTH)
            health = MAX_HEALTH;
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


    public void inputUpdate(Vector2 input)
    {
        moveInput = input;
        if (interruptibleState())
        {
            if (input == Vector2.zero)
                currState = State.idle;
            else
            {
                currState = State.run;
                directionFacing = moveInput;
                if (moveInput.x > 0) facing = 1;
                else if (moveInput.x < 0) facing = 3;
                else if (moveInput.y > 0) facing = 0;
                else
                    facing = 2;
            }
                
        }
    }



    bool interruptibleState()
    {
        if (currState == State.shovel) return false;
        if (currState == State.sword) return false;
        if (currState == State.knockBack) return false;
        if (currState == State.freeze) return false;
        
        return true;
    }

    public void setAnimationIdle()
    {
        currState = State.idle;
    }

}
