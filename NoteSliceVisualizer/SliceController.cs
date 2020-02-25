using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NoteSliceVisualizer
{
	class SliceController 
	{
		private float _scale;
		private RectTransform _blockTransform;
		private RectTransform _sliceTransform;
		private RawImage _backgroundImage;
		private RawImage _noteSliceImage;
		private Color _backgroundColor;

		private const float _fadeTime = 0.3f;
		private const float _popTime = 0.1f;
		private float _timeSinceSliced = _fadeTime;

		public SliceController(GameObject canvas)
		{
			_blockTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "Block");
			_sliceTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteSlice");
			_scale = _blockTransform.rect.width;

			_backgroundImage = _blockTransform.GetComponent<RawImage>();
			_noteSliceImage = _sliceTransform.GetComponent<RawImage>();
			_noteSliceImage.color = Color.white;
		}

		public void UpdateBlockColor(Color color)
		{
			_backgroundColor = color;
		}

		public void UpdateSlice(Vector3 cutPoint, Vector3 normal)
		{
			cutPoint = new Vector3(cutPoint.x, cutPoint.y);
			normal = new Vector3(-normal.y, normal.x).normalized;

			Vector3 start = (cutPoint + normal) * _scale * 1.1f;
			float rot = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;

			_sliceTransform.localPosition = start;
			_sliceTransform.localRotation = Quaternion.Euler(0f, 0f, rot);

			_timeSinceSliced = 0f;
		}

		public void Update()
		{
			float popT = _timeSinceSliced / _popTime;
			float fadeT = _timeSinceSliced / _fadeTime;

			float r = Mathf.Lerp(_backgroundColor.r * 10, _backgroundColor.r, popT);
			float g = Mathf.Lerp(_backgroundColor.g * 10, _backgroundColor.g, popT);
			float b = Mathf.Lerp(_backgroundColor.b * 10, _backgroundColor.b, popT);
			float a = Mathf.Lerp(1f, 0f, fadeT);

			Color color = new Color(r, g, b, a);
			_backgroundImage.color = color;

			_timeSinceSliced += Time.deltaTime;
		}
	}
}
