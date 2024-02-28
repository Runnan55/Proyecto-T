//Notepad Tool by SVerde
//contact@sverdegd.com
//https://github.com/sverdegd

using UnityEngine;
using UnityEditor;
using System.IO;

namespace SVerdeTools.Notepad{
	public class NotepadWindow : EditorWindow {

		public static NotepadWindow window;

		static Note note;
		static string assetName;
		static string assetPath;

		static GUIContent wTitle = new GUIContent();

		static Rect headerSection = new Rect ();
		static Texture2D headerSectionTexture;
		static Color headerSectionColor = DefaultColorThemes.Yellow.header;

		static Rect bodySection = new Rect ();
		static Texture2D bodySectionTexture;
		static Color bodySectionColor =  DefaultColorThemes.Yellow.body;

        static Rect footerSection = new Rect();

        static int noteIndex;
        static int notesCount;
        static string[] notesPaths;
        static string[] notesNames;
        static string noteInfo; 

        GUISkin skin;
		Texture addT;
		Texture browseT;
		Texture copyT;
		Texture exportT;
		Texture customizeT;

		Vector2 scrollPos;
        static int arrayIndex;

        [MenuItem ("SVerdeTools/Notepad", false, 1)]
		public static void  OpenWindow () {
			if (window == null) {
				window = (NotepadWindow)GetWindow (typeof(NotepadWindow));

				wTitle.text = "Notepad";
				wTitle.image = Resources.Load<Texture> ("Icons/NotepadIcon");
				wTitle.tooltip = "Use this tool to take your notes";
				window.minSize = new Vector2 (475f, 350f);
				window.titleContent = wTitle;

			}

			window.Show ();
		}

		void OnEnable(){
			LoadIcons ();
			LoadPrefs ();
            SearchAllNotes();
		}

		static void InitTextures(){
			headerSectionTexture = new Texture2D (1, 1);
			headerSectionTexture.SetPixel (0, 0, headerSectionColor);
			headerSectionTexture.Apply ();

			bodySectionTexture = new Texture2D (1, 1);
			bodySectionTexture.SetPixel (0, 0, bodySectionColor);
			bodySectionTexture.Apply ();
		}

		void LoadIcons(){
			skin = Resources.Load<GUISkin> ("GUISkins/NotepadSkin");
			addT = Resources.Load<Texture> ("Icons/AddButtonIcon");
			browseT = Resources.Load<Texture> ("Icons/BrowseButtonIcon");
			copyT = Resources.Load<Texture> ("Icons/CopyButtonIcon");
			exportT = Resources.Load<Texture> ("Icons/ExportButtonIcon");
			customizeT = Resources.Load<Texture> ("Icons/CustomizeButtonIcon");
		}

		void DrawLayouts(){
			headerSection.x = 0;
			headerSection.y = 0;
			headerSection.width = Screen.width;
			headerSection.height = 60;

			bodySection.x = 0;
			bodySection.y = 60;
			bodySection.width = Screen.width;
			bodySection.height = Screen.height - 40;

            footerSection.x = 0;
            footerSection.y = Screen.height - 40;
            footerSection.width = Screen.width;
            footerSection.height = Screen.height;

            if (headerSectionTexture == null || bodySectionTexture == null) {
				LoadPrefs ();
			}

			GUI.DrawTexture (headerSection, headerSectionTexture);
			GUI.DrawTexture (bodySection, bodySectionTexture);
            GUI.DrawTexture(footerSection, headerSectionTexture);
        }

		void OnGUI(){

			DrawLayouts ();

			GUILayout.BeginArea (headerSection);
			EditorGUILayout.Space ();
			GUILayout.Label ("Notepad", skin.GetStyle("Header"));

			if (GUI.Button(new Rect(new Vector2(Screen.width - 55f, 5f), new Vector2(50f, 50f)), copyT, skin.GetStyle("button"))){
				CopyToClipboard();
			}
				
			if (GUI.Button(new Rect(new Vector2(Screen.width - 110f, 5f), new Vector2(50f, 50f)), customizeT, skin.GetStyle("button"))) {
				ChangeColorTheme();
			}

			if (GUI.Button(new Rect(new Vector2(Screen.width - 165f, 5f), new Vector2(50f, 50f)), exportT, skin.GetStyle("button"))) {
				SaveAsJson ();
			}

			if (GUI.Button(new Rect(new Vector2(Screen.width - 220f, 5f), new Vector2(50f, 50f)), browseT, skin.GetStyle("button"))) {
				OpenNote ();
			}

			if (GUI.Button(new Rect(new Vector2(Screen.width - 275f, 5f), new Vector2(50f, 50f)), addT, skin.GetStyle("button"))) {
				CreateNewNote ();
			}

			EditorGUILayout.Space ();
			GUILayout.EndArea ();

			GUILayout.BeginArea (bodySection);

			if (note != null) {

				EditorGUI.BeginChangeCheck ();

				GUI.skin = skin;
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height-100));
				GUI.skin = null;

