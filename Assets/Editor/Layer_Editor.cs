using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class Layer_Editor : EditorWindow
{
    Layer_Manager lm;

    float dimValue = 0.8f;

    Dictionary<string, List<Light2D>> globalLights;
    List<GameObject> levels;


    [MenuItem("Window/Layer Editor")]
    public static void ShowWindow()
    {
        
        // Get existing open window or if none, create a new one
        Layer_Editor window = GetWindow<Layer_Editor>("Layer Editor");
        window.minSize = new Vector2(200, 200);
    }


    private void OnGUI()
    {
        lm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Layer_Manager>();

        GUILayout.Label("Relayer Objects in Scene");

        GUILayout.Label("Inspect Level");
        if (GUILayout.Button("Inspect"))
        {
            inspectLayer();
        }

        if (GUILayout.Button("DeInspect"))
        {
            DeInspect();
        }

        if (GUILayout.Button("INIT"))
        {
            init();
        }


        // Add a button
        if (GUILayout.Button("Layer"))
        {
            lm.relayer();
        }

        init();

    }

    void inspectLayer()
    {
               
        GameObject selectedLevel = Selection.activeGameObject;
        string[] splitName = selectedLevel.name.Split(" ");
        foreach(GameObject level in levels)
        {
            if(level.name.CompareTo(selectedLevel.name) == 0)
            {
                brightenLevel(globalLights[level.name]);
            }
            else
            {
                dimLevel(globalLights[level.name]);
            }
        }
       
    }

    void brightenLevel(List<Light2D> lights)
    {
        foreach(Light2D light in lights)
        {
            light.intensity = 1.0f;
        }
    }

    void dimLevel(List<Light2D> lights)
    {
        foreach (Light2D light in lights)
        {
            light.intensity = dimValue;
        }
    }

    void init()
    {
        levels = lm.getAllLevels();
        globalLights = new Dictionary<string, List<Light2D>>();

        foreach (GameObject level in levels)
        {
            Light2D[] lights = level.GetComponentsInChildren<Light2D>();
            List<Light2D> list = new List<Light2D>();
            foreach (Light2D l2d in lights)
            {
                if (l2d.lightType == Light2D.LightType.Global) list.Add(l2d);
            }
            globalLights.Add(level.name, list);
        }
    }

    void DeInspect()
    {
        foreach (GameObject level in levels)
        {
            brightenLevel(globalLights[level.name]);
        }
    }

}
