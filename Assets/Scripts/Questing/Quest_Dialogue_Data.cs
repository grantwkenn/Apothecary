using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "q_dialogue_data", menuName = "Questing/Quest Dialogue Data")]
public class Quest_Dialogue_Data : ScriptableObject
{
    public Message pitch;
    public Message pitchInvFull;
    public Message questLogFull;
    public Message[] ongoing;
    public Message[] hints;
    public Message turnIn;
    public Message turnInFull;
    public Message turnInComplete;

    private int ongoingIndex = 0;

    public Message getOngoingMessage()
    {
        int temp = ongoingIndex;

        ongoingIndex = Random.Range(0, ongoing.Length);

        if (ongoingIndex == temp)
            ongoingIndex = (temp + 1) % ongoing.Length;
        
        return ongoing[temp];
    }

}
