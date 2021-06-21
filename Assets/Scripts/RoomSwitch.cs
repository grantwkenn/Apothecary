using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSwitch : MonoBehaviour
{
    public Vector3 playerDelta;

    private CameraMovement cam;

    //reference to Game Manager
    private GameObject gameManager;

    public int Destination;

    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.GetComponent<CameraMovement>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //tell the room manager script to switch  to room #X
            gameManager.GetComponent<RoomManager>().SwitchRoom(Destination);

            collision.transform.position += playerDelta;
        }
    }



    //TODO
    /*
     * this script will set the cameras bounds (min and max points)
     * 
     * 
     * 
     */
}
