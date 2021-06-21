using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    int startingRoomNum;
    int currentRoomNum;
    
    GameObject[] allRooms;

    Camera mainCam;

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
           
        currentRoomNum = startingRoomNum;
        
        //get all the rooms unsorted
        GameObject[] roomsUnsorted = GameObject.FindGameObjectsWithTag("Room");
        Debug.Log("roomsUnsorted:" + roomsUnsorted.Length);

        // empty array of rooms
        allRooms = new GameObject[roomsUnsorted.Length];


        //get room number and assign that room to the number in array
        foreach(GameObject room in roomsUnsorted)
        {
            int number = room.GetComponent<RoomScript>().RoomNumber;
            Debug.Log("number:" + number);
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
        for(int i=0; i< allRooms.Length; i++)
        {
            if (i != startingRoomNum)
            {
                //deactivate this room
                allRooms[i].SetActive(false);
            }
        }

        foreach(GameObject rm in allRooms)
        {
            if (rm != allRooms[startingRoomNum])
                rm.SetActive(false);
        }
        allRooms[startingRoomNum].SetActive(true);
        

    }



    // Update is called once per frame
    void Update()
    {
       
    }


    /* TODO:
    Changing rooms fades the screen, inactivates previous room and activates next room;

    Room Manager can access (via Game Manager?) the room collection of objects (tiles, enemies, etc) to set activation

    */

    public void SwitchRoom(int destination)
    {
        mainCam.GetComponent<CameraMovement>().setBounds(allRooms[destination].GetComponent<RoomScript>().minPos, allRooms[destination].GetComponent<RoomScript>().maxPos);
        //mainCam.GetComponent<CameraMovement>().setBounds(cameraBounds[destination].minPos, cameraBounds[destination].maxPos);

        //inactivate Previous room, activate destination
        allRooms[currentRoomNum].SetActive(false);
        currentRoomNum = destination;
        allRooms[destination].SetActive(true);

    }
    

    public GameObject getCurrentRoom()
    {
        return allRooms[currentRoomNum];
    }





}
