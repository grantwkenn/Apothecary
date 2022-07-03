using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


enum InputState { inGame, paused }

public class Input_Manager : MonoBehaviour
{
    Player player;

    PlayerControls controls;

    Inventory_Manager invManager;
    Dialogue_Manager dialogueManager;
    Pause_Manager pauseManager;

    [SerializeField]
    Camera mainCamera;
    CameraMovement cm;

    InputState inputState;

    //get reference to the quick bar
    bool inventoryBarWake = false;
    int inventoryTimer;

    int barSelection = 0;

    Vector2 moveInput;
    Vector2 direction;
    Vector2 scroll;

    int scrollUpHoldCounter = 0;
    int scrollDownHoldCounter = 0;

    bool scrollUpHold = false;
    bool scrollDownHold = false;

    bool scrollingUp = false;
    bool scrollingDown = false;

    int scrollTimer = 0;


    private void Awake()
    {
        GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
        invManager = gm.GetComponent<Inventory_Manager>();
        dialogueManager = gm.GetComponent<Dialogue_Manager>();
        pauseManager = gm.GetComponent<Pause_Manager>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        cm = mainCamera.GetComponent<CameraMovement>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        controls = new PlayerControls();

        //WASD / Left Stick
        controls.Gameplay.Move.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += context => moveInput = Vector2.zero;

        //Scroll / RB,LB
        controls.Gameplay.Select.performed += context => scroll = context.ReadValue<Vector2>();
        controls.Gameplay.Select.canceled += context => scrollRelease();

        //Left Click / A Button
        controls.Gameplay.Use.performed += context => aButton();

        //SPACE / B Button
        controls.Gameplay.CancelSprint.performed += context => cancelPress();
        controls.Gameplay.CancelSprint.canceled += context => cancelRelease();

        //ENTER / START Button
        controls.Gameplay.Pause.performed += context => togglePause();

        //Z Key
        controls.Gameplay.Zoom.performed += context => toggleZoom();

        direction = new Vector2();

        inputState = InputState.inGame;

    }

    void Start()
    {
        inventoryTimer = 0;
    }


    ////  CONTROLS  ///////////////////////////////
    
    // PAUSE
    void togglePause()
    {
        pauseManager.togglePause();

        if (pauseManager.checkPaused())
        {
            inputState = InputState.paused;
        }
        else
        {
            inputState = InputState.inGame;
        }
    }

    ////// A
    void aButton()
    {
        if (inputState == InputState.inGame)
        {
            if (dialogueManager.AwaitingInput())
                dialogueManager.displayText();
            else
                player.Use();
        }
    }

    ////// B
    void cancelPress()
    {
        if(inputState == InputState.inGame)
        {
            player.toggleSprint(true);
        }

    }
    void cancelRelease()
    {
        if (inputState == InputState.inGame)
        {
            player.toggleSprint(false);
        }
    }



    /// LB / RB
    void scrollRelease()
    {
        scrollUpHoldCounter = 0;
        scrollDownHoldCounter = 0;
        scrollUpHold = false;
        scrollDownHold = false;
        scroll = Vector2.zero;
        scrollingDown = false;
        scrollingUp = false;

    }


    //Z Zoom
    void toggleZoom()
    {
        cm.toggleZoom();
    }


    // Update is called once per frame
    void Update()
    {
        //Dead Zone Rejection
        if (moveInput.magnitude < 0.1) moveInput = Vector2.zero;

        float angle = Vector2.Angle(moveInput, Vector2.up);

        int tolerance = 20;

        if (moveInput == Vector2.zero)
            direction = Vector2.zero;

        
        
        
        else if (0 <= angle && angle < 0 + tolerance) direction = Vector2.up;
        else if (0 + tolerance <= angle && angle < 90 - tolerance) { direction.x = 1; direction.y = 1; }
        else if (90 - tolerance <= angle && angle < 90 + tolerance) { direction = Vector2.right; }
        else if (90 + tolerance <= angle && angle < 180 - tolerance) { direction.x = 1; direction.y = -1; }
        else if (180 - tolerance <= angle) { direction = Vector2.down; }

        if (moveInput.normalized.x < 0)
            direction.x = 0-direction.x;


        if (inputState == InputState.inGame)
            player.inputUpdate(direction);

        else if (inputState == InputState.paused)
            invManager.inputUpdate(direction);

        if (scrollingUp || scrollingDown)
            return;

        //Inventory Select L/R or Scroll
        if(inputState == InputState.inGame)
        {
            if (scroll.y < 0)
            {
                invManager.toggleSelection(1);
                scrollUpHold = true;
            }
            else if (scroll.y > 0)
            {
                invManager.toggleSelection(-1);
                scrollDownHold = true;
            }
            scroll = Vector2.zero;
        }



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

        if (scrollUpHold)
        {
            scrollUpHoldCounter++;
            if (scrollUpHoldCounter == 10)
            {
                scrollingUp = true;
                scrollUpHold = false;
                scrollUpHoldCounter = 0;
            }

        }

        else if (scrollDownHold)
        {
            scrollDownHoldCounter++;
            if(scrollDownHoldCounter == 10)
            {
                scrollingDown = true;
                scrollDownHold = false;
                scrollDownHoldCounter = 0;
            }
        }

        if(scrollingDown || scrollingUp)
        {
            scrollTimer++;
            if(scrollTimer == 4)
            {
                if(scrollingDown) invManager.toggleSelection(-1);
                else if (scrollingUp) invManager.toggleSelection(1);
                scrollTimer = 0;
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

    public Vector2 readInput() { return moveInput; }

}
