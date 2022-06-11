using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialogue Data", menuName = "Messages/Dialogue Data")]
public class NewBehaviourScript : ScriptableObject
{
    public Message message;
    public Item[] itemOfferings;
}
