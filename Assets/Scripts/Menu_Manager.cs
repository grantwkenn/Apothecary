using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Manager : MonoBehaviour
{
    byte selectedTab;
    
    Input_Manager im;
    Inventory_Manager invMan;

    GameObject[] pauseMenuTabs;

    [SerializeField]
    GameObject pauseMenu;

    GameObject invMenu, questLog;

    
    [SerializeField]
    GameObject storeMenu;

    public GameObject[] storeMenus;

    public GameObject currentMenu;
    
    Transform canvas;

    Transform menuSelector;
    Image[] menuImages;

    private void OnEnable()
    {
        im = this.GetComponent<Input_Manager>();
        invMan = this.GetComponent<Inventory_Manager>();

        pauseMenuTabs = new GameObject[2];
        invMenu = pauseMenu.transform.Find("Inventory Menu").gameObject;
        questLog = pauseMenu.transform.Find("Quest Log Menu").gameObject;

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
        invMenu.SetActive(false);
        questLog.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        //does this update when not active??

    }


    public void loadShopMenu(byte shopId)
    {
        Time.timeScale = 0;
        im.enableMenuInput();

        currentMenu = Instantiate(storeMenus[shopId], canvas);
        currentMenu.SetActive(true);      
        
    }

    public void closeMenu()
    {  
        

        if(!currentMenu.CompareTag("Pause Tab"))
            Object.Destroy(currentMenu);
        else
        {
            currentMenu.SetActive(false);
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

    public void pauseGame()
    {
        Time.timeScale = 0f;

        currentMenu = pauseMenuTabs[selectedTab];
        pauseMenu.SetActive(true);
        currentMenu.SetActive(true);
    }


    public void incrementTab(sbyte value)
    {

        pauseMenuTabs[selectedTab].SetActive(false);
        
        int sel = selectedTab + value;
        selectedTab = (byte)(selectedTab + value);

        if (sel >= pauseMenuTabs.Length)
            selectedTab = 0;
        if (sel < 0)
            selectedTab = (byte)(pauseMenuTabs.Length - 1);


        pauseMenuTabs[selectedTab].SetActive(true);
    }
}
