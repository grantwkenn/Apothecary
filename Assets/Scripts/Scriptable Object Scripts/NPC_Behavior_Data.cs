using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC_Behav_Data", menuName = "NPC_Behav_Data")]
public class NPC_Behavior_Data : ScriptableObject
{
    enum pathMode { cyclic, reverse, once }

    [SerializeField]
    Vector2[] targets;
    [SerializeField]
    int pathIndex;

    bool reverse;

    pathMode mode;

    public Vector2 getNextTarget()
    {
        Vector2 result = targets[pathIndex];

        if(mode == pathMode.cyclic)
        {
            pathIndex = (pathIndex + 1) % targets.Length;
        }
        else if(mode == pathMode.reverse)
        {
            if(reverse)
            {
                if (pathIndex == 0)
                {
                    reverse = false;
                    pathIndex = 1;
                }
                else pathIndex--;
            }

            else
            {
                if (pathIndex == targets.Length - 1)
                {
                    reverse = true;
                    pathIndex--;
                }
                else pathIndex++;
            }
        }
        
        return result;
    }

    public void init()
    {
        pathIndex = 0;
        mode = pathMode.reverse;
        reverse = false;
    }

    public void trainTargets(Vector2[] targets) { this.targets = targets; }

    public Vector2[] trainerGetTargets() { return this.targets; }

}
