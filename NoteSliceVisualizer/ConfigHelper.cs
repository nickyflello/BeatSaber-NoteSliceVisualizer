using Newtonsoft.Json;
using System;
using System.IO;

namespace NoteSliceVisualizer
{
	public static class ConfigHelper
	{
		public static Config Config { get; private set; }

		private const string ConfigFolder = "UserData";
		private const string ConfigFileName = "NoteSliceVisualizerConfig.json";

		private static string ConfigFolderPath => Path.Combine(Environment.CurrentDirectory, ConfigFolder);
		private static string ConfigFilePath => Path.Combine(ConfigFolderPath, ConfigFileName);

		public static void LoadConfig()
		{
			Directory.CreateDirectory(ConfigFolderPath);

			if (!File.Exists(ConfigFilePath))
			{
				Console.WriteLine("[NoteSliceVisualizer] Creating Default Config");
				Config = new Config();
			}
			else
			{
				Console.WriteLine("[NoteSliceVisualizer] Loading Config");
				string data = File.ReadAllText(ConfigFilePath);
				Config = JsonConvert.DeserializeObject<Config>(data);
			}

			// TODO: Save config version number
			SaveConfig();
		}

		public static void SaveConfig()
		{
			Console.WriteLine("[NoteSliceVisualizer] Saving Config");
			string data = JsonConvert.SerializeObject(Config, Formatting.Indented);
			File.WriteAllText(ConfigFilePath, data);
		}
	}
}
