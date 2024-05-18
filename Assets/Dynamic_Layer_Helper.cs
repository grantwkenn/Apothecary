using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamic_Layer_Helper : MonoBehaviour, ICustomLayer
{
    SpriteRenderer sr;

    private void OnEnable()
    {
        this.sr = this.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.y);
        sr.sortingOrder = 4096 - (int)(transform.position.z * 16);
    }

    public void layer(string sortingLayerName)
    {
        this.sr.sortingLayerName = sortingLayerName;
    }


}
