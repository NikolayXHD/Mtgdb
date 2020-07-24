using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Mtgdb.Controls;
using NPlot;
using PlotSurface2D = NPlot.Windows.PlotSurface2D;

namespace Mtgdb.Gui
{
	public partial class FormChart
	{
		private void populateDataNPlot(
			List<string> arguments,
			List<string> series,
			List<List<object[]>> seriesValues,
			IList<object[]> argumentTotals,
			IList<object[]> seriesTotals,
			ReportSettings settings)
		{
			var chart = new PlotSurface2D
			{
				BackColor = SystemColors.Control,
				ForeColor = SystemColors.ControlText,
				PlotBackColor = SystemColors.Window,
				TitleColor = SystemColors.ControlText,
				SmoothingMode = SmoothingMode.HighQuality,
				Font = Font
			};
			replaceChart(chart);
			if (arguments.Count == 0)
				return;

			var colorTransformation = new ColorSchemeTransformation(null)
			{
				IgnoreSaturation = true,
				AsBackground = true,
			};

			var palette = _palette
				.Select(colorTransformation.TransformColor)
				.ToArray();

			var summaryFields = settings.SummaryFields;

			chart.Legend = new Legend
			{
				BackgroundColor = SystemColors.Window,
				TextColor = SystemColors.WindowText,
				BorderColor = SystemColors.ActiveBorder
			};

			chart.Legend.Font = chart.Legend.Font.ByDpi();
			chart.Add(new Grid
			{
				VerticalGridType = Grid.GridType.Fine,
				HorizontalGridType = Grid.GridType.None,
				MinorGridPen = new Pen(SystemColors.GrayText)
				{
					DashPattern = new[] { 1f, 5f }
				},
				MajorGridPen = new Pen(SystemColors.GrayText)
			});

			var seriesSummaryLegend = new string[series.Count];
			var argumentSummaryLegends = new List<string>[arguments.Count];
			for (int i = 0; i < arguments.Count; i++)
				argumentSummaryLegends[i] = new List<string>();

			var seriesList = new HistogramPlot[series.Count * summaryFields.Count];

			float totalBarWidth = 0.75f;
			float barWidth = totalBarWidth / summaryFields.Count;

			double minY = double.MaxValue;
			double maxY = double.MinValue;

			var dataLabels = new List<TextItem>();
			for (int k = 0; k < summaryFields.Count; k++)
			{
				float barOffset = (barWidth - totalBarWidth) * 0.5f + k * barWidth;

				string summaryFieldAlias = _fields.ByName[summaryFields[k]].Alias;
				string summaryFunctionAlias = Aggregates.Alias[settings.SummaryFunctions[k]];

				var stackedYValues = new double[arguments.Count];
				minY = Math.Min(0d, minY);
				maxY = Math.Max(0d, maxY);

				for (int j = 0; j < series.Count; j++)
				{
					int n = k * series.Count + j;

					Color color = palette[n % palette.Length];
					var chartSeries = new HistogramPlot
					{
						Filled = true,
						RectangleBrush = new RectangleBrushes.Solid(color),
						BaseWidth = barWidth,
						BaseOffset = barOffset,
						Color = color,
					};

					seriesList[n] = chartSeries;
					chart.Add(chartSeries);
					chartSeries.ShowInLegend = settings.ShowSeriesTotal;

					chartSeries.Label = series[j];
					var yValues = Enumerable.Range(0, arguments.Count)
						.Select(i => Convert.ToDouble(seriesValues[j][i][k] ?? 0))
						.ToList();

					chartSeries.DataSource = yValues;
					for (int i = 0; i < arguments.Count; i++)
						stackedYValues[i] += yValues[i];

					minY = Math.Min(minY, stackedYValues.Min());
					maxY = Math.Max(maxY, stackedYValues.Max());

					if (j > 0)
						chartSeries.StackedTo(seriesList[n - 1]);

					for (int i = 0; i < arguments.Count; i++)
					{
						double yValue = yValues[i];
						if (yValue < 0.009)
							continue;

						string text;
						switch (settings.LabelDataElement)
						{
							case DataElement.SummaryField:
								text = $"{yValue:0.##}: {summaryFieldAlias}";
								break;
							case DataElement.Series:
								text = $"{yValue:0.##}: {series[j]}";
								break;
							case DataElement.Argument:
								text = $"{yValue:0.##}: {arguments[i]}";
								break;
							case DataElement.SeriesAndArgument:
								text = $"{yValue:0.##}: {arguments[i]}, {series[j]}";
								break;
							case DataElement.Values:
								text = $"{yValue:0.##}";
								break;
							default:
								continue;
						}

						var labelY = stackedYValues[i] - yValue * 0.5;
						dataLabels.Add(new TextItem(
							new PointD(i + barOffset - 0.5f * barWidth + 0.05, labelY),
							text)
						{
							TextColor = SystemColors.ControlText,
						});
					}
				}

				// if (settings.ShowSeriesTotal)
				// 	for (int j = 0; j < series.Count; j++)
				// 	{
				// 		string legend;
				//
				// 		bool seriesSet = settings.SeriesFields.Count > 1 || settings.SeriesFields[0] != string.Empty;
				// 		string seriesName;
				// 		if (seriesSet)
				// 			seriesName = series[j];
				// 		else
				// 			seriesName = $"{summaryFunctionAlias} {summaryFieldAlias}";
				//
				// 		if (settings.ExplainTotal && seriesSet)
				// 			legend = $"{seriesName}: {seriesTotals[j][k]}, {summaryFunctionAlias} {summaryFieldAlias}";
				// 		else
				// 			legend = $"{seriesName}: {seriesTotals[j][k]}";
				//
				// 		if (metadata.CanDisplayMultipleSeries)
				// 			seriesList[k * series.Count + j].LegendText = legend;
				// 		else
				// 			seriesSummaryLegend[j] = legend;
				// 	}
				//
				// if (settings.ShowArgumentTotal)
				// 	for (int i = 0; i < arguments.Count; i++)
				// 	{
				// 		string legend;
				//
				// 		if (settings.ExplainTotal)
				// 			legend = $"{arguments[i]}: {argumentTotals[i][k]}, {summaryFunctionAlias} {summaryFieldAlias}";
				// 		else
				// 			legend = $"{arguments[i]}: {argumentTotals[i][k]}";
				//
				// 		argumentSummaryLegends[i].Add(legend);
				// 	}
			}

			int largeTickStep = getLargeTickStep(maxY);
			chart.YAxis1 = new LinearAxis
			{
				WorldMin = minY,
				WorldMax = maxY + 0.5,
				LargeTickStep = largeTickStep,
				NumberOfSmallTicks = Math.Min(4, largeTickStep - 1),
				TickTextFont = Font,
				Color = SystemColors.WindowText,
				LabelColor = SystemColors.WindowText,
				TickTextColor = SystemColors.WindowText,
			};
			var la = new LabelAxis
			{
				WorldMin = -0.5,
				WorldMax = arguments.Count - 0.5,
				TickTextFont = Font,
				Color = SystemColors.WindowText,
				LabelColor = SystemColors.WindowText,
				TickTextColor = SystemColors.WindowText,
				TickTextNextToAxis = false,
				TicksLabelAngle = 10f
			};
			chart.XAxis1 = la;
			if (settings.ShowArgumentTotal)
			{
				var argumentSummaries = argumentSummaryLegends
					.Select(legends => string.Join("\n", legends))
					.ToArray();
				for (int i = 0; i < arguments.Count; i++)
					la.AddLabel((arguments[i] + '\n' + argumentSummaries[i]).TrimEnd(), i);
			}
			else
				for (int i = 0; i < arguments.Count; i++)
					la.AddLabel(arguments[i], i);

			foreach (TextItem label in dataLabels)
				chart.Add(label);
		}

