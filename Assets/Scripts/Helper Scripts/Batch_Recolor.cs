using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Batch_Recolor : MonoBehaviour
{
    [SerializeField]
    Color color;

    public bool run = false;
    
    void execute()
    {
        SpriteRenderer[] srs = this.GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer sr in srs)
        {
            Canopy_Tree ct = sr.transform.parent.GetComponent<Canopy_Tree>();
            if (ct != null)
            {
                ct.setColor(color);
            }
            else
                sr.color = color;
        }
    }

    private void OnEnable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            run = false;
            execute();
        }
    }
}
