//Notepad Tool by SVerde
//contact@sverdegd.com
//https://github.com/sverdegd

using UnityEngine;

namespace SVerdeTools.Notepad{
	[CreateAssetMenu(fileName = "_newColorTheme", menuName = "SVerdeTools/Notepad/Color Theme", order = 1)]
	public class ColorTheme : ScriptableObject {

		public Color header = DefaultColorThemes.Yellow.header;
		public Color body = DefaultColorThemes.Yellow.body;

	}
}
