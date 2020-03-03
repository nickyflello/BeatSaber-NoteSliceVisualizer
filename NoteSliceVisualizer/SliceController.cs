using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NoteSliceVisualizer
{
	public class SliceController
	{
		private readonly float _uiScale;
		private RectTransform _blockTransform;
		private RectTransform _maskTransform;
		private RectTransform _sliceTransform;
		private CanvasGroup _canvasGroup;
		private RawImage _backgroundImage;
		private RawImage _noteSliceImage;
		private Color _backgroundColor;
		private float _timeSinceSliced;

		private readonly Texture _cutLineTexture = ConfigHelper.Config.CutLineUseTriangleTexture ? AssetBundleHelper.TriangleTexture : null;
		private readonly Color _cutLineColor = ConfigHelper.Config.CutLineColor;
		private readonly float _cutLineWidth = ConfigHelper.Config.CutLineWidth;
		private readonly float _cutLineLengthScale = ConfigHelper.Config.CutLineLengthScale;

		private readonly bool _shouldUpdateColor = !ConfigHelper.Config.TwoNoteMode;
		private readonly float _maxAlpha = ConfigHelper.Config.Alpha;
		private readonly float _popDuration = ConfigHelper.Config.PopDuration;
		private readonly float _delayDuration = ConfigHelper.Config.DelayDuration;
		private readonly float _fadeDuration = ConfigHelper.Config.FadeDuration;

		public SliceController(GameObject canvas)
		{
			_blockTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "Block");
			_maskTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "Mask");
			_sliceTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteSlice");
			_canvasGroup = _blockTransform.GetComponent<CanvasGroup>();
			_uiScale = _blockTransform.rect.width;

			_backgroundImage = _blockTransform.GetComponent<RawImage>();
			_noteSliceImage = _sliceTransform.GetComponent<RawImage>();
			_noteSliceImage.texture = _cutLineTexture;
			_noteSliceImage.color = _cutLineColor;

			_maskTransform.sizeDelta *= _cutLineLengthScale;
			float cutLineHeight = _sliceTransform.sizeDelta.x * _cutLineLengthScale;
			_sliceTransform.sizeDelta = new Vector2(cutLineHeight, _cutLineWidth);

			if (_popDuration <= 0) _popDuration = 0.001f;
			if (_delayDuration <= 0) _delayDuration = 0.001f;
			if (_fadeDuration <= 0) _fadeDuration = 0.001f;
			_timeSinceSliced = _fadeDuration + _delayDuration;
		}

		public void UpdateBlockColor(Color color)
		{
			_backgroundColor = color;
			_backgroundImage.color = color;
		}

		public void UpdateSlice(Vector3 cutPoint, Vector3 normal)
		{
			cutPoint = new Vector3(cutPoint.x, cutPoint.y);
			normal = new Vector3(-normal.y, normal.x).normalized;

			Vector3 start = cutPoint * _uiScale * 1.1f;
			float rot = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;

			_sliceTransform.localPosition = start;
			_sliceTransform.localRotation = Quaternion.Euler(0f, 0f, rot);

			_timeSinceSliced = 0f;
		}

		public void Update()
		{
			if (_shouldUpdateColor && _canvasGroup != null)
			{
				float popT = _timeSinceSliced / _popDuration;
				float fadeT = (_timeSinceSliced - _delayDuration) / _fadeDuration;

				float pop = Mathf.Lerp(10f, 1f, popT);
				float a = Mathf.Lerp(_maxAlpha, 0f, fadeT);

				_backgroundImage.color = _backgroundColor * pop;
				_canvasGroup.alpha = a;

				_timeSinceSliced += Time.deltaTime;
			}
		}
	}
}
