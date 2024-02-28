//Notepad Tool by SVerde
//contact@sverdegd.com
//https://github.com/sverdegd

using UnityEngine;

namespace SVerdeTools.Notepad{
	public class ColorThemeData{

		public Color header;
		public Color body;

		public ColorThemeData(Color header, Color body){
			this.header = header;
			this.body = body;
		}

		public ColorThemeData(Vector3 header, Vector3 body){
			this.header = new Color(header.x, header.y, header.z);
			this.body = new Color(body.x, body.y, body.z);
		}

		public ColorThemeData(float headerR, float headerG, float headerB, float bodyR, float bodyG, float bodyB){
			this.header = new Color(headerR, headerG, headerB);
			this.body = new Color(bodyR, bodyG, bodyB);
		}

		public static Color ColorOne(float r, float g, float b){
			return new Color (r / 255f, g / 255f, b / 255f);
		}

		public static Color ColorOne(float r, float g, float b, float a){
			return new Color (r / 255f, g / 255f, b / 255f, a/255f);
		}
	}
}