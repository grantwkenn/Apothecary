using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Teleport : MonoBehaviour
{
    public string sceneName;
    private Player player;
    Inventory_Manager im;

    [SerializeField]
    Scene_Persistence so;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        im = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Inventory_Manager>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //store health
            so.health = player.getHealth();

            //store inventory
            so.items = im.getItems();
            
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
