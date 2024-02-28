//Notepad Tool by SVerde
//contact@sverdegd.com
//https://github.com/sverdegd

using UnityEngine;
using UnityEditor;

namespace SVerdeTools.Notepad{
	[CustomEditor(typeof(Note))]
	public class NoteEditor : Editor {

		public override void OnInspectorGUI(){
			Note myTarget = (Note)target;

			GUILayout.Label ("Note content preview:", EditorStyles.boldLabel);
			GUILayout.BeginVertical ("box");

			string aux;
			if (myTarget.content.Length > 280) {
				aux = myTarget.content.Substring (0, 280) + " [...]";
			} else if (myTarget.content == string.Empty) {
				aux = "[This note is blank]";
			}else{
				aux = myTarget.content;
			}

			GUILayout.TextArea (aux, GUILayout.MinWidth(Screen.width));
			GUILayout.EndVertical ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("Copy to clipboard")) {
				CopyToClipboard (myTarget);
			}
			if (GUILayout.Button ("Open")) {
				Open(myTarget);
			}
		}


		void CopyToClipboard(Note note){
			TextEditor tea = new TextEditor ();
			tea.text = note.content;
			tea.SelectAll ();
			tea.Copy ();	
		}

		void Open(Note note){
			NotepadWindow.OpenNoteFromEditor (note);
		}
	}
}
