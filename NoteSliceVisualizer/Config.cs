
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

			public static implicit operator UnityEngine.Color(Color c)
				=> new UnityEngine.Color(c.R, c.G, c.B, c.A);

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

			public static implicit operator UnityEngine.Vector3(Vector3 v)
				=> new UnityEngine.Vector3(v.X, v.Y, v.Z);

			public float X;
			public float Y;
			public float Z;
		}

		public bool Enabled = true;
		public bool UseCustomNoteColors = true;

		public Vector3 Position = new Vector3(0f, 1.5f, 16f);
		public Vector3 Rotation = new Vector3(0f, 0f, 0f);
		public float Scale = 1f;
		public float Alpha = 1f;

		public float PopDuration = 0.1f;
		public float DelayDuration = 0.1f;
		public float FadeDuration = 0.3f;

		public Color CutLineColor = new Color(1f, 1f, 1f);
		public float CutLineWidth = 1f;
		public float CutLineLengthScale = 1f;
		public bool CutLineUseTriangleTexture = true;

		//public bool DisplayGrid = false;
		//public float GridAlpha = 0.1f;

		public float Separation = 1f;
		public bool TwoNoteMode = false;
		//public bool LogToFile = false;
	}
}
