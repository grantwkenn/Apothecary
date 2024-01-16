using UnityEngine;
using UnityEditor;

public class Layer_Editor : EditorWindow
{
    Layer_Manager lm;


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
        // Add a button
        if (GUILayout.Button("Layer"))
        {
            lm.relayer();
        }
    }



}
