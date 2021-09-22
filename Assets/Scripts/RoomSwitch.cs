using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSwitch : MonoBehaviour
{
    private CameraMovement cam;
    private GameObject gameManager;

    int roomTo;
    public Vector2 directionFacing;
    public GameObject DestTrigger;
    private Vector2 moveTo;

    //TODO phase out roomTo and get it directly from the DestTrigger object parent

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.GetComponent<CameraMovement>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");

        //position right in "front" of the target door trigger based on direction facing
        moveTo = new Vector2(DestTrigger.transform.position.x + (directionFacing.x), DestTrigger.transform.position.y + (directionFacing.y/2));
        roomTo = DestTrigger.GetComponentInParent<RoomScript>().RoomNumber;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //tell the room manager script to switch  to room #X

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// TODO
            //adjust destination from room switch by direction to bypass the trigger.
            //change direction to be vector 2 all the way to player setIdleAnimation in Room Manager

            gameManager.GetComponent<RoomManager>().SwitchRoom(roomTo, moveTo, directionFacing);

            //collision.transform.position = moveTo;
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
