using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_Log : Menu
{
    Quest_Manager qm;

    public List<Quest> quests;

    public Transform slotList;
    public Transform[] slots;

    public Transform selection;
    Transform coinImage;

    Text coinRewardContainer;

    int currentSelection = 0;

    byte questsPerPage = 8;

    Color gray;

    Transform counter;

    Transform description;

    Transform objectiveContainer;



    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        qm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Quest_Manager>();
        slotList = this.transform.Find("Slot List");
        currentSelection = 0;
        objectiveContainer = this.transform.Find("Quest Panel").transform.Find("Objectives");
        description = this.transform.Find("Quest Panel").transform.Find("Quest Description");
        coinRewardContainer = this.transform.Find("Quest Panel").transform.Find("Coin Reward").GetComponent<Text>();
        coinImage = this.transform.Find("Quest Panel").transform.Find("Coin Image");
        counter = this.transform.Find("Counter");


        gray = new Color(0f, 0f, 0f, 0.3f);

        selection = slotList.Find("Selection");
        counter = this.transform.Find("Counter");



        slots = new RectTransform[questsPerPage];
        for(int i=0; i< questsPerPage; i++)
        {
            slots[i] = slotList.Find("Slot" + i);
            slots[i].gameObject.SetActive(false);
        }

        this.refresh();
        refreshLog();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void refreshLog()
    {


    }

    public override void handleInput(direction urdl)
    {
        if (quests == null || quests.Count == 0) return;

        if (urdl == direction.up)
        {
            currentSelection -= 1;
            if (currentSelection < 0) currentSelection = quests.Count - 1;
        }
        if (urdl == direction.down)
        {
            currentSelection += 1;
            if (currentSelection >= quests.Count) currentSelection = 0;
        }

        this.refresh();

    }

    public override void refresh()
    {
        //get all quests in log
        quests = qm.getLog();
        description.GetComponent<Text>().text = "";
        objectiveContainer.GetComponent<Text>().text = "";
        


        if (quests.Count == 0)
        {
            selection.gameObject.SetActive(false);

            for (int i = 1; i < slots.Length; i++)
            {
                slots[i].gameObject.SetActive(false);

            }

            slots[0].gameObject.SetActive(true);
            slots[0].GetComponent<Text>().text = "Quest Log is Empty";
            counter.GetComponent<Text>().text = "0/0";
            coinImage.gameObject.SetActive(false);
            coinRewardContainer.gameObject.SetActive(false);
        }

        else
        {
            selection.gameObject.SetActive(true);           
            
            for (int i = 0; i < quests.Count; i++)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].GetComponent<Text>().text = quests[i].getData().getTitle();
                slots[i].GetComponent<Text>().color = gray;

            }

            counter.GetComponent<Text>().text = (currentSelection + 1).ToString() + "/" + quests.Count;

            slots[currentSelection].GetComponent<Text>().color = Color.black;

            selection.transform.position = new Vector3(selection.position.x, slots[currentSelection].position.y, 0);

            description.GetComponent<Text>().text = quests[currentSelection].getData().getDescription();
            
            objectiveContainer.GetComponent<Text>().text = quests[currentSelection].getObjectiveStatus();

            int coinReward = quests[currentSelection].getData().getCoinReward();
            if(coinReward > 0)
            {
                coinRewardContainer.gameObject.SetActive(true);
                coinRewardContainer.text = "Reward: " + coinReward;
                coinImage.gameObject.SetActive(true);
            }
            else
            {
                coinImage.gameObject.SetActive(false);
                coinRewardContainer.gameObject.SetActive(false);
            }
                

        }

        


    }
}
