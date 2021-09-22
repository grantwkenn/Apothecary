using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency_Script : MonoBehaviour
{
    bool behind = false;

    float alpha = 1;

    float smoothing = 0.008f;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(behind)
        {
            if (alpha > 0.6f)
                //
                alpha -= smoothing;
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        }
        else if (alpha < 1)
        {
            alpha += 0.005f;
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        }


    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            behind = true;
        }
    }

    private void OnTriggerExit2D(Collider2D player)
    {
        if (player.gameObject.CompareTag("Player"))
        {
            behind = false;
        }
    }
}
