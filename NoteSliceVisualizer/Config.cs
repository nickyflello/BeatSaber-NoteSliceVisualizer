
namespace NoteSliceVisualizer
{
	public class Config
	{
		public struct Color
		{
			public Color(float r, float g, float b, float a = 1f)
			{
				R = r;
				G = g;
				B = b;
				A = a;
			}

			public UnityEngine.Color ToUnityColor()
			{
				return new UnityEngine.Color(R, G, B, A);
			}

			public float R;
			public float G;
			public float B;
			public float A;
		}

		public struct Vector3
		{
			public Vector3(float x, float y, float z)
			{
				X = x;
				Y = y;
				Z = z;
			}

			public UnityEngine.Vector3 ToUnityVector()
			{
				return new UnityEngine.Vector3(X, Y, Z);
			}

			public float X;
			public float Y;
			public float Z;
		}

		//public bool Enabled = true;
		//public bool UseDefaultNoteColors;

		//public Vector3 Position = new Vector3(0f, 0f, 18f);
		//public Vector3 Rotation = new Vector3(0f, 0f, 0f);
		//public float Scale = 1f;
		public float Alpha = 1f;

		public float PopDuration = 0.1f;
		public float DelayDuration = 0.1f;
		public float FadeDuration = 0.3f;

		public Color CutLineColor = new Color(0f, 0f, 0f);
		public float CutLineWidth = 1f;
		//public float CutLineScale;
		//public bool CutLineTriangle = false;

		//public bool DisplayGrid = false;
		//public float GridAlpha = 0.1f;

		//public float UISeperation = ?;
		//public bool Use2Notes = false;
		//public bool LogToFile = false;
	}
}