		private static int getLargeTickStep(double max)
		{
			double order = Math.Log10(max / 4);
			double ratio = (int) order;
			double remainder = order % 1;
			int q;
			if (remainder > Math.Log10(5))
				q = 5;
			else if (remainder > Math.Log10(2))
				q = 2;
			else
				q = 1;
			int largeTickStep = q * (int) Math.Pow(10, ratio);
			return largeTickStep;
		}

		private static readonly Color[] _palette =
		{
			Color.FromArgb(unchecked((int) 0xfff3a683)),
			Color.FromArgb(unchecked((int) 0xfff7d794)),
			Color.FromArgb(unchecked((int) 0xff778beb)),
			Color.FromArgb(unchecked((int) 0xffe77f67)),
			Color.FromArgb(unchecked((int) 0xffcf6a87)),
			Color.FromArgb(unchecked((int) 0xfff19066)),
			Color.FromArgb(unchecked((int) 0xfff5cd79)),
			Color.FromArgb(unchecked((int) 0xff546de5)),
			Color.FromArgb(unchecked((int) 0xffe15f41)),
			Color.FromArgb(unchecked((int) 0xffc44569)),
			Color.FromArgb(unchecked((int) 0xff786fa6)),
			Color.FromArgb(unchecked((int) 0xfff8a5c2)),
			Color.FromArgb(unchecked((int) 0xff63cdda)),
			Color.FromArgb(unchecked((int) 0xffea8685)),
			Color.FromArgb(unchecked((int) 0xff596275)),
			Color.FromArgb(unchecked((int) 0xff574b90)),
			Color.FromArgb(unchecked((int) 0xfff78fb3)),
			Color.FromArgb(unchecked((int) 0xff3dc1d3)),
			Color.FromArgb(unchecked((int) 0xffe66767)),
			Color.FromArgb(unchecked((int) 0xff303952)),
		};
	}
}
