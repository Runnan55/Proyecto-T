using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LayoutData))]
public class LayoutDataEditor : Editor {
    public override void OnInspectorGUI() {
        LayoutData layout = (LayoutData)target;
        
        layout.InitGrid();

        int rows = layout.rows;
        int cols = layout.cols;
        
        for (int i = 0; i < rows; i++) {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < cols; j++) {
                int index = i * cols + j;
                layout.grid[index] = GUILayout.Toggle(layout.grid[index], "");
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUI.changed) {
            EditorUtility.SetDirty(layout);
        }
    }
}