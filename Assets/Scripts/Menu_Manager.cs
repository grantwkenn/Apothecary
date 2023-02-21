using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Manager : MonoBehaviour
{
    byte selectedTab;

    GameObject[] MenuTabs;
    
    Input_Manager im;
    Inventory_Manager invMan;

    [SerializeField]
    GameObject pauseMenu;

    GameObject invMenu, questLog;

    
    [SerializeField]
    GameObject storeMenu;

    public GameObject[] storeMenus;

    public GameObject currentMenu;
    
    Transform canvas;

    private void OnEnable()
    {
        im = this.GetComponent<Input_Manager>();
        invMan = this.GetComponent<Inventory_Manager>();

        MenuTabs = new GameObject[2];
        invMenu = pauseMenu.transform.Find("Inventory Menu").gameObject;
        questLog = pauseMenu.transform.Find("Quest Log").gameObject;

        MenuTabs[0] = invMenu;
        MenuTabs[1] = questLog;

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
        currentMenu = Instantiate(storeMenus[shopId], canvas);
        currentMenu.SetActive(true);

        //change input schema

        im.enableMenuInput();
        Time.timeScale = 0;
    }

    public void closeMenu()
    {
        Time.timeScale = 1;
        currentMenu.SetActive(false);
        Object.Destroy(currentMenu);
        currentMenu = null;

        im.enableGameplay();
    }


    public void handleInput(direction urdl)
    {

    }

    public void pauseGame()
    {
        pauseMenu.SetActive(true);
        MenuTabs[selectedTab].SetActive(true);
    }

    public void resumeGame()
    {
        pauseMenu.SetActive(false);
        MenuTabs[selectedTab].SetActive(false);
    }

    public void incrementTab(sbyte value)
    {
        
        MenuTabs[selectedTab].SetActive(false);
        
        int sel = selectedTab + value;
        selectedTab = (byte)(selectedTab + value);

        if (sel >= MenuTabs.Length)
            selectedTab = 0;
        if (sel < 0)
            selectedTab = (byte)(MenuTabs.Length - 1);


        MenuTabs[selectedTab].SetActive(true);
    }
}
