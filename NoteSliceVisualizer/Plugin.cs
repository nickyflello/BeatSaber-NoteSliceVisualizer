using BS_Utils.Utilities;
using IPA;
using System;
using System.Linq;
using UnityEngine;

namespace NoteSliceVisualizer
{
	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		ColorManager _colorManager;
		BeatmapObjectManager _spawnController;

		Transform _parentCanvas;
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

		private void MenuSceneLoadedFresh(ScenesTransitionSetupDataSO scenesTransitionSetupDataSO)
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
			_spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectExecutionRatingsRecorder>().LastOrDefault().GetPrivateField<BeatmapObjectManager>("_beatmapObjectManager");//GameObject.FindObjectOfType<BeatmapObjectManager>();
			_spawnController.noteWasCutEvent += OnNoteCut;

			_parentCanvas = GameObject.Instantiate(AssetBundleHelper.Canvas).transform;

			if (TwoNoteMode)
			{
				_sliceControllers = new SliceController[2];
				float minX = -Separation * 0.5f;
				for (int x = 0; x < 2; ++x)
				{
					int index = x;
					float posX = minX + (Separation * x);
					float posY = 0f;

					SliceController controller = CreateSliceController(posX, posY);
					Color color = UseCustomNoteColors ? _colorManager.ColorForType((ColorType)index) : _defaultColors[index];
					controller.UpdateBlockColor(color);
					_sliceControllers[index] = controller;
				}
			}
			else
			{
				_sliceControllers = new SliceController[12];
				float minX = -Separation * 1.5f;
				for (int x = 0; x < 4; ++x)
				{
					for (int y = 0; y < 3; ++y)
					{
						int index = 3 * x + y;
						float posX = minX + (Separation * x);
						float posY = (Separation * y);

						_sliceControllers[index] = CreateSliceController(posX, posY);
					}
				}
			}

			_parentCanvas.localPosition = Position;
			_parentCanvas.eulerAngles = Rotation;
			_parentCanvas.localScale *= Scale;
		}

		private SliceController CreateSliceController(float posX, float posY)
		{
			GameObject slicedNoteUI = GameObject.Instantiate(AssetBundleHelper.NoteUI);
			slicedNoteUI.transform.SetParent(_parentCanvas.transform, false);
			slicedNoteUI.transform.localPosition = new Vector3(posX, posY);
			return slicedNoteUI.AddComponent<SliceController>();
		}

		private void OnNoteCut(NoteController noteController, NoteCutInfo info)
		{
			NoteData data = noteController.noteData;
			if (ShouldDisplayNote(data, info))
			{
				Vector3 center = noteController.noteTransform.position;
				Vector3 localCutPoint = info.cutPoint - center;
				NoteCutDirection directionType = data.cutDirection;

				if (TwoNoteMode)
				{
					int index = (int)info.saberType;
					SliceController sliceController = _sliceControllers[index];
					sliceController.UpdateSlice(localCutPoint, info.cutNormal, directionType);
				}
				else
				{
					int index = 3 * data.lineIndex + (int)data.noteLineLayer;
					SliceController sliceController = _sliceControllers[index];

					Color color = UseCustomNoteColors ? _colorManager.ColorForSaberType(info.saberType) : _defaultColors[(int)info.saberType];
					sliceController.UpdateBlockColor(color);
					sliceController.UpdateSlice(localCutPoint, info.cutNormal, directionType);
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

		private bool ShouldDisplayNote(NoteData data, NoteCutInfo cutInfo)
		{
			int lineIndex = data.lineIndex;
			int layer = (int)data.noteLineLayer;

			// Mapping Extensions may place notes beyond the 12 note array. Ignore these.
			return cutInfo.allIsOK &&
				(TwoNoteMode ||
				(lineIndex >= 0 &&
				lineIndex <= 3 &&
				layer >= 0 &&
				layer <= 2));
		}

		[OnStart]
		public void OnApplicationStart()
		{
			BS_Utils.Utilities.BSEvents.OnLoad();
			BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += MenuSceneLoadedFresh;
			BS_Utils.Utilities.BSEvents.gameSceneLoaded += GameSceneLoaded;
			ConfigHelper.LoadConfig();
		}

		[OnExit]
		public void OnApplicationQuit()
		{
			BS_Utils.Utilities.BSEvents.gameSceneLoaded -= GameSceneLoaded;
			BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh -= MenuSceneLoadedFresh;
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

			// Reload config
			if (Input.GetKeyDown(KeyCode.F5))
			{
				ConfigHelper.LoadConfig();
			}
		}

	}
}
