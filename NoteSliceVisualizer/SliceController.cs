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
		private readonly RectTransform _maskTransform;
		private readonly RectTransform _sliceTransform;
		private readonly CanvasGroup _canvasGroup;
		private readonly RawImage _backgroundImage;
		private readonly RawImage _noteSliceImage;
		private readonly RawImage _maskLeftImage;
		private readonly RawImage _maskRightImage;
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
			_maskTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteMask");
			_sliceTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteSlice");
			_noteSliceImage = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteSliceImage").GetComponent<RawImage>();
			_maskLeftImage = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "MaskLeft").GetComponent<RawImage>();
			_maskRightImage = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "MaskRight").GetComponent<RawImage>();
			_canvasGroup = _blockTransform.GetComponent<CanvasGroup>();
			_uiScale = _blockTransform.rect.width * 1.1f;

			_backgroundImage = _blockTransform.GetComponent<RawImage>();
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
			//_backgroundColor = Color.white;
			//_backgroundImage.color = Color.white;
		}

		public void UpdateSlice(Vector3 cutPoint, Vector3 cutPlaneNormal)
		{
			cutPoint = new Vector3(cutPoint.x, cutPoint.y);
			Vector3 uiCutPoint = cutPoint * _uiScale;

			Vector3 cutDirection = new Vector3(-cutPlaneNormal.y, cutPlaneNormal.x).normalized;
			float uiRotation = Mathf.Atan2(cutDirection.y, cutDirection.x) * Mathf.Rad2Deg;

			_sliceTransform.localPosition = uiCutPoint;
			_sliceTransform.localRotation = Quaternion.Euler(0f, 0f, uiRotation);

			Vector3 lineNormal = new Vector3(cutPlaneNormal.x, cutPlaneNormal.y);
			float d = Vector3.Dot(lineNormal, -cutPoint);

			float alpha = Mathf.Clamp01(cutPoint.magnitude / 0.3f);

			//if (d > 0f)
			//{
			//	_maskLeftImage.color = new Color(_maskLeftImage.color.r, _maskLeftImage.color.g, _maskLeftImage.color.b, 0f);
			//	_maskRightImage.color = new Color(_maskRightImage.color.r, _maskRightImage.color.g, _maskRightImage.color.b, alpha);
			//}
			//else
			//{
			//	_maskLeftImage.color = new Color(_maskLeftImage.color.r, _maskLeftImage.color.g, _maskLeftImage.color.b, alpha);
			//	_maskRightImage.color = new Color(_maskRightImage.color.r, _maskRightImage.color.g, _maskRightImage.color.b, 0f);
			//}

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
