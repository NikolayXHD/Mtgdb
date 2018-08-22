using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using JetBrains.Annotations;

namespace Mtgdb.Gui
{
	public class ReportSettings
	{
		[UsedImplicitly] // to find usages in IDE
		public ReportSettings()
		{
		}

		public DataSource DataSource { get; set; }
		public bool ApplyFilter { get; set; }
		public List<string> SeriesFields { get; set; } = new List<string>();
		public List<SortOrder> SeriesFieldsSort { get; set; } = new List<SortOrder>();
		public List<string> ColumnFields { get; set; } = new List<string>();
		public List<SortOrder> ColumnFieldsSort { get; set; } = new List<SortOrder>();
		public List<string> SummaryFields { get; set; } = new List<string>();
		public List<string> SummaryFunctions { get; set; } = new List<string>();
		public List<SortOrder> SummarySort { get; set; } = new List<SortOrder>();
		public SeriesChartType ChartType { get; set; }
		public DataElement LabelDataElement { get; set; }
		public bool ShowArgumentTotal { get; set; }
		public bool ShowSeriesTotal { get; set; } = true;
		public bool ExplainTotal { get; set; }

		public bool EnsureDefaults()
		{
			bool modified = false;
			modified |= ensureNonEmpty(SeriesFields, SeriesFieldsSort, string.Empty);
			modified |= ensureNonEmpty(ColumnFields, ColumnFieldsSort, string.Empty);
			modified |= ensureNonEmpty(SummaryFields, SummarySort, string.Empty);

			bool addDefaultSummaryFunction = SummaryFunctions.Count == 0;

			modified |= addDefaultSummaryFunction;

			if (addDefaultSummaryFunction)
				SummaryFunctions.AddRange(Enumerable.Repeat(Aggregates.Count, SummaryFields.Count));

			modified |= removeDuplicateSummaries();

			return modified;
		}

		private bool removeDuplicateSummaries()
		{
			bool modified = false;

			for (int k = SummaryFields.Count - 1; k >= 0; k--)
			{
				if (Enumerable.Range(0, k).Any(s =>
					SummaryFunctions[s] == SummaryFunctions[k] &&
					SummaryFields[s] == SummaryFields[k]))
				{
					SummaryFields.RemoveAt(k);
					SummaryFunctions.RemoveAt(k);
					SummarySort.RemoveAt(k);

					modified = true;
				}
			}

			return modified;
		}

		private static bool ensureNonEmpty(List<string> fields, List<SortOrder> order, string emptyValue)
		{
			if (fields.Count == 0)
			{
				fields.Add(emptyValue);
				order.Add(SortOrder.None);
				return true;
			}

			if (order.Count == 0)
			{
				order.AddRange(Enumerable.Repeat(SortOrder.None, fields.Count));
				return true;
			}

			return false;
		}
	}
}