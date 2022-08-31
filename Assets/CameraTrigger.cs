using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    Transform focus;
    CameraMovement mainCam;
    
    
    // Start is called before the first frame update
    void Start()
    {
        focus = transform.Find("Focus");
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
    }   

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mainCam.setTarget(focus);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        mainCam.resetCameraTarget();
    }
}
