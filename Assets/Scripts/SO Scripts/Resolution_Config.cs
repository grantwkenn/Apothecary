using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Resolution_Config : ScriptableObject
{
    [SerializeField]
    string screenModeString;
    [SerializeField]
    string resolutionString;

    public string getScreenModeString() { return this.screenModeString; }
    public string getResolutionString() { return this.resolutionString; }

    public void setScreenModeString(TMPro.TMP_Dropdown dd) { this.screenModeString = dd.options[dd.value].text; }
    public void setResolutionString(TMPro.TMP_Dropdown dd) { 
        
        this.resolutionString = dd.options[dd.value].text; 
    
    }
}

