using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableVector3Int
{
    public int x;
    public int y;
    public int z;

    public SerializableVector3Int(Vector3Int vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3Int ToVector3()
    {
        return new Vector3Int(x, y, z);
    }
}



