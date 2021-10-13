using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public int startingRoomNum;
    int currentRoomNum;
    
    GameObject[] allRooms;

    Camera mainCam;


    GameObject player;

    public RectTransform fadeOut;
    public Image fadeImage;

    float fadeSeconds = 0.3f;
    int fadeSteps = 30;


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

        fadeImage = fadeOut.GetComponent<Image>();

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

        /*
        foreach (GameObject room in allRooms)
        {
            //get all the switches
            GameObject[] switches = GameObject.FindGameObjectsWithTag("Room Trigger");
            foreach (GameObject sw in switches)
            {
                foreach (GameObject sw2 in switches)
                {
                    if (sw2.GetComponent<RoomSwitch>().Destination == room.GetComponent<RoomScript>().RoomNumber)
                    {
                        sw.GetComponent<RoomSwitch>().to = sw2.transform.position;
                    }
                }
            }



        }*/


        //Find the current Room Player is in
        foreach (GameObject rm in allRooms)
        {
            rm.SetActive(false);

            Vector2 minPos = rm.GetComponent<RoomScript>().getMinPos();
            Vector2 maxPos = rm.GetComponent<RoomScript>().getMaxPos();
            //if within x and within y bounds
            if (playerLocation.position.x <= maxPos.x
                && playerLocation.position.x >= minPos.x
                && playerLocation.position.y <= maxPos.y
                && playerLocation.position.y >= minPos.y)
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

    public void SwitchRoom(int roomTo, Vector3 moveTo, Vector2 directionFacing)
    {
        //TODO Freeze all players / Physics / Pause Game??

        //freeze player
        player.GetComponent<Player>().freeze();
        
        StartCoroutine(FadeOut(roomTo, moveTo, directionFacing));

    }

    IEnumerator FadeOut(int destination, Vector3 moveTo, Vector2 direction)
    {
        fadeImage.enabled = true;
        
        while(fadeImage.color.a < 1)
        {
            fadeImage.color = new Color(0, 0, 0, fadeImage.color.a + (1f/fadeSteps));
            yield return new WaitForSeconds(fadeSeconds / fadeSteps);
            //yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
        movePlayer(destination, moveTo, direction);

    }

    private void movePlayer(int destination, Vector3 moveTo, Vector2 direction)
    {
        //move Player
        player.transform.position = moveTo;


        //inactivate Previous room, activate destination
        allRooms[currentRoomNum].SetActive(false);
        currentRoomNum = destination;
        allRooms[currentRoomNum].SetActive(true);

        //set player's facing direction
        player.GetComponent<Player>().setDirectionFacing(direction);


        //TODO camera pans in direction of doorway     
        //move camera bounds
        mainCam.GetComponent<CameraMovement>().setBounds(allRooms[destination].GetComponent<RoomScript>().getMinPos(), allRooms[destination].GetComponent<RoomScript>().getMaxPos());
        moveTo.z = mainCam.transform.position.z;
        mainCam.transform.position = moveTo;


        StartCoroutine(FadeIn());

    }

    IEnumerator FadeIn()
    {

        //Fade back in

        //wait for Fade to Black to Complete
        while (fadeImage.color.a > 0)
        {
            fadeImage.color = new Color(0, 0, 0, fadeImage.color.a - (1f/ fadeSteps));
            yield return new WaitForSeconds(fadeSeconds / fadeSteps);
            //yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.enabled = false;
        player.GetComponent<Player>().unfreeze();

    }


    public GameObject getCurrentRoom()
    {
        return allRooms[currentRoomNum];
    }





}
