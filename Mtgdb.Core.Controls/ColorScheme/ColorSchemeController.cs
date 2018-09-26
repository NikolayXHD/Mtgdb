using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public class ColorSchemeController
	{
		public static event Action SystemColorsChanging;

		public ColorSchemeController()
		{
			// force init color table
			byte unused = SystemColors.Window.R;

			_colorTableField = typeof(Color).Assembly.GetType("System.Drawing.KnownColorTable")
				.GetField("colorTable", BindingFlags.Static | BindingFlags.NonPublic);

			_colorTable = readColorTable();
			SystemEvents.UserPreferenceChanging += userPreferenceChanging;

			OriginalColors = Save();
		}

		private void userPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
		{
			if (e.Category != UserPreferenceCategory.Color)
				return;

			_colorTable = readColorTable();
			SystemColorsChanging?.Invoke();
		}

		private int[] readColorTable() =>
			(int[]) _colorTableField.GetValue(null);

		public void SetColor(KnownColor knownColor, int argb)
		{
			setColor(knownColor, argb);
			SystemColorsChanging?.Invoke();
		}

		private void setColor(KnownColor knownColor, int argb)
		{
			if (!KnownColors.Contains(knownColor))
				throw new ArgumentException();

			_colorTable[(int) knownColor] = argb;
		}

		public int GetOriginalColor(KnownColor knownColor)
		{
			if (!KnownColors.Contains(knownColor))
				throw new ArgumentException();

			return OriginalColors[(int) knownColor];
		}

		public int GetColor(KnownColor knownColor)
		{
			if (!KnownColors.Contains(knownColor))
				throw new ArgumentException();

			return _colorTable[(int) knownColor];
		}

		public IReadOnlyDictionary<int, int> Save() =>
			KnownColors.Cast<int>()
				.ToDictionary(i => i, i => _colorTable[i])
				.AsReadOnlyDictionary();

		public void Load(IReadOnlyDictionary<int, int> saved)
		{
			foreach (var pair in saved)
				setColor((KnownColor) pair.Key, pair.Value);

			SystemColorsChanging?.Invoke();
		}

		public void Reset(KnownColor color) =>
			SetColor(color, OriginalColors[(int) color]);

		public void ResetAll() =>
			Load(OriginalColors);

		public readonly HashSet<KnownColor> KnownColors = new HashSet<KnownColor>(
			new[]
			{
				SystemColors.Control,

				SystemColors.Window,
				SystemColors.WindowText,
				SystemColors.GrayText,

				SystemColors.HotTrack,
				SystemColors.Highlight,
				SystemColors.HighlightText,

				SystemColors.ActiveCaption,
				SystemColors.GradientActiveCaption,

				SystemColors.InactiveCaption,
				SystemColors.GradientInactiveCaption,

				SystemColors.ActiveBorder
			}.Select(_ => _.ToKnownColor())
		);

		private int[] _colorTable;
		private readonly FieldInfo _colorTableField;

		private IReadOnlyDictionary<int, int> OriginalColors { get; }
	}
}