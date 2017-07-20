using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms.DataVisualization.Charting;

namespace Mtgdb.Gui
{
	public static class ChartPalettes
	{
		private static readonly Type ChartPaletteColorsType = typeof (SeriesChartType).Assembly.GetType(
			"System.Windows.Forms.DataVisualization.Charting.Utilities.ChartPaletteColors");

		private static readonly MethodInfo GetPaletteColorsMethod = ChartPaletteColorsType.GetMethod("GetPaletteColors");

		public static Dictionary<ChartColorPalette, Color[]> ByName = Enum.GetValues(typeof (ChartColorPalette))
			.Cast<ChartColorPalette>()
			.Where(_ => _ != ChartColorPalette.None)
			.ToDictionary(p => p, getPalette);

		private static Color[] getPalette(ChartColorPalette palette)
		{
			var result = (Color[]) GetPaletteColorsMethod.Invoke(null, new object[] { palette });
			return result;
		}
	}
}