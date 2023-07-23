using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "NPC_Data")]
public class NPC_Data : ScriptableObject
{
    enum pathMode { cyclic, reverse, oneway }

    [SerializeField]
    Vector2[] destinations;
    [SerializeField]
    byte wait_URDL;
    [SerializeField]
    int pathIndex;

    bool reverse;

    pathMode mode;

    public Vector2 getNextDestination()
    {
        if (destinations.Length == 0) return Vector2.zero;
        
        Vector2 result = destinations[pathIndex];

        if(mode == pathMode.cyclic)
        {
            pathIndex = (pathIndex + 1) % destinations.Length;
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
                if (pathIndex == destinations.Length - 1)
                {
                    reverse = true;
                    pathIndex--;
                }
                else pathIndex++;
            }
        }
        else if(mode == pathMode.oneway)
        {
            if (pathIndex + 1 < destinations.Length)
                pathIndex++;
        }
        
        return result;
    }

    public byte getURDL()
    {
        return this.wait_URDL;
    }

    public void init()
    {
        pathIndex = 0;
        mode = pathMode.reverse;
        reverse = false;
    }

    public void trainTargets(Vector2[] targets) { this.destinations = targets; }

    public Vector2[] trainerGetTargets() { return this.destinations; }

}
