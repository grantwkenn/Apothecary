using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;

public class Path_Editor : EditorWindow
{
    [SerializeField]
    NPC_Behavior_Data SO;
    [SerializeField]
    Transform Parent_Transform;

    Transform[] targets;

    public static int rounding;

    [MenuItem("Window/Path Trainer")]
    // Start is called before the first frame update
    public static void ShowWindow()
    {
        GetWindow<Path_Editor>("Edit Mode Functions");
    }

    private void Update()
    {
        
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Train Object"))
        {
            trainTargets();
        }
        if (GUILayout.Button("Round Coordinates")) { roundCoords(); }


        SO = (NPC_Behavior_Data)EditorGUILayout.ObjectField("L", SO, typeof(NPC_Behavior_Data), true);

        Parent_Transform = (Transform)EditorGUILayout.ObjectField("Parent", Parent_Transform, typeof(Transform), true);

        rounding = (int)EditorGUILayout.Slider(rounding, 0, 16);

    }

    void roundCoords()
    {
        if (rounding == 0) return;
        
        float x = Parent_Transform.position.x;
        float y = Parent_Transform.position.y;
        int sixteenths = (int)((x - (int)x) * rounding);
        x = (int)x + sixteenths/rounding;
        sixteenths = (int)((y - (int)y) * rounding);
        y = (int)y + sixteenths/rounding;

        Parent_Transform.position = new Vector2(x, y);

        foreach(Transform t in Parent_Transform.GetComponentsInChildren<Transform>())
        {
            if (t == Parent_Transform) continue;

            x = t.position.x;
            y = t.position.y;
            sixteenths = (int)((x - (int)x) * rounding);
            x = (int)x + sixteenths/rounding;
            sixteenths = (int)((y - (int)y) * rounding);
            y = (int)y + sixteenths/rounding;

            t.position = new Vector2(x, y);


        }
    }

    void trainTargets()
    {
        Transform[] transforms = Parent_Transform.GetComponentsInChildren<Transform>();
        Vector2[] positions = new Vector2[transforms.Length-1];
        int i=0;
        foreach(Transform t in transforms)
        {
            if (t == Parent_Transform) continue;
            positions[i] = t.position;
            i++;
        }

        SO.trainTargets(positions);
    }

}
