using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LayerManager : MonoBehaviour
{
    public bool run;

    Transform[] levels;
    List<Transform> levelsList;

    string[] levelNames;

    
    // Start is called before the first frame update
    void Start()
    {
        levelNames = new string[6];
        levelNames[0] = "Level 0";
        levelNames[1] = "Level 1";
        levelNames[2] = "Level 2";
        levelNames[3] = "Level 3";
        levelNames[4] = "Level 4";
        levelNames[5] = "Level 5";

        levels = new Transform[6];

        for(int i=0; i<levels.Length; i++)
        {
            if (GameObject.Find(levelNames[i]) != null)
            {
                levels[i] = GameObject.Find(levelNames[i]).transform;
            }
                
        }
    }




    void relayer()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (GameObject.Find(levelNames[i]) != null)
            {
                levels[i] = GameObject.Find(levelNames[i]).transform;
            }

        }

        for (int lvlNo = 0; lvlNo < 6; lvlNo++)
        {
            if (levels[lvlNo] == null) continue;
            Transform level = levels[lvlNo];
            string objectName = "" + lvlNo + " Object";
            string groundName = "" + lvlNo + " Ground";
            string overheadName = "" + lvlNo + " Above";
            Transform Object = level.Find(objectName);
            Transform Ground = level.Find(groundName);
            Transform Overhead = level.Find(overheadName);

            if(Object != null)
                layerRecursive(Object, objectName);
            // layerRecursive(Ground, groundName);
            if(Overhead != null)
                layerRecursive(Overhead, overheadName);

        }


    }

    void layerRecursive(Transform t, string layerName)
    {
        
        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
        if(sr != null)
        {
            lyHelper lyHelp = sr.GetComponent<lyHelper>();
            Layered ly = sr.GetComponent<Layered>();

            //update the sorting level name unless it has edge case flag
            //// For example a canopy over a doorway is nested inside the house but needs
            ///to be sorted on a different layer from its parent
            if(lyHelp == null)
            {
                sr.sortingLayerName = layerName; 
            }
            else if(!lyHelp.edgeCase)
            {
                sr.sortingLayerName = layerName;
            }////////////////////////////////////////////////

            //TODO For now don't affect objects that use the older script
            if(ly == null)
            {
                //if no layerhelper, use a standard layering from sprite's bottom
                if(lyHelp != null)
                {
                    lyHelp.enabled = false;
                    lyHelp.enabled = true;
                }
                else
                    lyHelper.layerManHelper(sr);
            }


        }

        //find all children
        int numChildren = t.childCount;
        for(int child = 0; child < numChildren; child++)
        {
            layerRecursive(t.GetChild(child), layerName);
        }
    }



    void relayerAllSprites()
    {
        SpriteRenderer[] srs = GameObject.FindObjectsOfType<SpriteRenderer>();

        foreach(SpriteRenderer sr in srs)
        {
            //do not affect sprites which use the dynamnic layering script
            Layered layered = sr.GetComponent<Layered>();
            if (layered != null) continue;

            lyHelper lyhelper = sr.GetComponent<lyHelper>();
            if(lyhelper == null)
            {
                int order = 4096 - (int)(sr.transform.position.z * 16);
                sr.sortingOrder = order;
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            relayer();
            run = false;
        }
        
    }
}
