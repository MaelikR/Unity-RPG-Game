#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RockGenerator))]
public class RockGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Rocks"))
        {
            RockGenerator rockGenerator = (RockGenerator)target;
            rockGenerator.GenerateRocks();
        }
    }

    void OnSceneGUI()
    {
        RockGenerator rockGenerator = (RockGenerator)target;

        // Visualize the generation area in the scene view
        Handles.color = Color.green;
        Handles.DrawWireCube(rockGenerator.transform.position, rockGenerator.generationArea * 2);
    }
}
#endif
