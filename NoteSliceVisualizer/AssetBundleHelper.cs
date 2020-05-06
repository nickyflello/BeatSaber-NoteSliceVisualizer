using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace NoteSliceVisualizer
{
	public static class AssetBundleHelper
	{
		private static AssetBundle _assetBundle;

		public static GameObject Canvas;
		public static GameObject NoteUI;
		public static Texture TriangleTexture;

		public static void LoadAssetBundle()
		{
			if (_assetBundle == null)
			{
				Console.WriteLine("[NoteSliceVisualizer] Loading Asset Bundle");

				Assembly assembly = Assembly.GetExecutingAssembly();
				using (Stream stream = assembly.GetManifestResourceStream("NoteSliceVisualizer.noteslicebundle"))
				{
					_assetBundle = AssetBundle.LoadFromStream(stream);
				}

				Canvas = LoadAsset<GameObject>("Canvas");
				NoteUI = LoadAsset<GameObject>("NoteUI");
				TriangleTexture = LoadAsset<Texture>("Triangle");
			}
		}

		private static T LoadAsset<T>(string name)
			where T : UnityEngine.Object
		{
			T obj = _assetBundle.LoadAsset<T>(name);

			// Replace any UI materials with beat saber ones
			if (obj is GameObject gameObject)
			{
				foreach (Graphic graphic in gameObject.GetComponentsInChildren<Graphic>())
				{
					graphic.material = Utilities.UiNoGlow;
				}
			}
			else if (obj is Graphic graphic)
			{
				graphic.material = Utilities.UiNoGlow;
			}

			return obj;
		}
	}
}
