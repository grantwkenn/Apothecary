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

    Transform arrow;
    Vector3 arrowRefPos;
    float arrowRefY;

    byte[] arrowAnimationHeights;
    bool animateArrow = false;
    byte animationTimer = 0;
    byte frameTime = 4;
    byte arrowAnimationIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        dialogueContainer.SetActive(false);

        arrow = dialogueContainer.transform.Find("Arrow");

        arrowRefPos = arrow.transform.localPosition;
        
        arrowAnimationHeights = new byte[8] { 0,1,2,4,2,1,0,0 };
        

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void FixedUpdate()
    {
        if (animateArrow)
        {
            arrow.GetComponent<Image>().enabled = true;
            if (animationTimer == frameTime)
            {
                arrowAnimationIndex++;
                arrow.transform.localPosition = new Vector3(arrowRefPos.x, arrowRefPos.y - arrowAnimationHeights[arrowAnimationIndex], arrowRefPos.z);
                animationTimer = 0;

                if (arrowAnimationIndex == arrowAnimationHeights.Length - 1)
                    arrowAnimationIndex = 0;
            }
            else
                animationTimer++;

        }
        else
        {
            arrow.GetComponent<Image>().enabled = false;
        }

    }

    public void displayMessage()
    {

        //if past all pages of text
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

            if (parsedMessage.Count > 1)
            {
                animateArrow = true;
            } 

            //last page of message
            if (messageIndex == parsedMessage.Count)
            {
                animateArrow = false;
            }
                
        }

        animationTimer = 0;
        arrowAnimationIndex = 0;
        
    }




    public void loadMessage(List<string> parsedMsg)
    {
        parsedMessage = parsedMsg;
        
        messageTrigger = true;
        messageIndex = 0;

        if (parsedMessage.Count > 1)
        {
            arrow.GetComponent<Image>().enabled = true;
            animateArrow = true;
        }
            
        else
        {
            arrow.GetComponent<Image>().enabled = false;
            animateArrow = false;
        }
            
    }

    public void clearMessage()
    {
        dialogueContainer.SetActive(false);
        messageTrigger = false;
        messageIndex = 0;
        parsedMessage = null;

        animateArrow = false;
    }

    public bool getMessageTrigger() { return messageTrigger; }
}
