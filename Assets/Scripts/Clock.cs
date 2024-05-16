using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{

    [SerializeField]
    Text text;

    Day_Night_Cycle dayNightCycle;


    private void OnEnable()
    {
        dayNightCycle = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Day_Night_Cycle>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //text.text = dayNightCycle.getTimeString();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = dayNightCycle.getTimeString();
    }

}
