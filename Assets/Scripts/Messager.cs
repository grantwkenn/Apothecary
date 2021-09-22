using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Messager : MonoBehaviour
{
    GameObject gameManager;
    Text_Manager textManager;
    public Message messageObj;

    List<string> parsedMessage;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        
        textManager = gameManager.GetComponent<Text_Manager>();

        parseMessage();
    }

    //parse the full message string into sub strings delimited by "<br>"
    void parseMessage()
    {
        string fullMsg = messageObj.message;
        int msgLen = fullMsg.Length;

        parsedMessage = new List<string>();

        string partialMessage = "";

        for(int c = 0; c < msgLen; c++)
        {
            if (fullMsg[c] == '<' && fullMsg.Length >= c + 4)//index error handling
            {
                if (fullMsg.Substring(c, 4) == "<br>")
                {
                    parsedMessage.Add(partialMessage);
                    partialMessage = "";
                    c += 3;
                }
                else
                    partialMessage += fullMsg[c];
            }
            else
            {
                partialMessage += fullMsg[c];
            }
        }
        parsedMessage.Add(partialMessage);

    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            textManager.loadMessage(parsedMessage);
        }
    }

    private void OnTriggerExit2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            textManager.clearMessage();
        }
    }
}
