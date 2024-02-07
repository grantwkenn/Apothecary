using UnityEngine;

[System.Serializable]
public class Byte3
{
    [SerializeField]
    public byte x;
    [SerializeField]
    public byte y;
    [SerializeField]
    public byte z;

    public Byte3() { this.x = this.y = this.z = 0; }

    public Byte3(Vector3Int v3int) { this.x = (byte)v3int.x; this.y = (byte)v3int.y; this.z = (byte)v3int.z; }
}
