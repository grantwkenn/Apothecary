using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Resolution_Script : MonoBehaviour
{

    TMP_Dropdown dd;
    CameraManager cm;

    private void OnEnable()
    {
        cm = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>();
    }

    void Start()
    {

        //Fetch the Dropdown GameObject
        dd = this.GetComponent<TMP_Dropdown>();
        //Add listener for when the value of the Dropdown changes, to take action
        dd.onValueChanged.AddListener(delegate {
            cm.setResolution(dd);
        });


    }

    public void setResolution()
    {
        cm.setResolution(dd);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
