using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_Manager : MonoBehaviour
{
    public Text textObj;
    public GameObject dialogueContainer;

    List<string> parsedMessage;

    int messageIndex = 0;
    bool messageTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        dialogueContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void displayMessage()
    {
        if (messageIndex >= parsedMessage.Count)
        {
            messageIndex = 0;
            dialogueContainer.SetActive(false);
        }
        else
        {
            textObj.text = parsedMessage[messageIndex];
            messageIndex++;
            dialogueContainer.SetActive(true);
        }  
        
    }


    public void loadMessage(List<string> parsedMsg)
    {
        parsedMessage = parsedMsg;
        
        messageTrigger = true;
        messageIndex = 0;

    }

    public void clearMessage()
    {
        dialogueContainer.SetActive(false);
        messageTrigger = false;
        messageIndex = 0;
        parsedMessage = null;
    }

    public bool getMessageTrigger() { return messageTrigger; }
}
