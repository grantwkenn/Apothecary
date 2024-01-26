using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Canopy_Tree : MonoBehaviour
{
    SpriteRenderer canopy;
    SpriteRenderer trunk;

    [SerializeField]
    Color color;

    [SerializeField]
    static float sway = 10;
    [SerializeField]
    float swayLimit;
    float offset = 0;
    sbyte direction = 1;

    float staticX;

    int counter = 0;

    int[] swayFrames;
    int swayIndex;


    // Start is called before the first frame update
    void Start()
    {
        trunk = this.transform.Find("trunk").GetComponent<SpriteRenderer>();
        canopy = this.transform.Find("canopy").GetComponent<SpriteRenderer>();

        var rand = new System.Random();
        if (rand.Next(0, 100) < 50)
            direction = -1;

        staticX = this.transform.position.x;

        swayIndex = rand.Next(0, 4);
        counter = rand.Next(0, 20);

        swayFrames = new int[4];
        swayFrames[0] = 0;
        swayFrames[1] = 1;
        swayFrames[2] = 0;
        swayFrames[3] = -1;

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

    void FixedUpdate()
    {
        //updateOffset();

        
    }

    void updateOffset()
    {
        counter++;
        if (counter > 20)
        {
            counter = 0;
            swayIndex++;
            if (swayIndex == swayFrames.Length)
                swayIndex = 0;
            offset = swayFrames[swayIndex] / 32f;
            canopy.transform.position = new Vector3(staticX + offset, canopy.transform.position.y, canopy.transform.position.z);
        }
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

        //todo update order

    }

    public void setColor(Color color) { this.color = color; canopy.color = color;
        trunk.color = color;
    }
}
