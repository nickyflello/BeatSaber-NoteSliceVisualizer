using BeatSaberMarkupLanguage.Attributes;
using UnityEngine;

namespace NoteSliceVisualizer
{
	class SettingsUI : PersistentSingleton<SettingsUI>
	{
		private Config Config => ConfigHelper.Config;

		[UIValue("boolEnable")]
		public bool Enabled
		{
			get => Config.Enabled;
			set => Config.Enabled = value;
		}

		[UIValue("boolUseCustomNoteColors")]
		public bool CustomNoteColors
		{
			get => Config.UseCustomNoteColors;
			set => Config.Enabled = value;
		}

		[UIValue("sliderPositionX")]
		public float PositionX
		{
			get => Config.Position.X;
			set => Config.Position.X = value;
		}

		[UIValue("sliderPositionY")]
		public float PositionY
		{
			get => Config.Position.Y;
			set => Config.Position.Y = value;
		}

		[UIValue("sliderPositionZ")]
		public float PositionZ
		{
			get => Config.Position.Z;
			set => Config.Position.Z = value;
		}

		[UIValue("sliderRotationX")]
		public float RotationX
		{
			get => Config.Rotation.X;
			set => Config.Rotation.X = value;
		}

		[UIValue("sliderRotationY")]
		public float RotationY
		{
			get => Config.Rotation.Y;
			set => Config.Rotation.Y = value;
		}

		[UIValue("sliderRotationZ")]
		public float RotationZ
		{
			get => Config.Rotation.Z;
			set => Config.Rotation.Z = value;
		}

		[UIValue("sliderScale")]
		public float Scale
		{
			get => Config.Scale;
			set => Config.Scale = value;
		}

		[UIValue("sliderAlpha")]
		public float Alpha
		{
			get => Config.Alpha;
			set => Config.Alpha = value;
		}

		[UIValue("sliderPopDuration")]
		public float PopDuration
		{
			get => Config.PopDuration;
			set => Config.PopDuration = value;
		}

		[UIValue("sliderDelayDuration")]
		public float DelayDuration
		{
			get => Config.DelayDuration;
			set => Config.DelayDuration = value;
		}

		[UIValue("sliderFadeDuration")]
		public float FadeDuration
		{
			get => Config.FadeDuration;
			set => Config.FadeDuration = value;
		}

		[UIValue("colorCutLineColor")]
		public Color CutLineColor
		{
			get => Config.CutLineColor;
			set => Config.CutLineColor = new Config.Color(value.r, value.g, value.b, value.a);
		}



		[UIAction("#apply")]
		public void OnApply()
		{
			ConfigHelper.SaveConfig();
		}

		[UIAction("#ok")]
		public void OnOk()
		{
			ConfigHelper.SaveConfig();
		}
	}
}
