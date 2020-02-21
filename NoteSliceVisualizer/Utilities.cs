using System.Linq;
using UnityEngine;

namespace NoteSliceVisualizer
{
	public static class Utilities
	{
		public static Material UiNoGlow;

		public static void Initialize()
		{
			UiNoGlow = Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault();
		}
	}
}