				string content = EditorGUILayout.TextArea(note.content, skin.GetStyle("textarea"));
				EditorGUILayout.EndScrollView();

				if (EditorGUI.EndChangeCheck ()) {
					Undo.RecordObject (note, "Changed Note Content");
					EditorUtility.SetDirty (note);
					note.content = content;
				}

			} else {
				GUI.Label (new Rect (3f, 3f, Screen.width - 6f, Screen.height - 88f), "Load or create a note to use the notepad :D", skin.GetStyle ("label"));
			}

			GUILayout.EndArea ();

            GUILayout.BeginArea(footerSection);
            if (note != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(noteInfo, EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                GUI.color = headerSectionColor;
                arrayIndex = EditorGUILayout.Popup(arrayIndex, notesNames);
                if (EditorGUI.EndChangeCheck())
                {
                    Note temp = AssetDatabase.LoadAssetAtPath(notesPaths[arrayIndex], typeof(Note)) as Note;
                    if(temp != null)
                    {
                        note = temp;
                        assetName = notesNames[arrayIndex];
                        assetPath = notesPaths[arrayIndex];
                        SavePrefs();
                        SearchAllNotes();
                    }
                    else
                    {
                        SearchAllNotes();
                    }
                }
                if (GUILayout.Button("<", EditorStyles.miniButtonLeft))
                {
                    if (noteIndex - 1 > -1)
                    {
                        OpenNoteFromPath(notesPaths[noteIndex - 1]);
                    }
                }
                if (GUILayout.Button(">", EditorStyles.miniButtonRight))
                {
                    if (noteIndex + 1 < notesCount)
                    {
                        OpenNoteFromPath(notesPaths[noteIndex + 1]);
                    }
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();

            }
            GUILayout.EndArea();

        }

		void OnInspectorUpdate(){
			Repaint ();
		}

		void OpenNote () {
			string absPath = EditorUtility.OpenFilePanel ("Select Note", Application.dataPath, "asset");
			if (absPath != string.Empty) {
				string ext = Path.GetExtension (absPath);
				string relPath = absPath.Substring (Application.dataPath.Length - ext.Length);
				Note temp = AssetDatabase.LoadAssetAtPath (relPath, typeof(Note)) as Note;
				if (temp != null) {
					note = temp;
					assetName = Path.GetFileNameWithoutExtension (absPath);
					assetPath = relPath;
					SavePrefs ();
                    SearchAllNotes();
                    GUI.FocusControl(null);
				} else {
					EditorUtility.DisplayDialog ("Error", "The selected asset is not a Note", "Ok");
				}
			}
		}

		void CreateNewNote () {
			Note asset = ScriptableObject.CreateInstance<Note> ();
			string relPath = EditorUtility.SaveFilePanelInProject ("Create new Note", "_newNote", "asset", Application.dataPath);
			if (relPath != string.Empty) {
				AssetDatabase.CreateAsset (asset, relPath);
				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh ();
				note = asset;
				assetName = Path.GetFileNameWithoutExtension (relPath);
				assetPath = relPath;
				SavePrefs ();
                SearchAllNotes();
                GUI.FocusControl(null);
			}
            
		}

		void CopyToClipboard(){
			if (note != null) {
				TextEditor tea = new TextEditor ();
				tea.text = note.content;
				tea.SelectAll ();
				tea.Copy ();
				EditorUtility.DisplayDialog ("Info", "Note copied to clipboard", "Ok");
			} else {
				EditorUtility.DisplayDialog ("Error", "There's no note open ", "Ok");
			}
		}

		void SaveAsJson(){
			if (note != null) {
				string dataAsJson = JsonUtility.ToJson (note);
				string filePath = EditorUtility.SaveFilePanel ("Save Note as Json", Application.dataPath, assetName, "json");
				if (filePath != string.Empty) {
					File.WriteAllText (filePath, dataAsJson);
					AssetDatabase.Refresh ();
				}
			}

		}

		void ChangeColorTheme(){
			string absPath = EditorUtility.OpenFilePanel("Select Color Theme", Application.dataPath, "asset");
			if (absPath != string.Empty) {
				string ext = Path.GetExtension (absPath);
				string relPath = absPath.Substring (Application.dataPath.Length - ext.Length);
				ColorTheme temp = AssetDatabase.LoadAssetAtPath (relPath, typeof(ColorTheme)) as ColorTheme;
				if (temp != null) {
					headerSectionColor = temp.header;
					bodySectionColor = temp.body;
					InitTextures ();
					Repaint ();
					SavePrefs ();
				} else {
					EditorUtility.DisplayDialog ("Error", "The selected asset is not a Color Theme", "Ok");
				}
			}
		}

	
		static void SavePrefs(){

			EditorPrefs.SetString (Application.productName + ".HeaderColor", "#" + ColorUtility.ToHtmlStringRGB(headerSectionColor));
			EditorPrefs.SetString (Application.productName + ".BodyColor", "#" + ColorUtility.ToHtmlStringRGB(bodySectionColor));
			EditorPrefs.SetString (Application.productName + ".LastNotePath", assetPath);

		}

		void LoadPrefs(){

			assetPath = EditorPrefs.GetString (Application.productName + ".LastNotePath");

			Color hc;
			if (ColorUtility.TryParseHtmlString (EditorPrefs.GetString (Application.productName + ".HeaderColor"), out hc)) {
				headerSectionColor = hc;
			} else {
				headerSectionColor = DefaultColorThemes.Yellow.header;
			}

			Color bc;
			if(ColorUtility.TryParseHtmlString (EditorPrefs.GetString (Application.productName + ".BodyColor"), out bc)){
				bodySectionColor = bc;
			}
			else{
				bodySectionColor = DefaultColorThemes.Yellow.body;
			}

			if (assetPath != string.Empty) {
				note = AssetDatabase.LoadAssetAtPath (assetPath, typeof(Note)) as Note;
			}

			InitTextures ();
			Repaint ();
		
		}

		public static void OpenNoteFromEditor(Note newNote){
			OpenWindow ();
			note = newNote;
			assetName = newNote.name;
			assetPath = AssetDatabase.GetAssetPath (newNote);
			SavePrefs ();
            SearchAllNotes();
		}

		public static void ApplyColorThemeFromEditor(ColorTheme ct){
			OpenWindow ();
            assetPath = EditorPrefs.GetString(Application.productName + ".LastNotePath");
            if (assetPath != string.Empty)
            {
                note = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Note)) as Note;
            }
            headerSectionColor = ct.header;
			bodySectionColor = ct.body;
			InitTextures ();
			window.Repaint ();
			SavePrefs ();
            SearchAllNotes();
		}

        void OpenNoteFromPath(string notePath)
        {
         
            Note temp = AssetDatabase.LoadAssetAtPath(notePath, typeof(Note)) as Note;
            if (temp != null)
            {
                note = temp;
                assetName = Path.GetFileNameWithoutExtension(notePath);
                assetPath = notePath;
                SavePrefs();
                SearchAllNotes();
                GUI.FocusControl(null);

            }
            else
            {
                SearchAllNotes();
            }

        }

        static void SearchAllNotes()
        {
            string[] guids;
            guids = AssetDatabase.FindAssets("t: Note");

            notesCount = guids.Length;

            if (notesCount == 0)
            {
                return;
            }

            notesNames = new string[notesCount];
            notesPaths = new string[notesCount];

            for (int i = 0; i < notesCount; i++)
            {
                notesPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                notesNames[i] = GetName(notesPaths[i]);

                if (GetName(assetPath) == notesNames[i])
                {
                    noteIndex = i;
                    arrayIndex = noteIndex;
                }
            }

            UpdateNoteInfo();

        }

        static string GetName(string path)
        {
            return path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);
        }

        static void UpdateNoteInfo()
        {
            noteInfo = " " + notesNames[noteIndex] + " (" + (noteIndex + 1).ToString() + "/" + notesCount.ToString() + ")";
        }
    }
}