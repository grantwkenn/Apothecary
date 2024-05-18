using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_Marker : MonoBehaviour
{
    Transform target;
    SpriteRenderer targetSR;
    SpriteRenderer sr;
    float offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + offset, target.transform.position.y);
        this.sr.sortingOrder = targetSR.sortingOrder;
        this.sr.sortingLayerName = targetSR.sortingLayerName;
    }

    public void init(Quest_Giver qg, Sprite symbol)
    {
        this.target = qg.transform;
        this.targetSR = qg.GetComponent<SpriteRenderer>();
        this.sr = this.GetComponent<SpriteRenderer>();
        this.sr.sprite = symbol;
        this.offset = 1.875f;
    }
}
