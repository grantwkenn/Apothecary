using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Layer_Manager : MonoBehaviour
{
    GameObject player;
    Scene_Manager sm;

    List<GameObject> levels;

    string[] sortingLayers;

    string[] levelNames;
    string[] layerNames;

    CompositeCollider2D[] compositeCollidersByLevel; 
    // Start is called before the first frame update
    void Start()
    {
        incrementPlayerLayer(player, 0);

    }

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sm = this.gameObject.GetComponent<Scene_Manager>();
        
        
        levelNames = new string[6];
        levelNames[0] = "Level 0";
        levelNames[1] = "Level 1";
        levelNames[2] = "Level 2";
        levelNames[3] = "Level 3";
        levelNames[4] = "Level 4";
        levelNames[5] = "Level 5";

        levels = new List<GameObject>(GameObject.FindGameObjectsWithTag("Level"));
        levels.Sort((l1, l2) => l1.name.CompareTo(l2.name));



        sortingLayers = new string[SortingLayer.layers.Length];

        foreach (SortingLayer layer in SortingLayer.layers)
        {
            sortingLayers[SortingLayer.GetLayerValueFromID(layer.id)] = layer.name;
        }

        compositeCollidersByLevel = new CompositeCollider2D[levels.Count];

        //get all colliders at all levels not including stair triggers TODO any triggers? should this just be for walls?
        for(int i = 0; i< levels.Count; i++)
        {
            if (GameObject.Find(levelNames[i]) == null) continue;

            levels[i] = GameObject.Find(levelNames[i]);

            //get all colliders at this level
            compositeCollidersByLevel[i] = levels[i].GetComponent<CompositeCollider2D>();

        }

        
    }

    public Transform getLevel(byte level)
    {
        return levels[level].transform;
    }
    

    public List<GameObject> getAllLevels() { return this.levels; }


    public void incrementPlayerLayer(GameObject obj, sbyte increment)
    {

        SpriteRenderer[] srs = obj.GetComponentsInChildren<SpriteRenderer>();

        string newSortingName = sortingLayers[SortingLayer.GetLayerValueFromID(srs[0].sortingLayerID) + increment * 3];

        foreach (SpriteRenderer sr in srs)
        {
            sr.sortingLayerName = newSortingName;
        }

        //for each Level ignore all collisions with this object except for current sortinglayer
        for (int i = 0; i < levels.Count; i++)
        {
            if (compositeCollidersByLevel[i] == null)
            {
                continue;
            }

            bool ignore = true;
            string objectName = "" + i + " Object";

            // check if the player is at this level
            if (newSortingName.CompareTo(objectName) == 0)
                ignore = false;

            //find the sorting name or ID for this level
            Physics2D.IgnoreCollision(compositeCollidersByLevel[i], obj.GetComponent<Collider2D>(), ignore);

        }

    }


    public void relayer()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (GameObject.Find(levelNames[i]) != null)
            {
                levels[i] = GameObject.Find(levelNames[i]);
            }

        }

        foreach(GameObject lvl in levels)
        {
            int lvlNo = levels.IndexOf(lvl);
            Transform level = levels[lvlNo].transform;
            string objectName = "" + lvlNo + " Object";
            string groundName = "" + lvlNo + " Ground";
            string overheadName = "" + lvlNo + " Above";
            string lightsName = "" + lvlNo + "Lights";
            Transform objectLayer = level.Find(objectName);
            Transform groundLayer = level.Find(groundName);
            Transform overheadLayer = level.Find(overheadName);
            Transform lights = level.Find(lightsName);

            SpriteRenderer[] objectObjects = objectLayer.GetComponentsInChildren<SpriteRenderer>();
            SpriteRenderer[] groundObjects = groundLayer.GetComponentsInChildren<SpriteRenderer>();
            SpriteRenderer[] overheadObjects = overheadLayer.GetComponentsInChildren<SpriteRenderer>();

            foreach(SpriteRenderer sr in objectObjects)
            {
                if (sr.transform.parent.GetComponent<ICustomLayer>() != null)
                    continue;
                newRelayer(sr, objectName);
            }

            foreach (SpriteRenderer sr in groundObjects)
            {
                if (sr.transform.parent.GetComponent<ICustomLayer>() != null)
                    continue;
                newRelayer(sr, groundName);
            }

            foreach (SpriteRenderer sr in overheadObjects)
            {
                if (sr.transform.parent.GetComponent<ICustomLayer>() != null)
                    continue;
                newRelayer(sr, overheadName);
            }



            //get all objects that implement layering interface
            ICustomLayer[] customLayerObject = objectLayer.GetComponentsInChildren<ICustomLayer>();
            ICustomLayer[] customLayerGround = groundLayer.GetComponentsInChildren<ICustomLayer>();
            ICustomLayer[] customLayerAbove = overheadLayer.GetComponentsInChildren<ICustomLayer>();

            foreach (ICustomLayer interfaces in customLayerObject)
            {
                interfaces.layer(objectName);
            }

            foreach (ICustomLayer interfaces in customLayerGround)
            {
                interfaces.layer(groundName);
            }

            foreach (ICustomLayer interfaces in customLayerAbove)
            {
                interfaces.layer(overheadName);
            }

            Perfect perfect = level.GetComponent<Perfect>();
            if (perfect == null)
                Debug.Log("Level " + lvlNo + "Needs Perfect Component");
            else
            {
                perfect.run();
            }



        }


    }

    void newRelayer(SpriteRenderer sr, string sortingLayerName)
    {
        lyHelper lyHelp = sr.GetComponent<lyHelper>();
        Layered layered = sr.GetComponent<Layered>();
        if (lyHelp == null && layered == null)
        {
            sr.sortingLayerName = sortingLayerName;
            updateOrder(sr);
        }
        else if(layered != null)
        {
            layered.updateLayer();
        }

    }

    public void relayerMe(SpriteRenderer sr, string sortingLayerName)
    {
        newRelayer(sr, sortingLayerName);
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

    public void updateOrder(SpriteRenderer sr)
    {
        float offset = 0;
        Transform t = sr.transform;

        BoxCollider2D bc = this.GetComponent<BoxCollider2D>();
        if(bc != null)
        {
            offset += bc.offset.y - (bc.size.y / 2.0f);
        }
        
        Vector3 temp = t.position;
        temp.z = temp.y + offset;
        t.position = temp;


        int order = 4096 - (int)(t.position.z * 16);


        sr.sortingOrder = order;

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


    public string getLayerRelative(string layerName, int index)
    {
        for(int i=0; i<18; i++)
        {
            if (layerName[i].Equals(layerName))
                return layerNames[i + index];
        }
        return null;
    }

}

public interface ICustomLayer
{
    void layer(string sortingLayerName);
}
