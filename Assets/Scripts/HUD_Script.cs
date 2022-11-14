using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Script : MonoBehaviour
{
    Sprite[] healthImages;
    Sprite[] manaImages;

    Image healthImage;
    Image manaImage;

    GameObject player;

    Sprite[] healthManaSprites;

    
    // Start is called before the first frame update
    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");


        // load health and mana images

        healthImages = new Sprite[11];
        manaImages = new Sprite[11];

        healthImage = this.transform.Find("Health").GetComponent<Image>();
        manaImage = this.transform.Find("Mana").GetComponent<Image>();

        //healthImages[0] = Resources.Load("Health and Mana Bars") as Sprite;

        healthManaSprites = Resources.LoadAll<Sprite>("Health and Mana Bars");
        

    }

    // Update is called once per frame
    void Update()
    {
        //get health from GM
        int health = player.GetComponent<Player>().getHealth();
        int mana = player.GetComponent<Player>().getMana();

        //set image sprites
        //access pattern due to sprite sheet staggered layout
        healthImage.sprite = healthManaSprites[1+(health*2)]; 
        manaImage.sprite = healthManaSprites[mana * 2];
    }
}
