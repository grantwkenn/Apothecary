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


    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        qm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Quest_Manager>();
        slotList = this.transform.Find("Slot List");

        slots = new RectTransform[5];
        for(int i=0; i<5; i++)
        {
            slots[i] = slotList.Find("Slot" + i);
            slots[i].gameObject.SetActive(false);
        }



        refreshLog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void refreshLog()
    {
        //get all quests in log
        quests = qm.getLog();

        for(int i =0; i< quests.Count; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].GetComponent<Text>().text = quests[i].getData().getTitle();

        }

    }

    public override void handleInput(direction urdl)
    {
        throw new System.NotImplementedException();
    }
}
