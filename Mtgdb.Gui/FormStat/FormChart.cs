﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using LinqLib.Sequence;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	[Localizable(false)]
	public partial class FormChart : CustomBorderForm
	{
		public FormChart()
		{
			InitializeComponent();
		}

		public FormChart(CardRepository repository)
			: this()
		{
			_repository = repository;

			Load += load;

			_headerButtons = new[] { _buttonManaCurve, _buttonDeckPrice, _buttonArtistsPerYear };

			foreach (var button in _headerButtons)
				button.Click += buttonClick;

			SnapTo(Direction.North, System.Windows.Forms.Cursor.Position);

			var defaultIcons = new[] { SortIconsOrder[0], SortIconsOrder[0], AggregateIconsOrder[0], SortIconsOrder[0] };
			var tabs = new[] { _tabCols, _tabRows, _tabSumm, _tabSummSort };

			var buttons = new[] { _buttonAddCol, _buttonAddRow, _buttonAddSum };

			_tabByButton = Enumerable.Range(0, buttons.Length)
				.ToDictionary(i => buttons[i], i => tabs[i]);

			foreach (var button in buttons)
				button.Click += buttonAddFieldClick;

			_buttonApply.Click += buttonApplyClick;

			for (int i = 0; i < tabs.Length; i++)
			{
				tabs[i].DefaultIcon = defaultIcons[i];

				if (tabs[i] != _tabSumm)
					tabs[i].Click += tabAxisClick;
				else
					tabs[i].Click += tabSummClick;

				tabs[i].TabRemoving += tabRemoving;
			}

			_menuDataSource.Items.AddRange(Enum.GetNames(typeof (DataSource)).Cast<object>().ToArray());
			_menuDataSource.SelectedIndex = 0;

			_menuLabelDataElement.Items.AddRange(Enum.GetNames(typeof (DataElement)).Cast<object>().ToArray());
			_menuDataSource.SelectedIndex = 0;

			_menuChartType.Items.AddRange(
				Enum.GetValues(typeof (SeriesChartType))
					.Cast<SeriesChartType>()
					.Where(isChartTypeSupported)
					.Select(_ => _.ToString())
					.Cast<object>().ToArray());
			_menuChartType.SelectedIndex = 0;

			_menuFields.Items.AddRange(FieldsOrder.Select(_ => Fields.ByName[_].Alias).Cast<object>().ToArray());

			_summTabs = new[]
			{
				_tabSumm,
				_tabSummSort
			};

			foreach (var tab in _summTabs)
			{
				tab.TabAdded += tabSummAdded;
				tab.TabRemoving += tabSummRemoving;
				tab.TabReordered += tabSummReordered;
			}

			_menuPriceChartType.Items.AddRange(new object[]
			{
				SeriesChartType.Bar.ToString(),
				SeriesChartType.Pyramid.ToString(),
				SeriesChartType.Pie.ToString(),
				SeriesChartType.Doughnut.ToString()
			});
			_menuPriceChartType.SelectedIndex = 0;

			_menuPrice.Items.AddRange(new object[]
			{
				@"Low",
				@"Mid",
				@"High"
			});
			_menuPrice.SelectedIndex = 1;

			_menuPrice.SelectedIndexChanged += priceMenuIndexChanged;
			_menuPriceChartType.SelectedIndexChanged += priceMenuIndexChanged;
			_menuChartType.SelectedIndexChanged += chartTypeChanged;

			_headerButtons[0].Checked = true;
		}



		private static bool isChartTypeSupported(SeriesChartType arg)
		{
			var metadata = ChartTypeMetadata.ByType[arg];
			return metadata.YValuesPerPoint == 1 && !metadata.CircularChartArea;
		}

		private void chartTypeChanged(object sender, EventArgs e)
		{
			if (_applyingSettings)
				return;

			buildCustomChart();
		}

		private void buttonApplyClick(object sender, EventArgs e)
		{
			buildCustomChart();
		}

		private void buildCustomChart()
		{
			foreach (var checkBox in _headerButtons)
				checkBox.Checked = false;

			var settings = readSettings();
			loadWhenReady(null, () => _repository.IsLocalizationLoadingComplete, settings, true);
		}

		private static void tabAxisClick(object sender, EventArgs e)
		{
			var tab = (TabHeaderControl) sender;

			if (tab.IsDragging())
				return;

			if (tab.HoveredCloseIndex >= 0)
				return;

			var tabSetting = tab.GetTabSetting(tab.HoveredIndex);
			if (tabSetting == null)
				return;

			var index = (SortIconsOrder.IndexOf(tabSetting.Icon) + 1)%SortIconsOrder.Length;
			var sortIcon = SortIconsOrder[index];

			tab.SetTabSetting(tabSetting.TabId, new TabSettings(sortIcon));
		}

		private static void tabSummClick(object sender, EventArgs e)
		{
			var tab = (TabHeaderControl) sender;

			if (tab.IsDragging())
				return;

			if (tab.HoveredCloseIndex >= 0)
				return;

			var tabSetting = tab.GetTabSetting(tab.HoveredIndex);
			if (tabSetting == null)
				return;

			string fieldName = getFieldName(tabSetting.TabId);
			var field = Fields.ByName[fieldName];

			var index = AggregateIconsOrder.IndexOf(tabSetting.Icon);
			while (true)
			{
				index = (index + 1)%AggregateIconsOrder.Length;
				var aggreate = AggregatesOrder[index];
				if (field.IsNumeric || aggreate == Aggregates.Count || aggreate == Aggregates.CountDistinct)
					break;
			}

			var aggregateIcon = AggregateIconsOrder[index];
			tab.SetTabSetting(tabSetting.TabId, new TabSettings(aggregateIcon));
		}

		private void tabRemoving(TabHeaderControl tab, int index)
		{
			var tabSettings = tab.GetTabSetting(index);
			if (tabSettings == null)
				return;

			var fieldName = getFieldName(tabSettings.TabId);
			var fieldIndex = FieldsOrder.IndexOf(fieldName);

			if (fieldIndex < 0)
				return;

			_menuFields.SelectedIndex = fieldIndex;
		}

		private static string getFieldName(object tabId)
		{
			return ((TabField) tabId).FieldName;
		}

		private static object createTabId(string fieldName)
		{
			return new TabField
			{
				FieldName = fieldName
			};
		}

		private void buttonAddFieldClick(object sender, EventArgs e)
		{
			if (_menuFields.SelectedIndex < 0)
				return;

			var button = (Button) sender;
			var tab = _tabByButton[button];
			var fieldName = FieldsOrder[_menuFields.SelectedIndex];

			var field = Fields.ByName[fieldName];
			var tabId = createTabId(fieldName);

			if (button == _buttonAddSum)
			{
				if (field.IsNumeric)
					tab.AddTab(tabId, field.Alias);
				else
					tab.AddTab(tabId, field.Alias, AggregateIconsOrder[AggregatesOrder.IndexOf(Aggregates.Count)]);
			}
			else
			{
				if (tab.TabIds.Select(getFieldName).Any(fn => fn == fieldName))
					return;

				tab.AddTab(tabId, field.Alias);
			}
		}



		private static ChartArea createArea()
		{
			var area = new ChartArea();
			area.AxisX.MajorGrid.LineWidth = 0;
			area.AxisX.MinorGrid.LineWidth = 0;
			area.AxisX.Interval = 1;
			area.AxisY.MajorGrid.LineColor = Color.DarkGray;

			return area;
		}

		private void load(object sender, EventArgs e)
		{
			loadReport(_buttonManaCurve);
		}

		private void priceMenuIndexChanged(object sender, EventArgs e)
		{
			foreach (var button in _headerButtons)
				button.Checked = button == _buttonDeckPrice;

			loadReport(_buttonDeckPrice);
		}

		private void buttonClick(object sender, EventArgs e)
		{
			foreach (CheckBox button in _headerButtons)
				button.Checked = button == sender;

			var checkBox = (CheckBox) sender;
			loadReport(checkBox);
		}

		private void loadReport(CheckBox button)
		{
			ReportSettings settings = null;
			Func<bool> isReady = () => _repository.IsLocalizationLoadingComplete;

			if (button == _buttonManaCurve)
			{
				settings = new ReportSettings
				{
					SeriesFields = new List<string> { nameof(Card.Types) },
					SeriesFieldsSort = new List<SortOrder> { SortOrder.Ascending },
					ColumnFields = new List<string> { nameof(Card.Cmc) },
					ColumnFieldsSort = new List<SortOrder> { SortOrder.Ascending },
					SummaryFields = new List<string> { nameof(Card.DeckCount) },
					SummaryFunctions = new List<string> { Aggregates.Sum },
					ChartType = SeriesChartType.StackedColumn,
					LabelDataElement = DataElement.Series,
					ShowArgumentTotal = true
				};

				isReady = () => _repository.IsLoadingComplete;
			}
			else if (button == _buttonArtistsPerYear)
			{
				settings = new ReportSettings
				{
					DataSource = DataSource.AllCards,
					ColumnFields = new List<string> { nameof(Card.ReleaseYear) },
					ColumnFieldsSort = new List<SortOrder> { SortOrder.Ascending },
					SummaryFields = new List<string> { nameof(Card.Artist) },
					SummaryFunctions = new List<string> { Aggregates.CountDistinct },
					ChartType = SeriesChartType.Spline,
					LabelDataElement = DataElement.Values,
					ExplainTotal = true
				};

				isReady = () => _repository.IsLoadingComplete;
			}
			else if (button == _buttonDeckPrice)
			{
				settings = new ReportSettings
				{
					ColumnFields = new List<string> { nameof(Card.NameEn) },
					SummaryFunctions = new List<string> { Aggregates.Sum, Aggregates.Sum },
					SummarySort = new List<SortOrder> { SortOrder.Descending, SortOrder.None },
					LabelDataElement = DataElement.SummaryField,
					SummaryFields = getPriceSummaryFields(),
					ChartType = (SeriesChartType) Enum.Parse(typeof (SeriesChartType), (string) _menuPriceChartType.SelectedItem)
				};

				if (!ChartTypeMetadata.ByType[settings.ChartType].CanDisplayMultipleSeries)
				{
					settings.SummaryFields.RemoveAt(1);
					settings.SummarySort.RemoveAt(1);
					settings.SummaryFunctions.RemoveAt(1);

					settings.LabelDataElement = DataElement.Argument;
				}
			}

			if (settings != null)
				loadWhenReady(button, isReady, settings, false);
		}

		private List<string> getPriceSummaryFields()
		{
			List<string> summaryFields;

			if (_menuPrice.SelectedIndex == 0)
			{
				summaryFields = new List<string>
				{
					nameof(Card.DeckTotalLow),
					nameof(Card.PriceLow)
				};
			}
			else if (_menuPrice.SelectedIndex == 1)
			{
				summaryFields = new List<string>
				{
					nameof(Card.DeckTotalMid),
					nameof(Card.PriceMid)
				};
			}
			else
			{
				summaryFields = new List<string>
				{
					nameof(Card.DeckTotalHigh),
					nameof(Card.PriceHigh)
				};
			}
			return summaryFields;
		}

		private void buildReport(ReportSettings settings)
		{
			var cards = getCards(settings.DataSource)
				.ToArray();

			var seriesGroups = getGroups(settings.SeriesFields, settings.SeriesFieldsSort, cards);
			var argumentGroups = getGroups(settings.ColumnFields, settings.ColumnFieldsSort, cards);

			var series = getNames(seriesGroups, SeriesSeparator);
			var arguments = getNames(argumentGroups, ArgumentSeparator);

			var argumentTotals = getTotals(settings, argumentGroups);
			var seriesTotals = getTotals(settings, seriesGroups);

			var values = getValues(seriesGroups, argumentGroups, settings.SummaryFields, settings.SummaryFunctions);

			var sortedArgumentIndices = getSortBySummary(settings, argumentTotals);
			var sortedArguments = applySort(arguments, sortedArgumentIndices);
			var sortedArgumentTotals = applySort(argumentTotals, sortedArgumentIndices);

			var sortedValues = values
				.Select(serie => applySort(serie, sortedArgumentIndices))
				.ToList();

			populateData(sortedArguments, series, sortedValues, sortedArgumentTotals, seriesTotals, settings);
		}

		private static IList<object[]> getTotals(ReportSettings settings, List<KeyValuePair<List<object>, HashSet<Card>>> groups)
		{
			object[][] totals = new object[groups.Count][];
			for (int i = 0; i < totals.Length; i++)
				totals[i] = new object[settings.SummaryFields.Count];

			for (int k = 0; k < settings.SummaryFields.Count; k++)
			{
				int s = k;

				var summField = Fields.ByName[settings.SummaryFields[s]];
				var summFunction = Aggregates.ByName[settings.SummaryFunctions[s]];

				for (int i = 0; i < totals.Length; i++)
					totals[i][k] = summFunction(groups[i].Value, summField);
			}

			return totals;
		}



		private static List<KeyValuePair<List<object>, HashSet<Card>>> getGroups(List<string> fields, List<SortOrder> order, Card[] cards)
		{
			var columnGroups = Fields.ByName[fields[0]].GroupBy(cards, order[0]);

			for (int i = 1; i < fields.Count; i++)
				columnGroups = Fields.ByName[fields[i]].ThenGroupBy(columnGroups, order[i]);

			return columnGroups;
		}

		private static List<string> getNames(List<KeyValuePair<List<object>, HashSet<Card>>> groups, string separator)
		{
			var seriesNames = new List<string>(groups.Count);

			for (int i = 0; i < groups.Count; i++)
			{
				string name = string.Join(separator, groups[i].Key.Where(_ => _ != null));
				seriesNames.Add(name);
			}

			return seriesNames;
		}
		
		private static List<List<object[]>> getValues(
			List<KeyValuePair<List<object>, HashSet<Card>>> rowGroups,
			List<KeyValuePair<List<object>, HashSet<Card>>> columnGroups,
			List<string> aggregateFields,
			List<string> aggregateFunctions)
		{
			var seriesValues = new List<List<object[]>>(rowGroups.Count);

			for (int j = 0; j < rowGroups.Count; j++)
			{
				seriesValues.Add(new List<object[]>(columnGroups.Count));

				for (int i = 0; i < columnGroups.Count; i++)
				{
					var objects = new HashSet<Card>(rowGroups[j].Value);
					objects.IntersectWith(columnGroups[i].Value);

					var aggregates = new object[aggregateFunctions.Count];

					for (int k = 0; k < aggregateFunctions.Count; k++)
					{
						string function = aggregateFunctions[k];
						string field = aggregateFields[k];
						object aggregated = Aggregates.ByName[function](objects, Fields.ByName[field]);
						aggregates[k] = aggregated;
					}

					seriesValues[j].Add(aggregates);
				}
			}

			return seriesValues;
		}



		private static int[] getSortBySummary(ReportSettings settings, IList<object[]> argumentTotals)
		{
			var sortedArgumentIndices = Enumerable.Range(0, argumentTotals.Count)
				.OrderBy(i => 0);

			for (int k = 0; k < settings.SummarySort.Count; k++)
			{
				int s = k;

				if (settings.SummarySort[s] == SortOrder.Ascending)
				{
					sortedArgumentIndices = sortedArgumentIndices
						.ThenByDescending(i => argumentTotals[i][s] != null)
						.ThenBy(i => argumentTotals[i][s], Comparer<object>.Default);
				}
				else if (settings.SummarySort[s] == SortOrder.Descending)
				{
					sortedArgumentIndices = sortedArgumentIndices
						.ThenBy(i => argumentTotals[i][s] != null)
						.ThenByDescending(i => argumentTotals[i][s], Comparer<object>.Default);
				}
			}

			return sortedArgumentIndices.ToArray();
		}

		private void populateData(
			List<string> arguments,
			List<string> series,
			List<List<object[]>> seriesValues,
			IList<object[]> argumentTotals,
			IList<object[]> seriesTotals,
			ReportSettings settings)
		{
			var summaryFields = settings.SummaryFields;
			var palette = ChartPalettes.ByName[_chart.Palette];
			var metadata = ChartTypeMetadata.ByType[settings.ChartType];

			_chart.ChartAreas.Clear();
			_chart.Series.Clear();
			_chart.Legends.Clear();

			if (metadata.CanDisplayMultipleSeries)
				_chart.Legends.Add(new Legend());

			ChartArea area = null;
			Series[] seriesList = null;
			string[] seriesSummaryLegend = null;
			List<string>[] argumentSummaryLegends = null;

			for (int k = 0; k < summaryFields.Count; k++)
			{
				string summaryFieldAlias = Fields.ByName[summaryFields[k]].Alias;
				string summaryFunctionAlias = Aggregates.Alias[settings.SummaryFunctions[k]];

				if (k == 0 || !metadata.CanDisplayMultipleSeries)
				{
					area = createArea();
					_chart.ChartAreas.Add(area);

					if (metadata.SwitchValueAxes)
						area.AxisX.IsReversed = true;

					if (metadata.IsPieChart)
					{
						area.Area3DStyle.Enable3D = true;
						area.Area3DStyle.Inclination = 30;
					}

					seriesSummaryLegend = new string[series.Count];

					argumentSummaryLegends = new List<string>[arguments.Count];
					for (int i = 0; i < arguments.Count; i++)
						argumentSummaryLegends[i] = new List<string>();

					if (metadata.CanDisplayMultipleSeries)
						seriesList = new Series[series.Count*summaryFields.Count];
					else
						seriesList = new Series[1];

					for (int j = 0; j < seriesList.Length; j++)
					{
						var chartSeries = new Series
						{
							ChartType = settings.ChartType,
							ChartArea = area.Name
						};

						if (settings.LabelDataElement == DataElement.None)
						{
							chartSeries.IsValueShownAsLabel = false;
						}
						else
						{
							chartSeries.LabelFormat = @"0.##";
							chartSeries.EmptyPointStyle.LabelFormat = @"#";
							chartSeries.IsValueShownAsLabel = true;
						}

						seriesList[j] = chartSeries;

						if (metadata.IsPieChart)
						{
							chartSeries[@"PieLabelStyle"] = @"Outside";
							chartSeries[@"PieLineColor"] = @"Gray";
						}

						chartSeries[@"PyramidValueType"] = @"Surface";

						_chart.Series.Add(chartSeries);

						chartSeries.IsVisibleInLegend = settings.ShowSeriesTotal && metadata.CanDisplayMultipleSeries;
					}
				}


				if (metadata.CanDisplayMultipleSeries && metadata.SupportStackedGroups)
					for (int j = 0; j < series.Count; j++)
					{
						var chartSeries = seriesList[k*series.Count + j];
						chartSeries[@"StackedGroupName"] = summaryFieldAlias;
					}

				for (int i = 0; i < arguments.Count; i++)
					for (int j = 0; j < series.Count; j++)
					{
						Series chartSeries;
						if (metadata.CanDisplayMultipleSeries)
							chartSeries = seriesList[k*series.Count + j];
						else
							chartSeries = seriesList[0];

						var yValue = seriesValues[j][i][k];

						chartSeries.Points.AddXY(arguments[i], yValue);

						var point = chartSeries.Points[chartSeries.Points.Count - 1];

						if (yValue == null || Convert.ToSingle(yValue) < 0.009f)
							point.IsEmpty = true;
						else if (settings.LabelDataElement == DataElement.SummaryField)
							point.Label = $"{yValue:0.##}: {summaryFieldAlias}";
						else if (settings.LabelDataElement == DataElement.Series)
							point.Label = $"{yValue:0.##}: {series[j]}";
						else if (settings.LabelDataElement == DataElement.Argument)
							point.Label = $"{yValue:0.##}: {arguments[i]}";
						else if (settings.LabelDataElement == DataElement.SeriesAndArgument)
							point.Label = $"{yValue:0.##}: {arguments[i]}, {series[j]}";
						else if (settings.LabelDataElement == DataElement.Values)
							point.Label = $"{yValue:0.##}";
						else
							point.Label = null;

						if (series.Count > 1)
							point.Color = palette[j%palette.Length];
						else if (!metadata.RequireAxes)
							point.Color = palette[i%palette.Length];
						else if (summaryFields.Count > 1)
							point.Color = palette[k%palette.Length];
					}

				if (settings.ShowSeriesTotal)
					for (int j = 0; j < series.Count; j++)
					{
						string legend;

						bool seriesSet = settings.SeriesFields.Count > 1 || settings.SeriesFields[0] != string.Empty;
						string seriesName;
						if (seriesSet)
							seriesName = series[j];
						else
							seriesName = $"{summaryFunctionAlias} {summaryFieldAlias}";

						if (settings.ExplainTotal && seriesSet)
							legend = $"{seriesName}: {seriesTotals[j][k]}, {summaryFunctionAlias} {summaryFieldAlias}";
						else
							legend = $"{seriesName}: {seriesTotals[j][k]}";

						if (metadata.CanDisplayMultipleSeries)
							seriesList[k*series.Count + j].LegendText = legend;
						else
							seriesSummaryLegend[j] = legend;
					}

				if (settings.ShowArgumentTotal)
					for (int i = 0; i < arguments.Count; i++)
					{
						string legend;

						if (settings.ExplainTotal)
							legend = $"{arguments[i]}: {argumentTotals[i][k]}, {summaryFunctionAlias} {summaryFieldAlias}";
						else
							legend = $"{arguments[i]}: {argumentTotals[i][k]}";

						argumentSummaryLegends[i].Add(legend);
					}

				if ((settings.ShowSeriesTotal || settings.ShowArgumentTotal) && !metadata.CanDisplayMultipleSeries)
				{
					var legend = new Legend
					{
						DockedToChartArea = area.Name,
						IsDockedInsideChartArea = false
					};

					if (settings.ShowSeriesTotal)
						for (int j = 0; j < seriesSummaryLegend.Length; j++)
						{
							string seriesLegend = seriesSummaryLegend[j];
							legend.CustomItems.Add(palette[j%palette.Length], seriesLegend);
						}

					if (settings.ShowArgumentTotal)
						for (int i = 0; i < argumentSummaryLegends.Length; i++)
						{
							var argumentLegend = argumentSummaryLegends[i];
							legend.CustomItems.Add(Color.DimGray, argumentLegend[0]);
						}

					_chart.Legends.Add(legend);
				}
			}

			if (metadata.CanDisplayMultipleSeries && settings.ShowArgumentTotal)
			{
				var argumentSummaries = argumentSummaryLegends
					.Select(legends => string.Join("\r\n", legends))
					.ToArray();

				for (int k = 0; k < summaryFields.Count; k++)
					for (int j = 0; j < series.Count; j++)
					{
						var chartSeries = _chart.Series[k*series.Count + j];

						for (int i = 0; i < chartSeries.Points.Count; i++)
							chartSeries.Points[i].AxisLabel = argumentSummaries[i];
					}
			}
		}



		private static List<T> applySort<T>(IList<T> values, int[] sortedIndices)
		{
			return sortedIndices
				.Select(i => values[i])
				.ToList();
		}

		private void loadWhenReady(CheckBox button, Func<bool> ready, ReportSettings settings, bool isCustom)
		{
			bool modified = settings.EnsureDefaults();

			if (!isCustom || modified)
				display(settings);

			ThreadPool.QueueUserWorkItem(_ =>
			{
				while (!ready())
				{
					this.Invoke(delegate
					{
						_progressBar.Visible = true;

						if (_progressBar.Value == _progressBar.Maximum)
							_progressBar.Value = _progressBar.Minimum;
						else
							_progressBar.Value++;
					});

					if (button?.Checked == false)
					{
						this.Invoke(delegate
						{
							_progressBar.Value = 1;
							_progressBar.Visible = false;
						});

						return;
					}

					Thread.Sleep(500);
				}

				this.Invoke(delegate
				{
					_progressBar.Value = 0;
					_progressBar.Visible = false;

					if (button?.Checked == false)
						return;

					buildReport(settings);
				});
			});
		}

		private IEnumerable<Card> getCards(DataSource source)
		{
			switch (source)
			{
				case DataSource.Collection:
					return _repository.Cards.Where(_ => _.CollectionCount > 0);
				case DataSource.Deck:
					return _repository.Cards.Where(_ => _.DeckCount > 0);
				case DataSource.SearchResult:
					return _repository.Cards.Where(_ => _.IsSearchResult);
				case DataSource.AllCards:
					return _repository.Cards;
				default:
					throw new ArgumentOutOfRangeException(nameof(source), source, null);
			}
		}



		private void display(ReportSettings settings)
		{
			_applyingSettings = true;

			displayAxis(_tabCols, settings.ColumnFields, settings.ColumnFieldsSort);
			displayAxis(_tabRows, settings.SeriesFields, settings.SeriesFieldsSort);

			_tabSumm.Count = 0;
			_tabSummSort.Count = 0;
			for (int i = 0; i < settings.SummaryFields.Count; i++)
			{
				_tabSummSort.AddTab(
					createTabId(settings.SummaryFields[i]),
					Fields.ByName[settings.SummaryFields[i]].Alias,
					SortIconsOrder[(int) settings.SummarySort[i]]);

				_tabSumm.AddTab(
					createTabId(settings.SummaryFields[i]),
					Fields.ByName[settings.SummaryFields[i]].Alias,
					AggregateIconsOrder[AggregatesOrder.IndexOf(settings.SummaryFunctions[i])]);
			}

			_menuDataSource.SelectedIndex = (int) settings.DataSource;
			_menuLabelDataElement.SelectedIndex = (int) settings.LabelDataElement;
			_menuChartType.SelectedIndex = _menuChartType.Items.IndexOf(settings.ChartType.ToString());

			_buttonArgumentTotals.Checked = settings.ShowArgumentTotal;
			_buttonSeriesTotal.Checked = settings.ShowSeriesTotal;
			_buttonExplainTotal.Checked = settings.ExplainTotal;

			_applyingSettings = false;
		}

		private static void displayAxis(TabHeaderControl tab, List<string> fields, List<SortOrder> fieldsSort)
		{
			tab.Count = 0;

			for (int i = 0; i < fields.Count; i++)
			{
				if (string.IsNullOrEmpty(fields[i]))
					continue;

				tab.AddTab(createTabId(fields[i]),
					Fields.ByName[fields[i]].Alias,
					SortIconsOrder[(int) fieldsSort[i]]);
			}
		}

		private ReportSettings readSettings()
		{
			var result = new ReportSettings
			{
				DataSource = (DataSource) Enum.Parse(typeof (DataSource), (string) _menuDataSource.SelectedItem),
				LabelDataElement = (DataElement) Enum.Parse(typeof (DataElement), (string) _menuLabelDataElement.SelectedItem),
				ChartType = (SeriesChartType) Enum.Parse(typeof (SeriesChartType), (string) _menuChartType.SelectedItem),
				ColumnFields = readFields(_tabCols),
				ColumnFieldsSort = readEnum<SortOrder>(_tabCols, SortIconsOrder),
				SeriesFields = readFields(_tabRows),
				SeriesFieldsSort = readEnum<SortOrder>(_tabRows, SortIconsOrder),
				SummaryFields = readFields(_tabSummSort),
				SummarySort = readEnum<SortOrder>(_tabSummSort, SortIconsOrder),
				SummaryFunctions = _tabSumm.Icons
					.Select(icn => AggregatesOrder[AggregateIconsOrder.IndexOf(icn)])
					.ToList(),
				ShowArgumentTotal = _buttonArgumentTotals.Checked,
				ShowSeriesTotal = _buttonSeriesTotal.Checked,
				ExplainTotal = _buttonExplainTotal.Checked
			};

			return result;
		}

		private static List<string> readFields(TabHeaderControl tab)
		{
			return tab.TabIds.Select(getFieldName).ToList();
		}

		private static List<T> readEnum<T>(TabHeaderControl tab, Bitmap[] iconsOrder)
		{
			return tab.Icons
				.Select(icn => iconsOrder.IndexOf(icn))
				.Cast<T>()
				.ToList();
		}



		private void tabSummAdded(TabHeaderControl tab, int addedIndex)
		{
			if (_applyingSettings)
				return;

			_applyingSettings = true;

			var settings = _tabSumm.GetTabSetting(addedIndex);

			var mirrors = _summTabs.Where(_ => _ != tab);
			foreach (var mirror in mirrors)
				mirror.AddTab(settings.TabId, settings.Text);

			_applyingSettings = false;
		}

		private void tabSummRemoving(TabHeaderControl tab, int removingIndex)
		{
			if (_applyingSettings)
				return;

			_applyingSettings = true;

			var mirrors = _summTabs.Where(_ => _ != tab);

			foreach (var mirror in mirrors)
				mirror.RemoveTab(removingIndex);

			_applyingSettings = false;
		}

		private void tabSummReordered(TabHeaderControl tab)
		{
			if (_applyingSettings)
				return;

			_applyingSettings = true;

			var mirrors = _summTabs.Where(_ => _ != tab);

			foreach (var mirror in mirrors)
				mirror.ApplyOrderFrom(tab);

			_applyingSettings = false;
		}



		private static readonly string[] AggregatesOrder =
		{
			Aggregates.Sum,
			Aggregates.Count,
			Aggregates.CountDistinct,
			Aggregates.Min,
			Aggregates.Average,
			Aggregates.Max
		};

		private static readonly Bitmap[] AggregateIconsOrder =
		{
			ResourcesFilter.sum_hovered,
			ResourcesFilter.count_hovered,
			ResourcesFilter.count_distinct_hovered,
			ResourcesFilter.min_hovered,
			ResourcesFilter.avg_hovered,
			ResourcesFilter.max_hovered
		};

		private static readonly Bitmap[] SortIconsOrder =
		{
			ResourcesFilter.sort_none_hovered,
			ResourcesFilter.sort_asc_hovered,
			ResourcesFilter.sort_desc_hovered
		};

		private readonly Dictionary<Button, TabHeaderControl> _tabByButton;
		private readonly TabHeaderControl[] _summTabs;

		private const string SeriesSeparator = " / ";
		private const string ArgumentSeparator = " / ";

		private static readonly string[] FieldsOrder = Fields.ChartFields.OrderBy(_ => Fields.ByName[_].Alias).ToArray();

		private readonly CardRepository _repository;
		private bool _applyingSettings;
		private readonly CheckBox[] _headerButtons;
	}
}