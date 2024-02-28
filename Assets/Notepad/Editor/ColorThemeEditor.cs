//Notepad Tool by SVerde
//contact@sverdegd.com
//https://github.com/sverdegd

using UnityEngine;
using UnityEditor;

namespace SVerdeTools.Notepad{
	[CustomEditor(typeof(ColorTheme))]
	public class ColorThemeEditor : Editor {

		bool enableFoldout = false;

		public override void OnInspectorGUI(){
			ColorTheme myTarget = (ColorTheme)target;

#if !UNITY_2018
            ColorPickerHDRConfig hdrpc = new ColorPickerHDRConfig(0, 1, 0, 1);
#endif


            GUIContent header = new GUIContent ();
			header.text = "Header Color";
			header.tooltip = "The color of the top of the notepad";
			GUIContent body = new GUIContent ();
			body.text = "Body Color";
			body.tooltip = "The color of the body of the notepad";





#if UNITY_2018
            EditorGUI.BeginChangeCheck();
            Color h = EditorGUILayout.ColorField(header, myTarget.header, true, false, false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(myTarget, "Changed Color Theme");
                EditorUtility.SetDirty(myTarget);
                myTarget.header = h;
            }

            EditorGUI.BeginChangeCheck();
			Color b  = EditorGUILayout.ColorField(body, myTarget.body, true, false, false);
            {
                Undo.RecordObject(myTarget, "Changed Color Theme");
                EditorUtility.SetDirty(myTarget);
                myTarget.body = b;
            }
#else
            EditorGUI.BeginChangeCheck();
            Color h = EditorGUI.ColorField(new Rect(10, 50, Screen.width - 20, 15), header, myTarget.header, true, false, false, hdrpc);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(myTarget, "Changed Color Theme");
                EditorUtility.SetDirty(myTarget);
                myTarget.header = h;
            }

            EditorGUI.BeginChangeCheck();
            Color b = EditorGUI.ColorField(new Rect(10, 75, Screen.width - 20, 15), body, myTarget.body, true, false, false, hdrpc);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(myTarget, "Changed Color Theme");
                EditorUtility.SetDirty(myTarget);
                myTarget.body = b;
            }
#endif


            enableFoldout  = EditorGUI.Foldout (new Rect(20, 100, Screen.width-20, 15), enableFoldout, "Default Themes", true);

			if (enableFoldout) {
				EditorGUI.DrawRect (new Rect (5, 120, Screen.width - 10, 150), DefaultColorThemes.Gray.body);
				GUI.backgroundColor = DefaultColorThemes.Yellow.header;
				if (GUI.Button (new Rect (10, 125, Screen.width - 20, 15), "Yellow")) {
                    Undo.RecordObject(myTarget, "Changed Color Theme");
                    EditorUtility.SetDirty(myTarget);
                    myTarget.header = DefaultColorThemes.Yellow.header;
					myTarget.body = DefaultColorThemes.Yellow.body;
				}
				GUI.backgroundColor = DefaultColorThemes.Green.header;
				if (GUI.Button (new Rect (10, 150, Screen.width - 20, 15), "Green")) {
                    Undo.RecordObject(myTarget, "Changed Color Theme");
                    EditorUtility.SetDirty(myTarget);
                    myTarget.header = DefaultColorThemes.Green.header;
					myTarget.body = DefaultColorThemes.Green.body;
				}
				GUI.backgroundColor = DefaultColorThemes.Pink.header;
				if (GUI.Button (new Rect (10, 175, Screen.width - 20, 15), "Pink")) {
                    Undo.RecordObject(myTarget, "Changed Color Theme");
                    EditorUtility.SetDirty(myTarget);
                    myTarget.header = DefaultColorThemes.Pink.header;
					myTarget.body = DefaultColorThemes.Pink.body;
				}
				GUI.backgroundColor = DefaultColorThemes.Purple.header;
				if (GUI.Button (new Rect (10, 200, Screen.width - 20, 15), "Purple")) {
                    Undo.RecordObject(myTarget, "Changed Color Theme");
                    EditorUtility.SetDirty(myTarget);
                    myTarget.header = DefaultColorThemes.Purple.header;
					myTarget.body = DefaultColorThemes.Purple.body;
				}
				GUI.backgroundColor = DefaultColorThemes.Blue.header;
				if (GUI.Button (new Rect (10, 225, Screen.width - 20, 15), "Blue")) {
                    Undo.RecordObject(myTarget, "Changed Color Theme");
                    EditorUtility.SetDirty(myTarget);
                    myTarget.header = DefaultColorThemes.Blue.header;
					myTarget.body = DefaultColorThemes.Blue.body;
				}
				GUI.backgroundColor = DefaultColorThemes.Grey.header;
				if (GUI.Button (new Rect (10, 250, Screen.width - 20, 15), "Grey")) {
                    Undo.RecordObject(myTarget, "Changed Color Theme");
                    EditorUtility.SetDirty(myTarget);
                    myTarget.header = DefaultColorThemes.Grey.header;
					myTarget.body = DefaultColorThemes.Grey.body;
				}
				GUI.backgroundColor = Color.white;

				if (GUI.Button (new Rect (10, 285, Screen.width - 20, 25), "Apply this theme")) {
					NotepadWindow.ApplyColorThemeFromEditor (myTarget);
				}
			} else {
				if (GUI.Button (new Rect (10, 130, Screen.width - 20, 25), "Apply this theme")) {
					NotepadWindow.ApplyColorThemeFromEditor (myTarget);
				}
			}

			



		}


	}
}
