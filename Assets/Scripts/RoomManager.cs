using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public int startingRoomNum;
    int currentRoomNum;
    
    GameObject[] allRooms;

    Camera mainCam;

    public Animator animator;

    GameObject player;

    struct CameraBoundary
    {
        public Vector2 minPos;
        public Vector2 maxPos;
    }


    // Room Points
    CameraBoundary[] cameraBounds;

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        animator = GetComponentInChildren<Animator>();  

        currentRoomNum = startingRoomNum;
        
        //get all the rooms unsorted
        GameObject[] roomsUnsorted = GameObject.FindGameObjectsWithTag("Room");

        // empty array of rooms
        allRooms = new GameObject[roomsUnsorted.Length];


        //allRooms sorted by room number
        foreach(GameObject room in roomsUnsorted)
        {
            int number = room.GetComponent<RoomScript>().RoomNumber;
            allRooms[number] = room;
        }
        

        //cameraBounds = new CameraBoundary[numRooms];

        /*
        CameraBoundary room1;
        room1.maxPos = new Vector2(86, 20);
        room1.minPos = new Vector2(22, -28);

        CameraBoundary room0;
        room0.maxPos = new Vector2(22, 20);
        room0.minPos = new Vector2(-44, -15);

        cameraBounds[0] = room0;
        cameraBounds[1] = room1;
        */

        //
        //mainCam = GameObject.FindWithTag("MainCamera");

        mainCam = Camera.main;


        //get player current room
        //for all rooms, match location of player with boundaries?

        //deactivate other rooms
        /*
        for(int i=0; i< allRooms.Length; i++)
        {
            if (i != startingRoomNum)
            {
                //deactivate this room
                allRooms[i].SetActive(false);
            }
        }
        */

        Transform playerLocation = player.transform;


        foreach (GameObject rm in allRooms)
        {
            rm.SetActive(false);

            //if within x and within y bounds
            if (playerLocation.position.x <= rm.GetComponent<RoomScript>().maxPos.x
                && playerLocation.position.x >= rm.GetComponent<RoomScript>().minPos.x
                && playerLocation.position.y <= rm.GetComponent<RoomScript>().maxPos.y
                && playerLocation.position.y >= rm.GetComponent<RoomScript>().minPos.y)
            {
                currentRoomNum = rm.GetComponent<RoomScript>().RoomNumber;
                allRooms[currentRoomNum].SetActive(true);
            }
        }



        

        //SwitchRoom(startingRoomNum);

    }



    // Update is called once per frame
    void Update()
    {
       
    }


    /* TODO:
    Changing rooms fades the screen, inactivates previous room and activates next room;

    Room Manager can access (via Game Manager?) the room collection of objects (tiles, enemies, etc) to set activation

    */

    public void SwitchRoom(int destination, Vector3 moveTo, int direction)
    {
        //TODO Freeze all players / Physics?

        //freeze player
        player.GetComponent<Player>().freeze();
        

        //Fade Screen to Black
        animator.SetTrigger("Fade Out");

        StartCoroutine(Fade(destination, moveTo, direction));

    }

    IEnumerator Fade(int destination, Vector3 moveTo, int direction)
    {
        
        //wait for Fade to Black to Complete
        while (!animator.GetBool("Black"))
            yield return null;


        //move Player
        player.transform.position = moveTo;

        //inactivate Previous room, activate destination
        allRooms[currentRoomNum].SetActive(false);
        currentRoomNum = destination;
        allRooms[destination].SetActive(true);
        

        animator.ResetTrigger("Fade Out");
        animator.SetTrigger("Fade In");
        animator.SetBool("Black", false);

        //set player's facing direction
        player.GetComponent<Player>().setIdleAnimation(direction);

        player.GetComponent<Player>().unfreeze();

        //TODO camera pans in direction of doorway     
        //move camera bounds
        mainCam.GetComponent<CameraMovement>().setBounds(allRooms[destination].GetComponent<RoomScript>().minPos, allRooms[destination].GetComponent<RoomScript>().maxPos);
        moveTo.z = mainCam.transform.position.z;
        mainCam.transform.position = moveTo;
    }
    

    public GameObject getCurrentRoom()
    {
        /*
        
        Transform playerLocation = GameObject.FindWithTag("Player").transform;


        foreach(GameObject rm in allRooms)
        {
            //if within x and within y bounds
            if (playerLocation.position.x <= rm.GetComponent<RoomScript>().maxPos.x
                && playerLocation.position.x >= rm.GetComponent<RoomScript>().minPos.x
                && playerLocation.position.y <= rm.GetComponent<RoomScript>().maxPos.y
                && playerLocation.position.y >= rm.GetComponent<RoomScript>().minPos.y)
            {
                currentRoomNum = rm.GetComponent<RoomScript>().RoomNumber;
                allRooms[currentRoomNum].SetActive(true);
            }
        }

        */

        return allRooms[currentRoomNum];
    }





}
