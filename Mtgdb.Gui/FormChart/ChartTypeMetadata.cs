using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace Mtgdb.Gui
{
	public class ChartTypeMetadata
	{
		private static readonly Type[] _pointChartTypes =
			Runtime.IsMono
				? Array.Empty<Type>()
				: new[]
				{
					findChartType(SeriesChartType.Point),
					findChartType(SeriesChartType.FastLine),
					findChartType(SeriesChartType.FastPoint)
				};

		private static readonly Type[] _pieChartTypes =
			Runtime.IsMono
				? Array.Empty<Type>()
				: new[] { findChartType(SeriesChartType.Pie) };

		public static readonly Dictionary<SeriesChartType, ChartTypeMetadata> ByType =
			Runtime.IsMono
				? new Dictionary<SeriesChartType, ChartTypeMetadata>()
				: Enum.GetValues(typeof(SeriesChartType))
					.Cast<SeriesChartType>()
					.ToDictionary(_ => _, create);

		private static ChartTypeMetadata create(SeriesChartType chartType)
		{
			var type = findChartType(chartType);

			var instance = Activator.CreateInstance(type);

			var result = new ChartTypeMetadata
			{
				Stacked = (bool) type.GetProperty("Stacked").GetValue(instance, null),
				SupportStackedGroups = (bool) type.GetProperty("SupportStackedGroups").GetValue(instance, null),
				CircularChartArea = (bool) type.GetProperty("CircularChartArea").GetValue(instance, null),
				SwitchValueAxes = (bool) type.GetProperty("SwitchValueAxes").GetValue(instance, null),
				SideBySideSeries = (bool) type.GetProperty("SideBySideSeries").GetValue(instance, null),
				StackSign = (bool) type.GetProperty("StackSign").GetValue(instance, null),
				RequireAxes = (bool) type.GetProperty("RequireAxes").GetValue(instance, null),
				HundredPercent = (bool) type.GetProperty("HundredPercent").GetValue(instance, null),

				YValuesPerPoint = (int) type.GetProperty("YValuesPerPoint").GetValue(instance, null),
				IsPointChart = _pointChartTypes.Any(_ => _.IsAssignableFrom(type)),
				IsPieChart = _pieChartTypes.Any(_ => _.IsAssignableFrom(type))
			};

			result.CanDisplayMultipleSeries =
				result.SideBySideSeries || result.Stacked || result.IsPointChart && chartType != SeriesChartType.Kagi;

			return result;
		}

		private static Type findChartType(SeriesChartType chartType)
		{
			var assembly = typeof(SeriesChartType).Assembly;

			string typeName = $"System.Windows.Forms.DataVisualization.Charting.ChartTypes.{chartType}Chart";

			var type = assembly.GetType(typeName, false, true) ??
			           assembly.GetType(typeName.Replace(@"100", string.Empty), false, true);
			return type;
		}

		public bool Stacked { get; private set; }
		public bool SupportStackedGroups { get; private set; }
		public bool CircularChartArea { get; private set; }
		public bool SwitchValueAxes { get; private set; }
		public bool SideBySideSeries { get; private set; }
		public bool StackSign { get; private set; }
		public bool RequireAxes { get; private set; }
		public bool HundredPercent { get; private set; }
		public int YValuesPerPoint { get; private set; }
		public bool IsPointChart { get; private set; }
		public bool IsPieChart { get; private set; }
		public bool CanDisplayMultipleSeries { get; private set; }
	}
}
