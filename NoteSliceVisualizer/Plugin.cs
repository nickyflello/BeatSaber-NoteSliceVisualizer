using IPA;
using System;
using UnityEngine;

namespace NoteSliceVisualizer
{
	public class Plugin : IBeatSaberPlugin
	{
		ColorManager _colorManager;
		BeatmapObjectSpawnController _spawnController;

		SliceController[] _sliceControllers;
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

			GameObject canvasA = AssetBundleHelper.Instantiate("Canvas");
			GameObject canvasB = AssetBundleHelper.Instantiate("Canvas");

			canvasA.transform.Translate(-1.5f, 0f, 0f);
			canvasB.transform.Translate(1.5f, 0f, 0f);

			Color colorNoteA = _colorManager.ColorForNoteType(NoteType.NoteA);
			Color colorNoteB = _colorManager.ColorForNoteType(NoteType.NoteB);

			Console.WriteLine($"[NoteSliceVisualizer] Color NoteA: {colorNoteA}");
			Console.WriteLine($"[NoteSliceVisualizer] Color NoteB: {colorNoteB}");

			_sliceControllers = new SliceController[]
			{
				new SliceController(canvasA, colorNoteA),
				new SliceController(canvasB, colorNoteB),
			};
		}

		private void OnNoteCut(BeatmapObjectSpawnController spawnController, INoteController noteController, NoteCutInfo info)
		{
			Vector3 center = noteController.noteTransform.position;
			Vector3 localCutPoint = info.cutPoint - center;
			_sliceControllers[(int)info.saberType].UpdateSlice(localCutPoint, info.cutNormal);

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
		}

		public void OnFixedUpdate()
		{
		}
		#endregion // IBeatSaberPlugin
	}
}
