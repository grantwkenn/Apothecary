using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{

    [SerializeField]
    Text text;

    Time_Manager timeMan;


    private void OnEnable()
    {
        timeMan = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Time_Manager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //text.text = dayNightCycle.getTimeString();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = timeMan.getTimeString();
    }

}
