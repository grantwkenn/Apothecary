using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;


public class Messager : MonoBehaviour
{
    //
    public int messagerID;
    
    GameObject gameManager;
    Text_Manager textManager;
    Dialogue_Manager dialogueManager;

    [SerializeField]
    private Message message;

    int numSegments;
    [SerializeField]
    int segmentIndex;

    [SerializeField]
    SpriteRenderer quest_indicator;


    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        textManager = gameManager.GetComponent<Text_Manager>();
        dialogueManager = gameManager.GetComponent<Dialogue_Manager>();
    }

    private void OnEnable()
    {
        
        //not all messagers have NPC. but we need a way to 
        //ID all messagers by messager ID
        // current solution is to manually ID all signs with ID not overlapping any NPCs
        //will automate at some time with Scr Objects / Database etc.
        NPC npc = this.GetComponentInParent<NPC>();
        if (npc != null) messagerID = npc.getNPCID();

        dialogueManager.messagerEnable(this);
    }

    private void OnDisable()
    {
        dialogueManager.messagerDisable(this);
    }

    // Start is called before the first frame update
    void Start()
    {


        //after dialogueManager and Quest Giver
        if (message == null)
        {
            nextMessage();
        }
        else
            setMessage(message);

    }

    //maybe only load message once player has interacted,

    private void OnTriggerEnter2D(Collider2D collider)
    {       
        if (collider.CompareTag("Player"))
            dialogueManager.setCurrentMessager(this);

    }



    private void OnTriggerExit2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            dialogueManager.clearMessager();
            segmentIndex = 0;
        }
    }

    /*public void setMessage(Message msg)
    {
        this.message = msg;
        segmentIndex = 0;
        numSegments = message.messageSegments.Length;
    }*/

    public String nextSegment()
    {
        int tempIndex = segmentIndex;

        if (segmentIndex >= numSegments) return "MESSAGE INDEX ERROR" + "index:" + segmentIndex + "num:" + numSegments;

        segmentIndex++;
        return message.messageSegments[tempIndex];

    }

    public bool hasNextSegment()
    {
        return segmentIndex < numSegments;
    }


   void setMessage(Message msg)
    {
        //workaround for now due to DM limitation
        if (msg == null)
        {
            this.message = null;
            numSegments = 0;
        }
        else
        {
            this.message = msg;
            numSegments = message.messageSegments.Length;
        }
                    
        segmentIndex = 0;
    }

    public Message getMessage()
    {
        return this.message;
    }

    public void resetMessageIndex() { segmentIndex = 0; }

    public int getID()
    {
        return messagerID;
    }

    public bool isQuestGiver()
    {
        return this.GetComponentInParent<Quest_Giver>() != null;
    }

    public void setSymbol(int index)
    {
        if(index < 0)
        {
            quest_indicator.enabled = false;
        }
        else
        {
            quest_indicator.enabled = true;
            quest_indicator.sprite = dialogueManager.getQuestSymbol(index);
            
        }
    }

    public void nextMessage()
    {
        this.message = dialogueManager.nextMessage(this);
        if(this.message == null)
        {
            numSegments = 0;
        }
        else
        {
            numSegments = message.messageSegments.Length;
        }
        segmentIndex = 0;
    }
    
}
