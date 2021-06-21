using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    walk, attack, interact, drawn
}


public class Player : MonoBehaviour
{
    public PlayerState currentState;

    [SerializeField]
    protected float baseSpeed;

    [SerializeField]
    protected float speed;

    protected Animator animator;

    private Rigidbody2D myRigidbody;

    protected Vector3 change;

    protected bool isAttacking = false;

    protected bool isWalking = false;

    protected Vector3 zero = Vector3.zero;

    public bool knockBack = false;


    public GameObject arrow;
    
    private bool bowReady = false;
    private bool drawingBow = false;

    private Vector2 directionFacing;
    private int facing = 0;

    public float arrowOffsetX =0;
    public float arrowOffsetY =0;


    public Transform shotPoint;
    public float bowWeight = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        
        directionFacing = Vector2.down;

        //direction = Vector2.down;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();

        currentState = PlayerState.walk;

        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);

    }

    // Update is called once per frame
    void Update()
    {
        //get movement input
        change = zero;
        if(canMove())
        {
            change.x = Input.GetAxisRaw("Horizontal"); //* speed * Time.fixedDeltaTime;
            change.y = Input.GetAxisRaw("Vertical"); //* speed * Time.fixedDeltaTime;
            if (change.x > 0) facing = 1;
            else if (change.x < 0) facing = 3;
            else if (change.y > 0) facing = 2;
            else if (change.y < 0) facing = 0;
        }

        if (Input.GetButtonDown("Sprint"))
        {
            speed = baseSpeed * 2;
        }

        if (Input.GetButtonUp("Sprint"))
        {
            speed = baseSpeed;
        }


        //get sword input and start coroutine
        if (Input.GetButtonDown("attack"))
        {
            if(!Input.GetButton("Bow"))
            {
                StartCoroutine(AttackCo());
            }
            
        }


        //get bow input and start coroutine
        else if (Input.GetButtonDown("Bow") && drawingBow == false)
        {
            drawingBow = true;
            StartCoroutine(DrawBowCo()); 
        }

        //
        else if (canMove())
        {
            WalkAnimationUpdate();
        }

        //check bow is released this frame
        if(Input.GetButtonUp("Bow"))
        {
            if (bowReady) ShootBow();
            bowReady = false;
            drawingBow = false;
            
            if (currentState == PlayerState.drawn) currentState = PlayerState.walk;
            animator.SetBool("drawingBow", false);
            animator.SetBool("drawn", false);

        }



    }

    private void FixedUpdate()
    {
        if (canMove() && !drawingBow && !knockBack)
            MoveCharacter();
    }


    private IEnumerator AttackCo()
    {
        isAttacking = true;
        animator.SetBool("attack", isAttacking);
        currentState = PlayerState.attack;
        yield return null;
        isAttacking = false;
        animator.SetBool("attack", isAttacking);
        change = zero;
        yield return new WaitForSeconds(0.3f);
        currentState = PlayerState.walk;
    }

    private IEnumerator DrawBowCo()
    {
        //isAttacking = true;
        animator.SetBool("drawingBow", true);
        yield return null;
        change = zero;
        yield return new WaitForSeconds(0.5f);
        if(drawingBow)
        {
            currentState = PlayerState.drawn;
            bowReady = true;
            drawingBow = false;
            animator.SetBool("drawn", true);
            animator.SetBool("drawingBow", false);
        }

    }

    private void WalkAnimationUpdate()
    {
        if (change != zero)
        {
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("walking", true);                     
        }
        else
        {
            animator.SetBool("walking", false);
        }
    }

    private bool canMove()
    {
        if(drawingBow) return false;
        return (currentState == PlayerState.walk || currentState == PlayerState.drawn);
    }

    public void MoveCharacter()
    {
        //myRigidbody.MovePosition(myRigidbody.transform.position + change * speed * Time.fixedDeltaTime * Time.timeScale);
        myRigidbody.velocity = change * speed;
    }

    private void ShootBow()
    {
        Vector2 offset0 = new Vector2(0, 0);
        Vector2 offset1 = new Vector3(0.0f, 0.3125f);
        Vector2 offset2 = new Vector2(0, 0);
        Vector2 offset3 = new Vector3(0.0f, 0.3125f);

        Vector2 offset = new Vector2(0, 0);

        //get directionFacing, and offset
        if (facing == 0)
        {
            directionFacing = Vector2.down;
            offset.y -= arrowOffsetY;
        }
        else if (facing == 1)
        {
            directionFacing = Vector2.right;
            offset.x += arrowOffsetX;
            //shotPoint.transform.position = new Vector3(transform.position.x + offset1.x, transform.position.y + offset1.y, 0.0f);
        }
        else if (facing == 2)
        {
            directionFacing = Vector2.up;
            offset.y += arrowOffsetY;
        }
        else if (facing == 3)
        {
            directionFacing = Vector2.left;
            offset.x -= arrowOffsetX;
            //shotPoint.transform.position = new Vector2(transform.position.x + offset3.x, transform.position.y + offset3.y);
        }

        //update shotPoint rotation based on direction facing
        float z = facing * 90.0f;
        shotPoint.eulerAngles = new Vector3(0, 0, z);

        //update shotPoint offset

        //shotPoint.transform.position = new Vector2(shotPoint.transform.position.x + offset.x, shotPoint.transform.position.y + offset.y);
        Vector2 offsetV = new Vector2(shotPoint.transform.position.x + offset.x, shotPoint.transform.position.y + offset.y);

        GameObject newArrow = Instantiate(arrow, offsetV, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = directionFacing * bowWeight;

    }

    public void KnockBack(Vector3 pos, float thrust)
    {

        if (knockBack) return;
        
        Vector2 difference = transform.position - pos;
        difference = difference.normalized * thrust;

        myRigidbody.AddForce(difference, ForceMode2D.Impulse);
        knockBack = true;
        StartCoroutine(KnockCo());

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //damage enemy, knock back

            other.GetComponentInParent<Enemy>().KnockBack(myRigidbody.position, 10);           
        }

    }

    private IEnumerator KnockCo()
    {
        yield return new WaitForSeconds(.25f);
        myRigidbody.velocity = Vector2.zero;
        knockBack = false;
    }

}
