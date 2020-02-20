using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoteSliceVisualizer
{
	public static class AssetBundleHelper
	{
		static AssetBundle _assetBundle;

		public static void LoadAssetBundle()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			using (Stream stream = assembly.GetManifestResourceStream("NoteSliceVisualizer.noteslicebundle"))
			{
				_assetBundle = AssetBundle.LoadFromStream(stream);
			}
		}

		public static GameObject Instantiate(string name)
		{
			GameObject template = _assetBundle.LoadAsset<GameObject>(name);
			return GameObject.Instantiate(template);
		}

	}
}
