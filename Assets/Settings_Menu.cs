using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings_Menu : Menu
{
    TMP_Dropdown reso_dd;
    TMP_Dropdown scaling_dd;
    CameraManager cm;

    private void OnEnable()
    {
        cm = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>();
        reso_dd = this.transform.Find("Resolutions").GetComponent<TMP_Dropdown>();
        scaling_dd = this.transform.Find("Scaling").GetComponent<TMP_Dropdown>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void handleInput(direction urdl)
    {
        this.refresh();
    }

    public override void refresh()
    {

    }

    public void setResolution()
    {
        cm.setResolution(reso_dd.options[reso_dd.value].text);
    }

    public void setScale() { cm.setScale(scaling_dd.options[scaling_dd.value].text); }


}
