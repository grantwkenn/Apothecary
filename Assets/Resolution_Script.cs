using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Resolution_Script : MonoBehaviour
{

    TMP_Dropdown dd_resolutions;
    TMP_Dropdown dd_scaling;
    CameraManager cm;

    private void OnEnable()
    {
        cm = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>();
        dd_resolutions = this.GetComponent<TMP_Dropdown>();
        dd_scaling = this.GetComponent<TMP_Dropdown>();
    }

    void Start()
    {

    }

    public void setResolution()
    {
        cm.setResolution(dd_resolutions.options[dd_resolutions.value].text);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
