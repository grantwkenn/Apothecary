using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Trigger : MonoBehaviour
{
    public string targetSceneName;
    public byte entranceNo;
    private Player player;
    Inventory_Manager im;
    Scene_Manager sm;

    [SerializeField]
    bool runningExit;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
        im = gm.GetComponent<Inventory_Manager>();
        sm = gm.GetComponent<Scene_Manager>();
        
        //TODO: get Scene Persistence Scr. object from the Game Manager instead of dragging for each one.

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sm.triggerSceneChange(targetSceneName, entranceNo, runningExit);
        }
    }
}
