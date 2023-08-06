using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Manager : MonoBehaviour
{
    [SerializeField]
    Resource_Object Resources;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite getSprite(int index)
    {
        return Resources.getSprite(index);
    }

    public GameObject getPrefab(string name) { return Resources.getPrefab(name); }

    public Sprite[] getQuestSymbols()
    {
        return Resources.getQuestSymbols();
    }
}
