using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Manager : MonoBehaviour
{
    Player player;

    PlayerControls controls;

    Inventory_Manager invManager;
    Text_Manager txtManager;

    //get reference to the quick bar
    bool inventoryBarWake = false;
    int inventoryTimer;

    int barSelection = 0;
    int barSize = 11;

    Vector2 movement;
    Vector2 scroll;


    private void Awake()
    {
        GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
        invManager = gm.GetComponent<Inventory_Manager>();
        txtManager = gm.GetComponent<Text_Manager>();
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        controls = new PlayerControls();

        controls.Gameplay.Use.performed += context => aButton();

        controls.Gameplay.Move.performed += context => movement = context.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += context => movement = Vector2.zero;
        controls.Gameplay.Select.performed += context => scroll = context.ReadValue<Vector2>();

        controls.Gameplay.CancelSprint.performed += context => cancelPress();
        controls.Gameplay.CancelSprint.canceled += context => cancelRelease();
    }


    void Start()
    {
        inventoryTimer = 0;
    }

    void cancelPress()
    {
        player.toggleSprint(true);
    }

    void cancelRelease()
    {
        player.toggleSprint(false);
    }

    void aButton()
    {
        if (txtManager.getMessageTrigger() && player.getDirectionFacing() == Vector2.up)
        {
            txtManager.displayMessage();
        }
        else
            player.Use();
    }


    // Update is called once per frame
    void Update()
    {
        //Dead Zone Rejection
        if (movement.magnitude < 0.1) movement = Vector2.zero;

        player.inputUpdate(movement);

        //Inventory Select L/R or Scroll
        if (scroll.y < 0)
        {
            //barSelection -= 1;
            //if (barSelection < 0) barSelection = barSize - 1;
            invManager.toggleSelection(1);
        }
        else if (scroll.y > 0)
        {
            //barSelection += 1;
            //if (barSelection >= barSize) barSelection = 0;
            invManager.toggleSelection(-1);
        }
        scroll = Vector2.zero;

    }

    private void FixedUpdate()
    {
        if(inventoryBarWake == true)
        {
            inventoryTimer++;
            if(inventoryTimer > 150) // 5 seconds at 30Hz
            {
                inventoryBarWake = false;
                inventoryTimer = 0;
            }
        }
        
    }

    public bool getInventoryBarWakeStatus()
    {
        return inventoryBarWake;
        
    }

    public int getBarSelection() { return barSelection; }


    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    public Vector2 readInput() { return movement; }
}
