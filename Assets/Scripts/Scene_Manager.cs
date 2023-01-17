using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class Scene_Manager : MonoBehaviour
{
    [SerializeField]
    Scene_Persistence sp;

    Player player;
    Inventory_Manager im;

    entrance currentEntrance;

    public RectTransform fadeOut;
    public Image fadeImage;

    float fadeSeconds = 0.3f;
    int fadeSteps = 30;

    bool fadingOut = false;

    string[] sortingLayers;

    //public bool manageLayers;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        im = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Inventory_Manager>();

        fadeImage = fadeOut.GetComponent<Image>();


        sortingLayers = new string[SortingLayer.layers.Length];

        foreach(SortingLayer layer in SortingLayer.layers)
        {
            sortingLayers[SortingLayer.GetLayerValueFromID(layer.id)] = layer.name;
        }

        
       

        player.GetComponent<Player>().unfreeze();

        //set character position from SO
        //if none, use a default spawn point

        //find object of name in entrance SO variable

        if(Time.time < 0.1f)
        {
            sp.setChangingScenes(false);
        }


        if (sp.isChangingScenes())
            sceneChangeStart();



        fadeIn();
    }

    void sceneChangeStart()
    {


        //player.GetComponent<Player>().freeze();
        player.GetComponent<Player>().unfreeze();

        currentEntrance = null;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Entrance"))
        {
            entrance e = go.GetComponent<entrance>();
            if (e.getEntranceNo() == sp.getEntranceNo())
            {
                currentEntrance = e;
            }
        }
        if (currentEntrance == null) Debug.Log("Error: No Scene Entrance Found");

        setPlayer();

        sp.setChangingScenes(false);

        player.GetComponent<Player>().unfreeze();

    }

    void setPlayer()
    {
        player.sceneInitialize(sp.health, currentEntrance.transform.position, currentEntrance.getURDL());
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public void changeScene(string targetSceneName, byte entranceNo)
    {
        if (sp.isChangingScenes()) return;
        
        player.GetComponent<Player>().freeze();

        sp.setChangingScenes(true);
        
        //store health
        sp.health = player.getHealth();

        //store inventory
        sp.items = im.getItems();

        sp.setEntrance(entranceNo);

        //SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);


        fadeImage.enabled = true;
        fadingOut = true;
        StartCoroutine(FadeOut(targetSceneName));


        
    }

    public int getHealth()
    {
        return sp.getHealth();
    }

    IEnumerator FadeOut(string sceneName)
    {   

        while (fadeImage.color.a < 1)
        {
            fadeImage.color = new Color(0, 0, 0, fadeImage.color.a + (1f / fadeSteps));
            yield return new WaitForSeconds(fadeSeconds / fadeSteps);
            //yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }


    IEnumerator FadeIn()
    {        
        //Fade screen back in

        //wait for Fade to Black to Complete
        //If we decide to change scenes (fade out) while still fading in, cancel the fade in
        while (fadeImage.color.a > 0 && !fadingOut)
        {
            fadeImage.color = new Color(0, 0, 0, fadeImage.color.a - (1f / fadeSteps));
            yield return new WaitForSeconds(fadeSeconds / fadeSteps);
            //yield return null;
        }
        if (fadingOut) yield break;


        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.enabled = false;
        
    }

    void fadeIn()
    {
        fadeImage.enabled = true;
        fadeImage.color = new Color(0, 0, 0, 1);
        StartCoroutine(FadeIn());

    }

    public void incrementLayer(GameObject obj, sbyte increment)
    {
        SpriteRenderer[] srs = obj.GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer sr in srs)
        {
            sr.sortingLayerName = sortingLayers[SortingLayer.GetLayerValueFromID(sr.sortingLayerID) + increment*3];
            
        }
    }



    

}
