using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Message List", menuName = "Messages/Message List")]
public class Message_List : ScriptableObject
{
    public List<Message> messages;
}
