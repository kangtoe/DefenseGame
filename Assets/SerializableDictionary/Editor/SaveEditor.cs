using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveViewer))]
public class SaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SaveViewer saveViewer = (SaveViewer)target;
        if (GUILayout.Button("Delete Save"))
        {
            SaveManager.ClearData();
        }
    }
}
