using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entrance : MonoBehaviour
{
    [SerializeField]
    byte entranceNo;

    [SerializeField]
    byte URDL;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public byte getEntranceNo() {  return entranceNo; }

    public byte getURDL() { return URDL; }

}
