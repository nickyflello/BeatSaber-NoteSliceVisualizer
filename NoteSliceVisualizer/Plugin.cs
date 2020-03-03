using IPA;
using System;
using UnityEngine;

namespace NoteSliceVisualizer
{
	public class Plugin : IBeatSaberPlugin
	{
		ColorManager _colorManager;
		BeatmapObjectSpawnController _spawnController;

		Transform _sliceParent;
		SliceController[] _sliceControllers;
		bool _logNotesCut = false;

		private static readonly Color[] _defaultColors = new Color[]
		{
			new Color(0.7843137f, 0.07843138f, 0.07843138f),
			new Color(0f, 0.4627451f, 0.8235294f)
		};

		static bool UseCustomNoteColors => ConfigHelper.Config.UseCustomNoteColors;
		static Vector3 Position => ConfigHelper.Config.Position;
		static Vector3 Rotation => ConfigHelper.Config.Rotation;
		static float Scale => ConfigHelper.Config.Scale * (ConfigHelper.Config.TwoNoteMode ? 4 : 1);
		static float Separation => ConfigHelper.Config.Separation * 0.8f; // x0.8 to have 1.0 as the default config
		static bool TwoNoteMode => ConfigHelper.Config.TwoNoteMode;

		private void MenuSceneLoadedFresh()
		{
			Utilities.Initialize();
			AssetBundleHelper.LoadAssetBundle();
		}

		private void GameSceneLoaded()
		{
			if (!ConfigHelper.Config.Enabled)
			{
				return;
			}

			_colorManager = GameObject.FindObjectOfType<ColorManager>();
			_spawnController = GameObject.FindObjectOfType<BeatmapObjectSpawnController>();
			_spawnController.noteWasCutEvent += OnNoteCut;

			_sliceParent = new GameObject("Slice Parent").transform;
			
			if (TwoNoteMode)
			{
				_sliceControllers = new SliceController[2];
				for (int x = 0; x < 2; ++x)
				{
					int index = x;
					float posX = -0.4f + (Separation * x);
					float posY = 1.5f;

					SliceController controller = CreateSliceController(posX, posY);
					Color color = UseCustomNoteColors ? _colorManager.ColorForNoteType((NoteType)index) : _defaultColors[index];
					controller.UpdateBlockColor(color);
					_sliceControllers[index] = controller;
				}
			}
			else
			{
				_sliceControllers = new SliceController[12];
				for (int x = 0; x < 4; ++x)
				{
					for (int y = 0; y < 3; ++y)
					{
						int index = 3 * x + y;
						float posX = -1.2f + (Separation * x);
						float posY = (Separation * y);

						_sliceControllers[index] = CreateSliceController(posX, posY);
					}
				}
			}

			_sliceParent.localPosition = Position;
			_sliceParent.eulerAngles = Rotation;
			_sliceParent.localScale *= Scale;
		}

		private SliceController CreateSliceController(float posX, float posY)
		{
			GameObject canvas = GameObject.Instantiate(AssetBundleHelper.Canvas);
			canvas.transform.parent = _sliceParent.transform;
			canvas.transform.localPosition = new Vector3(posX, posY);
			return new SliceController(canvas);
		}

		private void OnNoteCut(BeatmapObjectSpawnController spawnController, INoteController noteController, NoteCutInfo info)
		{
			NoteData data = noteController.noteData;
			if (ShouldDisplayNote(data))
			{
				Vector3 center = noteController.noteTransform.position;
				Vector3 localCutPoint = info.cutPoint - center;
				float rotation = noteController.noteTransform.eulerAngles.z;

				if (TwoNoteMode)
				{
					int index = (int)info.saberType;
					SliceController sliceController = _sliceControllers[index];
					sliceController.UpdateSlice(localCutPoint, info.cutNormal, rotation);
				}
				else
				{
					int index = 3 * data.lineIndex + (int)data.noteLineLayer;
					SliceController sliceController = _sliceControllers[index];

					Color color = UseCustomNoteColors ? _colorManager.ColorForSaberType(info.saberType) : _defaultColors[(int)info.saberType];
					sliceController.UpdateBlockColor(color);
					sliceController.UpdateSlice(localCutPoint, info.cutNormal, rotation);
				}

				if (_logNotesCut)
				{
					Console.WriteLine($"[CutVisualizer] OnNoteCut -------------------------------");
					Console.WriteLine($"[CutVisualizer] Center: ({center.x} {center.y})");
					Console.WriteLine($"[CutVisualizer] Cut Normal: ({info.cutNormal.x} {info.cutNormal.y})");
					Console.WriteLine($"[CutVisualizer] Cut Point: ({info.cutPoint.x} {info.cutPoint.y})");
					Console.WriteLine($"[CutVisualizer] Cut Local: ({localCutPoint.x} {localCutPoint.y})");
				}
			}
		}

		private bool ShouldDisplayNote(NoteData data)
		{
			int lineIndex = data.lineIndex;
			int layer = (int)data.noteLineLayer;

			// Mapping Extensions may place notes beyond the 12 note array. Ignore these.
			return TwoNoteMode ||
				(lineIndex >= 0 &&
				lineIndex <= 3 &&
				layer >= 0 &&
				layer <= 2);
		}

		#region IBeatSaberPlugin
		public void OnApplicationStart()
		{
			BS_Utils.Utilities.BSEvents.OnLoad();
			BS_Utils.Utilities.BSEvents.menuSceneLoadedFresh += MenuSceneLoadedFresh;
			BS_Utils.Utilities.BSEvents.gameSceneLoaded += GameSceneLoaded;
			ConfigHelper.LoadConfig();
		}

		public void OnApplicationQuit()
		{
			BS_Utils.Utilities.BSEvents.gameSceneLoaded -= GameSceneLoaded;
		}

		public void OnSceneLoaded(global::UnityEngine.SceneManagement.Scene scene, global::UnityEngine.SceneManagement.LoadSceneMode sceneMode)
		{
		}

		public void OnSceneUnloaded(global::UnityEngine.SceneManagement.Scene scene)
		{
			Console.WriteLine($"SCENE LOADED: {scene.name}");
		}

		public void OnActiveSceneChanged(global::UnityEngine.SceneManagement.Scene prevScene, global::UnityEngine.SceneManagement.Scene nextScene)
		{
		}

		public void OnUpdate()
		{
			if (_sliceControllers != null)
			{
				foreach (SliceController sliceController in _sliceControllers)
				{
					sliceController?.Update();
				}
			}
		}

		public void OnFixedUpdate()
		{
		}

		#endregion // IBeatSaberPlugin
	}
}
