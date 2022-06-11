using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Message", menuName = "Messages/New Message")]
public class Message : ScriptableObject
{
    public string[] messageSegments;

    public Message[] nextMessage;
    public List<Item> itemsToDeliver;
}