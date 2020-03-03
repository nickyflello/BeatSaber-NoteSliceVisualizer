using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NoteSliceVisualizer
{
	public class SliceController
	{
		private readonly float _uiScale;
		private readonly RectTransform _blockTransform;
		private readonly RectTransform _blockMaskTransform;
		private readonly RectTransform _noteMaskTransform;
		private readonly RectTransform _noteMaskInverseTransform;
		private readonly RectTransform _sliceTransform;
		private readonly RectTransform _cutOffsetTransform;
		private readonly CanvasGroup _canvasGroup;
		private readonly RawImage _backgroundImage;
		private readonly RawImage _cutSliceImage;
		private readonly RawImage _cutOffsetImage;
		private Color _backgroundColor;
		private float _timeSinceSliced;

		private readonly Texture _cutLineTexture = ConfigHelper.Config.CutLineUseTriangleTexture ? AssetBundleHelper.TriangleTexture : null;
		private readonly Color _cutLineColor = ConfigHelper.Config.CutLineColor;
		private readonly Color _cutOffsetColor = ConfigHelper.Config.CutOffsetColor;
		private readonly float _cutLineWidth = ConfigHelper.Config.CutLineWidth;
		private readonly float _cutLineLengthScale = ConfigHelper.Config.CutLineLengthScale;
		private readonly float _cutLineSensitivity = ConfigHelper.Config.CutLineSensitivity;

		private readonly bool _shouldUpdateColor = !ConfigHelper.Config.TwoNoteMode;
		private readonly float _maxAlpha = ConfigHelper.Config.Alpha;
		private readonly float _popDuration = ConfigHelper.Config.PopDuration;
		private readonly float _delayDuration = ConfigHelper.Config.DelayDuration;
		private readonly float _fadeDuration = ConfigHelper.Config.FadeDuration;

		public SliceController(GameObject canvas)
		{
			_blockTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "Block");
			_blockMaskTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "BlockMask");
			_noteMaskTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteMask");
			_noteMaskInverseTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteMaskInverse");
			_sliceTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteSlice");
			_cutOffsetTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "MissedAreaImage");
			_canvasGroup = canvas.GetComponent<CanvasGroup>();
			_cutSliceImage = _sliceTransform.GetComponent<RawImage>();
			_cutOffsetImage = _cutOffsetTransform.GetComponent<RawImage>();
			_uiScale = _blockTransform.rect.width * 1.1f;

			_backgroundImage = _blockTransform.GetComponent<RawImage>();
			_cutSliceImage.texture = _cutLineTexture;
			_cutSliceImage.color = _cutLineColor;
			_cutOffsetImage.color = _cutOffsetColor;

			_noteMaskTransform.sizeDelta *= _cutLineLengthScale;
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
			//_backgroundColor = Color.white;
			//_backgroundImage.color = Color.white;
		}

		public void UpdateSlice(Vector3 cutPoint, Vector3 cutPlaneNormal, float noteRotation)
		{
			cutPoint = new Vector3(cutPoint.x, cutPoint.y);
			Vector3 uiCutPoint = cutPoint * _uiScale * _cutLineSensitivity;

			Vector3 cutDirection = new Vector3(-cutPlaneNormal.y, cutPlaneNormal.x).normalized;
			float cutRotation = Mathf.Atan2(cutDirection.y, cutDirection.x) * Mathf.Rad2Deg;

			_blockTransform.localRotation = Quaternion.Euler(0f, 0f, noteRotation);
			_blockMaskTransform.localRotation = Quaternion.Euler(0f, 0f, noteRotation);
			_noteMaskTransform.localRotation = Quaternion.Euler(0f, 0f, noteRotation);
			_noteMaskInverseTransform.localRotation = Quaternion.Euler(0f, 0f, -noteRotation);

			_sliceTransform.localPosition = uiCutPoint;
			_sliceTransform.localRotation = Quaternion.Euler(0f, 0f, cutRotation);

			Vector3 lineNormal = new Vector3(cutPlaneNormal.x, cutPlaneNormal.y);
			float d = Vector3.Dot(lineNormal, -cutPoint);
			//float alpha = Mathf.Clamp01(cutPointDistance / 0.3f);
			float cutPointDistance = cutPoint.magnitude;
			float missedAreaRotation = cutRotation - noteRotation + ((d > 0f) ?  180f : 0f);

			_cutOffsetTransform.sizeDelta = new Vector2(_cutOffsetTransform.rect.width, cutPointDistance * _uiScale * _cutLineSensitivity);
			_cutOffsetTransform.localRotation = Quaternion.Euler(0f, 0f, missedAreaRotation);

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
