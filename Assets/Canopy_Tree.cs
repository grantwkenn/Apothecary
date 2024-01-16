using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Canopy_Tree : MonoBehaviour, ICustomLayer
{
    SpriteRenderer canopy;
    SpriteRenderer trunk;

    [SerializeField]
    Color color;

    
    // Start is called before the first frame update
    void Start()
    {
        trunk = this.transform.Find("trunk").GetComponent<SpriteRenderer>();
        canopy = this.transform.Find("canopy").GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        trunk = this.transform.Find("trunk").GetComponent<SpriteRenderer>();
        canopy = this.transform.Find("canopy").GetComponent<SpriteRenderer>();

        if (canopy.color != Color.white) color = canopy.color;

        if (color == null || color == new Color(0, 0, 0, 0)) color = Color.white;

        trunk.color = color;
        canopy.color = color;
    }

    void Awake()
    {
        trunk = this.transform.Find("trunk").GetComponent<SpriteRenderer>();
        canopy = this.transform.Find("canopy").GetComponent<SpriteRenderer>();
    }

    private void OnValidate()
    {
        trunk = this.transform.Find("trunk").GetComponent<SpriteRenderer>();
        canopy = this.transform.Find("canopy").GetComponent<SpriteRenderer>();
        canopy.color = color;
        trunk.color = color;
    }

    public void layer(string layerOrderName)
    {
        //find the parent sorting name
        trunk.sortingLayerName = layerOrderName;

        string[] split = layerOrderName.Split(' ');
        int layer = System.Int32.Parse(split[0]);
        string newLayer = "" + layer + " Above";
        canopy.sortingLayerName = newLayer;
        canopy.sortingOrder = trunk.sortingOrder;

        canopy.transform.localPosition = new Vector3(canopy.transform.localPosition.x, canopy.transform.localPosition.y, -1);

    }

    public void setColor(Color color) { this.color = color; canopy.color = color;
        trunk.color = color;
    }
}
