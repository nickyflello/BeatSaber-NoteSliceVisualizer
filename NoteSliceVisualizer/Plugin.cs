using IPA;
using System;
using UnityEngine;

namespace NoteSliceVisualizer
{
	public class Plugin : IBeatSaberPlugin
	{
		ColorManager _colorManager;
		BeatmapObjectSpawnController _spawnController;

		SliceController[,] _sliceControllers;
		bool _logNotesCut = false;

		private void MenuSceneLoadedFresh()
		{
			Utilities.Initialize();
		}

		private void GameSceneLoaded()
		{
			_colorManager = GameObject.FindObjectOfType<ColorManager>();
			_spawnController = GameObject.FindObjectOfType<BeatmapObjectSpawnController>();
			_spawnController.noteWasCutEvent += OnNoteCut;

			Color colorNoteB = _colorManager.ColorForNoteType(NoteType.NoteB);

			GameObject[,] canvases = new GameObject[4, 3];
			_sliceControllers = new SliceController[4, 3];
			for (int x = 0; x < 4; ++x)
			{
				for (int y = 0; y < 3; ++y)
				{
					GameObject canvas = canvases[x, y];

					float xPos = -1.2f + (0.8f * x);
					float yPos = 0f + (0.8f * y) - 1.5f;

					canvas = AssetBundleHelper.Instantiate("Canvas");
					canvas.transform.localScale *= 0.25f;
					canvas.transform.Translate(xPos, yPos, 0f);
					_sliceControllers[x, y] = new SliceController(canvas);
				}
			}
		}

		private void OnNoteCut(BeatmapObjectSpawnController spawnController, INoteController noteController, NoteCutInfo info)
		{
			Vector3 center = noteController.noteTransform.position;
			Vector3 localCutPoint = info.cutPoint - center;

			NoteData data = noteController.noteData;
			SliceController sliceController = _sliceControllers[data.lineIndex, (int)data.noteLineLayer];
			sliceController.UpdateBlockColor(_colorManager.ColorForSaberType(info.saberType));
			sliceController.UpdateSlice(localCutPoint, info.cutNormal);

			if (_logNotesCut)
			{
				Console.WriteLine($"[CutVisualizer] OnNoteCut -------------------------------");
				Console.WriteLine($"[CutVisualizer] Center: ({center.x} {center.y})");
				Console.WriteLine($"[CutVisualizer] Cut Normal: ({info.cutNormal.x} {info.cutNormal.y})");
				Console.WriteLine($"[CutVisualizer] Cut Point: ({info.cutPoint.x} {info.cutPoint.y})");
				Console.WriteLine($"[CutVisualizer] Cut Local: ({localCutPoint.x} {localCutPoint.y})");
			}
		}

		#region IBeatSaberPlugin
		public void OnApplicationStart()
		{
			BS_Utils.Utilities.BSEvents.OnLoad();
			BS_Utils.Utilities.BSEvents.menuSceneLoadedFresh += MenuSceneLoadedFresh;
			BS_Utils.Utilities.BSEvents.gameSceneLoaded += GameSceneLoaded;
			AssetBundleHelper.LoadAssetBundle();
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
