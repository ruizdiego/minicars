
// Unity Framework
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Add New Node"))
        {
            WaypointManager wpm = target as WaypointManager;
            wpm.AddNode();
        }
        
        if (GUILayout.Button("Clear All"))
        {
            WaypointManager wpm = target as WaypointManager;
            wpm.ClearAll();
        }
        
    }
}
