using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum direction { up, down, left, right }

public class Input_Manager : MonoBehaviour
{
    Player player;

    PlayerControls controls;

    Inventory_Manager invManager;
    Dialogue_Manager dialogueManager;
    Menu_Manager menuManager;

    [SerializeField]
    Camera mainCamera;
    CameraManager cm;

    //InputState inputState;

    //get reference to the quick bar
    bool inventoryBarWake = false;
    int inventoryTimer;

    int barSelection = 0;

    Vector2 moveInput;
    Vector2 direction;


    private void Awake()
    {
        GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
        invManager = gm.GetComponent<Inventory_Manager>();
        dialogueManager = gm.GetComponent<Dialogue_Manager>();
        menuManager = gm.GetComponent<Menu_Manager>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cm = mainCamera.GetComponent<CameraManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();


        controls = new PlayerControls();

        //Gameplay Movement -- WASD / Left Stick
        controls.Gameplay.Move.performed += context =>
        {
            moveInput = context.ReadValue<Vector2>();
        };
        controls.Gameplay.Move.canceled += context => moveInput = Vector2.zero;



        //Menu Navigation -- WASD / Left Stick
        controls.Menus.Up.performed += context => menuManager.handleInput(global::direction.up);
        controls.Menus.Down.performed += context => menuManager.handleInput(global::direction.down);
        controls.Menus.Left.performed += context => menuManager.handleInput(global::direction.left);
        controls.Menus.Right.performed += context => menuManager.handleInput(global::direction.right);




        //Scroll / RB,LB
        //controls.Gameplay.Bumpers.performed += context => scroll = context.ReadValue<Vector2>();
        //controls.Gameplay.Bumpers.canceled += context => scrollRelease();

        controls.Gameplay.LB.performed += context => menuManager.toggleBarSelection(global::direction.left);
        controls.Gameplay.RB.performed += context => menuManager.toggleBarSelection(global::direction.right);

        controls.Menus.LB.performed += context => menuManager.incrementTab(-1);
        controls.Menus.RB.performed += context => menuManager.incrementTab(1);


        //Left Click / A Button
        controls.Gameplay.Use.performed += context => aButton();


        //SPACE / B Button
        controls.Gameplay.CancelSprint.performed += context => bButton();
        controls.Gameplay.CancelSprint.canceled += context => bRelease();

        //Q / Y Button
        controls.Gameplay.Discard.performed += context => invManager.discardSelection();


        //ENTER / START Button
        controls.Gameplay.Pause.performed += context => pauseGame();
        controls.Menus.Start.performed += context => closeMenu();

        //Z Key
        controls.Gameplay.Zoom.performed += context => toggleZoom();

        direction = new Vector2();

        //inputState = InputState.ignoreInput;

        controls.Menus.Disable();

    }

    void Start()
    {
        inventoryTimer = 0;

    }



    ////  CONTROLS  ///////////////////////////////
    
    // PAUSE
    void pauseGame()
    {
        menuManager.pauseGame();
        controls.Gameplay.Disable();
        controls.Menus.Enable();

    }

    void closeMenu()
    {
        menuManager.closeMenu();
        controls.Menus.Disable();
        controls.Gameplay.Enable();

    }

    ////// A
    void aButton()
    {
        if (dialogueManager.AwaitingInput())
            dialogueManager.displayText();
        else
            invManager.useItem();

    }

    ////// B
    void bButton()
    {
        player.toggleSprint(true);
    }
    void bRelease()
    {
        player.toggleSprint(false);
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

        if(controls.Gameplay.enabled)
            player.inputUpdate(direction);





    }

    private void FixedUpdate()
    {






        if (inventoryBarWake == true)
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
        //controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    public Vector2 readInput() { return moveInput; }


    public void enableMenuInput()
    {
        controls.Gameplay.Disable();
        controls.Menus.Enable();
    }

    public void enableGameplay()
    {
        controls.Gameplay.Enable();
        controls.Menus.Disable();

        if(controls.Gameplay.CancelSprint.IsPressed())
            player.toggleSprint(true);

        moveInput = controls.Gameplay.Move.ReadValue<Vector2>();
    }

    public void button(int index)
    {

    }
}
