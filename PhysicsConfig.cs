using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics
{
	public class PhysicsConfig : MonoBehaviour
	{
		public static Color DefaultLineColour = new Color(1, 1, 1, 1);
		public static Color CollidingLineColour = new Color(1, 0, 0, 1);
		public static Color SubLineColour = new Color(0, 0, 1, 1);

		public Color DefaultColor = DefaultLineColour;
		public Color CollidingColor = CollidingLineColour;
		public Color AltLineColor = SubLineColour;

		private void OnValidate()
		{
			DefaultLineColour = DefaultColor;
			CollidingLineColour = CollidingColor;
			SubLineColour = AltLineColor;
		}
	}
}

