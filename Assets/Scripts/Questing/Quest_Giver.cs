using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
 * Quest Giver is only for what an NPC needs on activate.
 * It is not meant to track progress realtime, since progress may happen in another scene where 
 * NPC is inactive.
 * 
 * Quest Giver is for loading dialogue and rigging behavior
 */




public class Quest_Giver : MonoBehaviour
{
    //public int currentQuestID;
    public int QGID;
    
    
    //public Quest activeQuest;

    Quest_Manager qm;

    [SerializeField]
    private QuestGiverState state;


    Messager messager;

    private void Awake()
    {
        //track all Quest givers in QM, hold their state
        state = QuestGiverState.none;
    }

    private void OnEnable()
    {
        qm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Quest_Manager>();
        messager = GetComponentInParent<Messager>();

        //after quest manager
        qm.questGiverEnable(this);

    }

    private void OnDisable()
    {
        qm.questGiverDisable(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        //set state, and QM will reference the current quest
        this.state = qm.evaluateQuestGiverState(this.QGID);
        /////CHECK can this be done in questGiverEnable instead???


        //Based on State, flag ! or ? or ...



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setState(QuestGiverState _state)
    {
        this.state = _state;
    }

    public QuestGiverState getState()
    {
        return state;
    }

    public void nextMessage() { messager.nextMessage(); }


}
