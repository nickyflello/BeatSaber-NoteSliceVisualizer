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
		private RectTransform _sliceTransform;
		private RawImage _backgroundImage;
		private RawImage _noteSliceImage;
		private Color _backgroundColor;

		private readonly float _popDuration = ConfigHelper.Config.PopDuration;
		private readonly float _delayDuration = ConfigHelper.Config.DelayDuration;
		private readonly float _fadeDuration = ConfigHelper.Config.FadeDuration;
		private float _timeSinceSliced;

		public SliceController(GameObject canvas)
		{
			_blockTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "Block");
			_sliceTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteSlice");
			_uiScale = _blockTransform.rect.width;

			_backgroundImage = _blockTransform.GetComponent<RawImage>();
			_noteSliceImage = _sliceTransform.GetComponent<RawImage>();
			_noteSliceImage.color = ConfigHelper.Config.CutLineColor.ToUnityColor();
			_sliceTransform.sizeDelta = new Vector2(_sliceTransform.sizeDelta.x, ConfigHelper.Config.CutLineWidth);

			if (_popDuration <= 0) _popDuration = 0.001f;
			if (_delayDuration <= 0) _delayDuration = 0.001f;
			if (_fadeDuration <= 0) _fadeDuration = 0.001f;
			_timeSinceSliced = _fadeDuration + _delayDuration;
		}

		public void UpdateBlockColor(Color color)
		{
			_backgroundColor = color;
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
			float popT = _timeSinceSliced / _popDuration;
			float fadeT = (_timeSinceSliced - _delayDuration) / _fadeDuration;

			float r = Mathf.Lerp(_backgroundColor.r * 10, _backgroundColor.r, popT);
			float g = Mathf.Lerp(_backgroundColor.g * 10, _backgroundColor.g, popT);
			float b = Mathf.Lerp(_backgroundColor.b * 10, _backgroundColor.b, popT);
			float a = Mathf.Lerp(ConfigHelper.Config.Alpha, 0f, fadeT);

			Color color = new Color(r, g, b, a);
			_backgroundImage.color = color;

			_timeSinceSliced += Time.deltaTime;
		}
	}
}
