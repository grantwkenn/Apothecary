using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "questList", menuName = "Questing/Quest List")]
public class Quest_List : ScriptableObject
{   
    public List<Quest_Data> questList;
}
