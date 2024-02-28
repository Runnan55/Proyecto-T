//Notepad Tool by SVerde
//contact@sverdegd.com
//https://github.com/sverdegd

namespace SVerdeTools.Notepad{

	public class DefaultColorThemes {
		static ColorThemeData _yellow =  new ColorThemeData (ColorThemeData.ColorOne(255,185,1), ColorThemeData.ColorOne(255,242,181));
		static ColorThemeData  _green =  new ColorThemeData (ColorThemeData.ColorOne(17,137,5), ColorThemeData.ColorOne(199,239,196));
		static ColorThemeData  _pink =  new ColorThemeData (ColorThemeData.ColorOne(217,1,169), ColorThemeData.ColorOne(255,195,244));
		static ColorThemeData  _purple =  new ColorThemeData (ColorThemeData.ColorOne(93,36,155), ColorThemeData.ColorOne(222,198,251));
		static ColorThemeData  _blue =  new ColorThemeData (ColorThemeData.ColorOne(1,121,215), ColorThemeData.ColorOne(196,229,255));
		static ColorThemeData  _grey =  new ColorThemeData (ColorThemeData.ColorOne(151,151,151), ColorThemeData.ColorOne(250,250,250));

		public static ColorThemeData Yellow{
			get{ return _yellow; }
		}
		public static ColorThemeData Green{
			get{ return _green; }
		}
		public static ColorThemeData Pink{
			get{ return _pink; }
		}
		public static ColorThemeData Purple{
			get{ return _purple; }
		}
		public static ColorThemeData Blue{
			get{ return _blue; }
		}
		public static ColorThemeData Grey{
			get{ return _grey; }
		}
		public static ColorThemeData Gray{
			get{ return _grey; }
		}
	}
}
