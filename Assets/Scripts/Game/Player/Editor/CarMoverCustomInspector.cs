// Unity Framework
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CarMover))]
public class CarMoverCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Move Car to Start"))
        {
            CarMover cm = target as CarMover;
            cm.MoveToStartPos();
        }
    }
}
