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

		public SliceController(GameObject canvas, Color color)
		{
			_sliceTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "NoteSlice");
			_blockTransform = canvas.GetComponentsInChildren<RectTransform>().First(o => o.name == "Block");
			_scale = _blockTransform.rect.width;

			_blockTransform.GetComponent<RawImage>().color = color;
		}

		public void UpdateSlice(Vector3 cutPoint, Vector3 normal)
		{
			cutPoint = new Vector3(cutPoint.x, cutPoint.y);
			normal = new Vector3(-normal.y, normal.x).normalized;

			Vector3 start = (cutPoint + normal) * _scale * 1.1f;
			float rot = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;

			_sliceTransform.localPosition = start;
			_sliceTransform.localRotation = Quaternion.Euler(0, 0, rot);
		}
	}
}
