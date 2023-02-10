using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Manager : MonoBehaviour
{

    public GameObject storeMenu;
    
    Transform canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("HUD").transform;
        //GameObject clone = Instantiate(storeMenu, canvas);
        //clone.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
