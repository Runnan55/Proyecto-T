#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    private GridManager gridManager;
    private int selectedWave = 0;
    private Vector2 scrollPos;

    private void OnEnable() => gridManager = (GridManager)target;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Wave Layout Editor", EditorStyles.boldLabel);
        
        DrawWaveSelector();
        DrawGridEditor();
        DrawWaveControls();
        
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawWaveSelector()
    {
        EditorGUILayout.BeginHorizontal();
        
        if (gridManager.waveLayouts != null && gridManager.waveLayouts.Count > 0)
        {
            selectedWave = Mathf.Clamp(
                EditorGUILayout.IntSlider("Selected Wave", selectedWave, 0, gridManager.waveLayouts.Count - 1),
                0, 
                gridManager.waveLayouts.Count - 1
            );
        }
        
    if (GUILayout.Button("Add Wave", GUILayout.Width(100)))
    {
        WaveData newWave = new WaveData(gridManager.gridSize);
        gridManager.waveLayouts.Add(newWave);
        SaveGridChanges();
    }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGridEditor()
    {
        if (gridManager.waveLayouts == null || 
            gridManager.waveLayouts.Count == 0 || 
            selectedWave >= gridManager.waveLayouts.Count) return;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        WaveData currentWave = gridManager.waveLayouts[selectedWave];
        int gridSize = gridManager.gridSize;

        if (currentWave.tilesToRaise == null || 
            currentWave.tilesToRaise.Length != gridSize * gridSize)
        {
            currentWave.tilesToRaise = new bool[gridSize * gridSize];
        }

    for (int y = 0; y < gridSize; y++)
    {
        EditorGUILayout.BeginHorizontal();
        for (int x = 0; x < gridSize; x++)
        {
            int index = y * gridSize + x;
            bool currentState = currentWave.tilesToRaise[index];
            
            GUI.color = currentState ? Color.green : Color.red;
            if (GUILayout.Button("", GUILayout.Width(25), GUILayout.Height(25)))
            {
                currentWave.tilesToRaise[index] = !currentState;
                SaveGridChanges();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        GUI.color = Color.white;
    }

    private void DrawWaveControls()
    {
        if (gridManager.waveLayouts == null || gridManager.waveLayouts.Count == 0) return;
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Apply Wave"))
        {
            if (selectedWave < gridManager.waveLayouts.Count)
                gridManager.ActivateWave(selectedWave);
        }
        
        if (GUILayout.Button("Clear Wave"))
        {
            if (selectedWave < gridManager.waveLayouts.Count)
                System.Array.Clear(gridManager.waveLayouts[selectedWave].tilesToRaise, 0, 
                                  gridManager.waveLayouts[selectedWave].tilesToRaise.Length);
            SaveGridChanges();
        }
        
        if (GUILayout.Button("Remove Wave"))
        {
            if (selectedWave < gridManager.waveLayouts.Count)
            {
                gridManager.waveLayouts.RemoveAt(selectedWave);
                selectedWave = Mathf.Clamp(selectedWave - 1, 0, gridManager.waveLayouts.Count - 1);
                SaveGridChanges();
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void SaveGridChanges()
    {
        EditorUtility.SetDirty(gridManager);
        PrefabUtility.RecordPrefabInstancePropertyModifications(gridManager);
    }
}
#endif