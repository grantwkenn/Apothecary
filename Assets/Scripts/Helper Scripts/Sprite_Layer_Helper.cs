using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_Layer_Helper : MonoBehaviour
{
    [SerializeField]
    bool ignoreLayering;

    [SerializeField]
    levelName customLevel;

    [SerializeField]
    float offset;

    [SerializeField]
    bool offsetFromParent;

    enum levelName
    {
        Auto, Object, Above, Ground
    }

    public string getName() { return this.customLevel.ToString(); }

    public float getOffset()
    {
        if (offsetFromParent)
        {
            this.offset = 0.0f - this.transform.localPosition.y;
        }

        return offset;
    }

    public bool isOffsetFromParent() { return offsetFromParent; }

    public bool isIgnored() { return this.ignoreLayering; }
}
