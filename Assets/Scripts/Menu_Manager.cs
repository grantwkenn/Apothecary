using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Manager : MonoBehaviour
{
    byte selectedTab;
  
    Input_Manager im;
    Inventory_Manager invMan;

    Menu[] pauseMenuTabs;

    GameObject pauseMenu;

    Menu invMenu, questLog;

    Sprite[] numbers;

    Inventory_Bar_Menu invBarMenu;


    //TODO this will change, find a dynamic way to find the store menu needed in a given scene from an npc
    [SerializeField]
    GameObject storeMenu;

    public GameObject[] storeMenus;

    public Menu currentMenu;
    
    Transform canvas;

    Transform menuSelector;
    Image[] menuImages;

    private void OnEnable()
    {
        im = this.GetComponent<Input_Manager>();
        invMan = this.GetComponent<Inventory_Manager>();
        invBarMenu = GameObject.FindGameObjectWithTag("HUD").transform.Find("Inventory Bar").GetComponent<Inventory_Bar_Menu>();
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        numbers = Resources.LoadAll<Sprite>("NUMBERS");

        pauseMenuTabs = new Menu[2];
        invMenu = pauseMenu.transform.Find("Inventory Menu").GetComponent<Menu>();
        questLog = pauseMenu.transform.Find("Quest Log Menu").GetComponent<Menu>();

        pauseMenuTabs[0] = invMenu;
        pauseMenuTabs[1] = questLog;

        menuSelector = invMenu.transform.Find("Selection");

        selectedTab = 0;





    }



    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("HUD").transform;
        //GameObject clone = Instantiate(storeMenu, canvas);
        //clone.SetActive(true);

        pauseMenu.SetActive(false);
        invMenu.gameObject.SetActive(false);
        questLog.gameObject.SetActive(false);

        refresh();
        
    }

    // Update is called once per frame
    void Update()
    {
        //does this update when not active??

    }

    public void refresh()
    {
        invBarMenu.refresh();
    }


    public void loadShopMenu(byte shopId)
    {
        Time.timeScale = 0;
        im.enableMenuInput();

        currentMenu = storeMenus[shopId].GetComponent<Menu>();
        storeMenu = Instantiate(storeMenus[shopId], canvas);     
        
    }

    public void closeMenu()
    {  
        

        if(!currentMenu.CompareTag("Pause Tab"))
            Destroy(storeMenu);
        else
        {
            currentMenu.gameObject.SetActive(false);
            pauseMenu.SetActive(false);     
        }
            

        currentMenu = null;

        Time.timeScale = 1f;

    }


    public void handleInput(direction urdl)
    {
        //TODO change this to a current menu variable to include other menus
        pauseMenuTabs[selectedTab].GetComponent<Menu>().handleInput(urdl);
    }

    public void toggleBarSelection(direction input)
    {
        invBarMenu.handleInput(input);
    }

    public void pauseGame()
    {
        Time.timeScale = 0f;

        currentMenu = pauseMenuTabs[selectedTab];
        pauseMenu.SetActive(true);
        currentMenu.gameObject.SetActive(true);
    }


    public void incrementTab(sbyte value)
    {

        pauseMenuTabs[selectedTab].gameObject.SetActive(false);
        
        int sel = selectedTab + value;
        selectedTab = (byte)(selectedTab + value);

        if (sel >= pauseMenuTabs.Length)
            selectedTab = 0;
        if (sel < 0)
            selectedTab = (byte)(pauseMenuTabs.Length - 1);


        pauseMenuTabs[selectedTab].gameObject.SetActive(true);
    }

    public Sprite getDigitSprite(byte digit)
    {
        return (numbers[digit]);

    }
}
