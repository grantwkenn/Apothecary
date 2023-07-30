using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canopy_Tree : MonoBehaviour
{
    SpriteRenderer canopy;
    SpriteRenderer thisSR;

    [ExecuteInEditMode]
    // Start is called before the first frame update
    void Start()
    {
        thisSR = this.GetComponent<SpriteRenderer>();
        canopy = this.transform.Find("canopy").GetComponent<SpriteRenderer>();
    }

    private void OnValidate()
    {
        thisSR = this.GetComponent<SpriteRenderer>();
        canopy = this.transform.Find("canopy").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        canopy.color = thisSR.color;
    }

    public void relayer()
    {
        canopy.color = thisSR.color;
        
        string lyname = thisSR.sortingLayerName;
        string[] split = lyname.Split(' ');
        int layer = System.Int32.Parse(split[0]);
        string newLayer = "" + layer + " Above";
        canopy.sortingLayerName = newLayer;
        canopy.sortingOrder = thisSR.sortingOrder;

        canopy.transform.localPosition = new Vector3(canopy.transform.localPosition.x, canopy.transform.localPosition.y, -1);
    }
}
