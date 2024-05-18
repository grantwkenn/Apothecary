using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue_Manager : MonoBehaviour
{

    //Keeps track of dialogue progression,
    //manipulates each NPC messager to load correct message in the progression

    //Reference to all (active) NPCs

    Inventory_Manager inventoryManager;
    Quest_Manager questManager;
    Text_Manager textManager;

    List<Item> itemsToReceive;

    [SerializeField]
    Messager currentMessager;

    Dictionary<int, Messager> activeMessagersByNPCID;


    private void Awake()
    {
        activeMessagersByNPCID = new Dictionary<int, Messager>();
    }

    private void OnEnable()
    {
        //When is the best time to do these?
        
        inventoryManager = GetComponentInParent<Inventory_Manager>();

        questManager = GetComponentInParent<Quest_Manager>();

        textManager = GetComponentInParent<Text_Manager>();


    }

    private void Start()
    {

    }


    public void displayText()
    {

        if (!currentMessager.hasNextSegment())
        {
            textManager.deactivateTextBox();
            messageTerminated();
        }
        
        else
        {
            textManager.activateTextBox();
            textManager.setText(currentMessager.nextSegment());
            textManager.setArrow(currentMessager.hasNextSegment());
        }
    }

    public void messagerRefresh(Messager messager)
    {
        messager.nextMessage();
    }

    public void allMessagerRefresh()
    {
        //force all messagers active to re-evaluate current message
        foreach(Messager messager in activeMessagersByNPCID.Values)
        {
            messager.nextMessage();
        }
    }

    public void messagerEnable(Messager messager)
    {
        //if (activeMessagersByNPCID == null) Debug.Log("BAD");
        
        if (!activeMessagersByNPCID.ContainsKey(messager.getID()))
            activeMessagersByNPCID.Add(messager.getID(), messager);
    }

    public void messagerDisable(Messager messager)
    {
        if (activeMessagersByNPCID.ContainsKey(messager.getID()))
            activeMessagersByNPCID.Remove(messager.getID());
    }

    public void messageTerminated()
    {        
        questManager.checkDialogueProgression(currentMessager);

        inventoryManager.offerItems(currentMessager.getMessage().itemsToDeliver);
        inventoryManager.addCoins(currentMessager.getMessage().giveCoins);

        //TODO implement a way to charge coins after dialogue, this requires a "not enough money" dialogue

        //Quest_Giver QG = currentMessager.GetComponentInParent<Quest_Giver>();
        //if (QG != null && QG.getState() == QuestGiverState.available)
        //{
        //    if (itemsWithheld)
        //        QG.setState(QuestGiverState.availableFull);
        //    else
        //    {
        //        //try to accept this quest

        //        questManager.accept(QG);


        //        QG.setState(QuestGiverState.active);
        //    }


        //}

        //not needed because setMessage does this
        //currentMessager.resetMessageIndex();

        //currentMessager.setMessage(nextMessage(currentMessager));
        currentMessager.nextMessage();

        //check for and prompt comeback message such as full inventory or quest log full
        if(currentMessager.isQuestGiver())
        {
            QuestGiverState state = currentMessager.GetComponentInParent<Quest_Giver>().getState();
            if(state == QuestGiverState.availableFull || state == QuestGiverState.turnInFull)
            {
                //trigger an interrupting message!
                if (AwaitingInput()) //may be redundent check
                    displayText();
                return;
            }
        }
        ///////////////////////////////////////////////////////////

        //if more messages in this list, increment

        //follow message flag if present

        // if end of list (quest / progression completed)
        //determine next quest available / progression...


        //check to open up a menu (shop, trade, etc)
        NPC thisNPC = currentMessager.GetComponent<NPC>();
        if (thisNPC != null && thisNPC.isOpen())
        {
            //Open the Shop Menu
            this.GetComponent<Menu_Manager>().loadShopMenu(thisNPC.getShopID());
        }

    }


    public Message nextMessage(Messager messager)
    {
        
        
        
        
        //get Quest Message if aplicaple from this messager
        Message nextMessage = questManager.getQuestMessage(messager);



        if (nextMessage != null) return nextMessage;
        
        //no quest content, get other message



        //TODO return its non-quest giver dialogue

        return null;

    }

    public void setCurrentMessager(Messager messager)
    {
        //try to set messager but don't overwrite existing
        //for example if another NPC walks into my path
        if (currentMessager == null)
            currentMessager = messager;

    }

    public void clearMessager()
    {
        currentMessager = null;
        textManager.deactivateTextBox();
    }

    public bool AwaitingInput()
    {
        return currentMessager != null 
            && Player.Instance.isFacing(currentMessager.transform);
    }


    public static bool compareMessages( Message m1, Message m2 )
    {
        return m1.messageSegments == m2.messageSegments;
    }


}
