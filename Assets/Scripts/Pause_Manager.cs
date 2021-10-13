using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// /TODO
/// BUG: pausing and unpausing during a room transition can regain control of the player.
///



public class Pause_Manager : MonoBehaviour
{
    [SerializeField]
    GameObject Menu;


    Player player;
    
    bool isPaused;
    bool beenPaused;

    private void Awake()
    {
        isPaused = false;
        beenPaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        Menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused && !beenPaused)
        {
            Time.timeScale = 0f;

            beenPaused = true;
            Menu.SetActive(true);
        }
        else if (!isPaused && beenPaused)
        {
            Time.timeScale = 1;
            beenPaused = false;
            Menu.SetActive(false);
        }

    }

    public void togglePause()
    {
        if(isPaused) //if already paused, resume
        {
            isPaused = false;
            //beenPaused = false;
        }
        else //pause
        {
            isPaused = true;
        }
    }

    public bool checkPaused() { return isPaused; }

}
