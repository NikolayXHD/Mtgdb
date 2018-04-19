using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	[Localizable(false)]
	public partial class FormChart : CustomBorderForm
	{
		public FormChart()
		{
			InitializeComponent();

			_menus = new[]
			{
				_menuDataSource,
				_menuChartType,
				_menuFields,
				_menuLabelDataElement,
				_menuPrice,
				_menuPriceChartType
			};

			_buttons = new[]
			{
				_buttonAddCol,
				_buttonAddRow,
				_buttonAddSum
			};

			_headerButtons = new[]
			{
				_buttonDeckPrice,
				_buttonCollectionPrice,

				_buttonManaCurveType,
				_buttonManaCurveManacost,
				_buttonDeckColors,
				_buttonCollectionColors,

				_buttonArtistsPerYear
			};

			_tabs = new[]
			{
				_tabCols,
				_tabRows,
				_tabSumm,
				_tabSummSort
			};

			_summTabs = new[]
			{
				_tabSumm,
				_tabSummSort
			};

			RegisterDragControl(_layoutTitle);

			foreach (var button in _headerButtons)
				RegisterDragControl(button);

			scale();
		}

		private void scale()
		{
			this.SuspendLayout();

			TitleHeight = TitleHeight.ByDpiHeight();

			ImageMinimize = ImageMinimize.HalfResizeDpi();
			ImageMaximize = ImageMaximize.HalfResizeDpi();
			ImageNormalize = ImageNormalize.HalfResizeDpi();
			ImageClose = ImageClose.HalfResizeDpi();

			foreach (var tab in _tabs)
			{
				tab.Height = tab.Height.ByDpiHeight();
				tab.SlopeSize = tab.SlopeSize.ByDpi();
				tab.AddButtonSlopeSize = tab.SlopeSize.ByDpi();

				tab.CloseIcon = tab.CloseIcon.HalfResizeDpi();
				tab.CloseIconHovered = tab.CloseIconHovered.HalfResizeDpi();
			}

			_buttonApply.ScaleDpi();
			_progressBar.ScaleDpi();

			foreach (var button in _buttons)
				button.ScaleDpi();

			foreach (var button in _headerButtons)
				button.ScaleDpi();

			foreach (var menu in _menus)
				menu.ScaleDpi();

			_sortIconsOrder = new[]
			{
				Resources.sort_none_hovered.HalfResizeDpi(),
				Resources.sort_asc_hovered.HalfResizeDpi(),
				Resources.sort_desc_hovered.HalfResizeDpi()
			};

			_aggregateIconsOrder = new[]
			{
				ResourcesFilter.sum_hovered.HalfResizeDpi(),
				ResourcesFilter.count_hovered.HalfResizeDpi(),
				ResourcesFilter.count_distinct_hovered.HalfResizeDpi(),
				ResourcesFilter.min_hovered.HalfResizeDpi(),
				ResourcesFilter.avg_hovered.HalfResizeDpi(),
				ResourcesFilter.max_hovered.HalfResizeDpi()
			};

			this.ResumeLayout(false);
			this.PerformLayout();
		}

		public FormChart(CardRepository repository, UiModel ui, Fields fields)
			: this()
		{
			_ui = ui;
			_fields = fields;
			_fieldsOrder = fields.ChartFields.OrderBy(_ => _fields.ByName[_].Alias)
				.ToArray();

			_repository = repository;

			Load += load;

			foreach (var button in _headerButtons)
				button.Click += buttonClick;

			SnapTo(Direction.North, System.Windows.Forms.Cursor.Position);

			_tabByButton = Enumerable.Range(0, _buttons.Length)
				.ToDictionary(i => _buttons[i], i => _tabs[i]);

			foreach (var button in _buttons)
				button.Click += buttonAddFieldClick;

			_buttonApply.Click += buttonApplyClick;

			var defaultIcons = new[]
			{
				_sortIconsOrder[0],
				_sortIconsOrder[0],
				_aggregateIconsOrder[0],
				_sortIconsOrder[0]
			};

			for (int i = 0; i < _tabs.Length; i++)
			{
				_tabs[i].DefaultIcon = defaultIcons[i];

				if (_tabs[i] != _tabSumm)
					_tabs[i].Click += tabAxisClick;
				else
					_tabs[i].Click += tabSummClick;

				_tabs[i].TabRemoving += tabRemoving;
			}

			_menuDataSource.Items.AddRange(Enum.GetNames(typeof(DataSource)).Cast<object>().ToArray());
			_menuDataSource.SelectedIndex = 0;

			_menuLabelDataElement.Items.AddRange(Enum.GetNames(typeof(DataElement)).Cast<object>().ToArray());
			_menuDataSource.SelectedIndex = 0;

			_menuChartType.Items.AddRange(
				Enum.GetValues(typeof(SeriesChartType))
					.Cast<SeriesChartType>()
					.Where(isChartTypeSupported)
					.Select(_ => _.ToString())
					.Cast<object>().ToArray());
			_menuChartType.SelectedIndex = 0;

			_menuFields.Items.AddRange(_fieldsOrder.Select(_ => _fields.ByName[_].Alias).Cast<object>().ToArray());

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
			_buttonApplyFilter.CheckedChanged += applyFilterChanged;
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

		private void applyFilterChanged(object sender, EventArgs e)
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

		private void tabAxisClick(object sender, EventArgs e)
		{
			var tab = (TabHeaderControl) sender;

			if (tab.IsDragging())
				return;

			if (tab.HoveredCloseIndex >= 0)
				return;

			var tabSetting = tab.GetTabSetting(tab.HoveredIndex);
			if (tabSetting == null)
				return;

			var index = (_sortIconsOrder.IndexOf(tabSetting.Icon) + 1) % _sortIconsOrder.Count;
			var sortIcon = _sortIconsOrder[index];

			tab.SetTabSetting(tabSetting.TabId, new TabSettings(sortIcon));
		}

		private void tabSummClick(object sender, EventArgs e)
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
			var field = _fields.ByName[fieldName];

			var index = _aggregateIconsOrder.IndexOf(tabSetting.Icon);
			while (true)
			{
				index = (index + 1) % _aggregateIconsOrder.Count;
				var aggreate = _aggregatesOrder[index];
				if (field.IsNumeric || aggreate == Aggregates.Count || aggreate == Aggregates.CountDistinct)
					break;
			}

			var aggregateIcon = _aggregateIconsOrder[index];
			tab.SetTabSetting(tabSetting.TabId, new TabSettings(aggregateIcon));
		}

		private void tabRemoving(TabHeaderControl tab, int index)
		{
			var tabSettings = tab.GetTabSetting(index);
			if (tabSettings == null)
				return;

			var fieldName = getFieldName(tabSettings.TabId);
			var fieldIndex = _fieldsOrder.IndexOf(fieldName);

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
			var fieldName = _fieldsOrder[_menuFields.SelectedIndex];

			var field = _fields.ByName[fieldName];
			var tabId = createTabId(fieldName);

			if (button == _buttonAddSum)
			{
				if (field.IsNumeric)
					tab.AddTab(tabId, field.Alias);
				else
					tab.AddTab(tabId, field.Alias, _aggregateIconsOrder[_aggregatesOrder.IndexOf(Aggregates.Count)]);
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
			_buttonManaCurveType.Checked = true;
			loadReport(_buttonManaCurveType);
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

			if (button == _buttonManaCurveType || button == _buttonManaCurveManacost)
			{
				settings = new ReportSettings
				{
					DataSource = DataSource.Deck,
					SeriesFieldsSort = new List<SortOrder> { SortOrder.Ascending },
					ColumnFields = new List<string> { nameof(Card.Cmc) },
					ColumnFieldsSort = new List<SortOrder> { SortOrder.Ascending },
					SummaryFields = new List<string> { nameof(Card.DeckCount) },
					SummaryFunctions = new List<string> { Aggregates.Sum },
					ChartType = SeriesChartType.StackedColumn,
					LabelDataElement = DataElement.Series,
					ShowArgumentTotal = true
				};

				if (button == _buttonManaCurveType)
					settings.SeriesFields = new List<string> { nameof(Card.Types) };
				else if (button == _buttonManaCurveManacost)
					settings.SeriesFields = new List<string> { nameof(Card.Color) };

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
			else if (button == _buttonDeckPrice || button == _buttonCollectionPrice)
			{
				settings = new ReportSettings
				{
					ColumnFields = new List<string> { nameof(Card.NameEn) },
					SummaryFunctions = new List<string> { Aggregates.Sum, Aggregates.Sum },
					SummarySort = new List<SortOrder> { SortOrder.Descending, SortOrder.None },
					LabelDataElement = DataElement.SummaryField,
					ChartType = (SeriesChartType) Enum.Parse(typeof(SeriesChartType), (string) _menuPriceChartType.SelectedItem)
				};

				if (button == _buttonDeckPrice)
				{
					settings.SummaryFields = getPriceSummaryFields(_deckPriceTotalFields);
					settings.DataSource = DataSource.Deck;
				}
				else if (button == _buttonCollectionPrice)
				{
					settings.SummaryFields = getPriceSummaryFields(_collectionPriceTotalFields);
					settings.DataSource = DataSource.Collection;
				}
				else
					throw new Exception("Logical error");

				if (!ChartTypeMetadata.ByType[settings.ChartType].CanDisplayMultipleSeries)
				{
					settings.SummaryFields.RemoveAt(1);
					settings.SummarySort.RemoveAt(1);
					settings.SummaryFunctions.RemoveAt(1);

					settings.LabelDataElement = DataElement.Argument;
				}
			}
			else if (button == _buttonDeckColors || button == _buttonCollectionColors)
			{
				settings = new ReportSettings
				{
					SeriesFields = new List<string> { nameof(Card.Types) },
					SeriesFieldsSort = new List<SortOrder> { SortOrder.Ascending },
					ColumnFields = new List<string> { nameof(Card.Color) },
					ColumnFieldsSort = new List<SortOrder> { SortOrder.Ascending },
					SummaryFunctions = new List<string> { Aggregates.Sum },
					ChartType = SeriesChartType.StackedColumn,
					LabelDataElement = DataElement.Series,
					ShowArgumentTotal = true
				};

				if (button == _buttonDeckColors)
				{
					settings.SummaryFields = new List<string> { nameof(Card.DeckCount) };
					settings.DataSource = DataSource.Deck;
				}
				else if (button == _buttonCollectionColors)
				{
					settings.SummaryFields = new List<string> { nameof(Card.CollectionCount) };
					settings.DataSource = DataSource.Collection;
				}

				isReady = () => _repository.IsLoadingComplete;
			}

			if (settings != null)
				loadWhenReady(button, isReady, settings, false);
		}

		private List<string> getPriceSummaryFields(string[] totalFields)
		{
			var summaryFields = new List<string>
			{
				totalFields[_menuPrice.SelectedIndex],
				_priceFields[_menuPrice.SelectedIndex]
			};

			return summaryFields;
		}

		private void buildReport(ReportSettings settings)
		{
			var cards = getCards(settings.DataSource, settings.ApplyFilter)
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

		private IList<object[]> getTotals(ReportSettings settings, List<KeyValuePair<List<object>, HashSet<Card>>> groups)
		{
			object[][] totals = new object[groups.Count][];
			for (int i = 0; i < totals.Length; i++)
				totals[i] = new object[settings.SummaryFields.Count];

			for (int k = 0; k < settings.SummaryFields.Count; k++)
			{
				int s = k;

				var summField = _fields.ByName[settings.SummaryFields[s]];
				var summFunction = Aggregates.ByName[settings.SummaryFunctions[s]];

				for (int i = 0; i < totals.Length; i++)
					totals[i][k] = summFunction(groups[i].Value, summField);
			}

			return totals;
		}



		private List<KeyValuePair<List<object>, HashSet<Card>>> getGroups(List<string> fields, List<SortOrder> order, Card[] cards)
		{
			var columnGroups = _fields.ByName[fields[0]].GroupBy(cards, order[0]);

			for (int i = 1; i < fields.Count; i++)
				columnGroups = _fields.ByName[fields[i]].ThenGroupBy(columnGroups, order[i]);

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

		private List<List<object[]>> getValues(
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
						object aggregated = Aggregates.ByName[function](objects, _fields.ByName[field]);
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
				string summaryFieldAlias = _fields.ByName[summaryFields[k]].Alias;
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
						seriesList = new Series[series.Count * summaryFields.Count];
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
						var chartSeries = seriesList[k * series.Count + j];
						chartSeries[@"StackedGroupName"] = summaryFieldAlias;
					}

				for (int i = 0; i < arguments.Count; i++)
					for (int j = 0; j < series.Count; j++)
					{
						Series chartSeries;
						if (metadata.CanDisplayMultipleSeries)
							chartSeries = seriesList[k * series.Count + j];
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
							point.Color = palette[j % palette.Length];
						else if (!metadata.RequireAxes)
							point.Color = palette[i % palette.Length];
						else if (summaryFields.Count > 1)
							point.Color = palette[k % palette.Length];
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
							seriesList[k * series.Count + j].LegendText = legend;
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
							legend.CustomItems.Add(palette[j % palette.Length], seriesLegend);
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
						var chartSeries = _chart.Series[k * series.Count + j];

						for (int i = 0; i < chartSeries.Points.Count; i++)
							chartSeries.Points[i].AxisLabel = argumentSummaries[i];
					}
			}

			setupScrollbar(metadata);
		}

		private void setupScrollbar(ChartTypeMetadata metadata)
		{
			if (metadata.RequireAxes)
			{
				foreach (var chartArea in _chart.ChartAreas)
				{
					var chartSeries = _chart.Series.Where(s => s.ChartArea == chartArea.Name).ToArray();

					if (chartSeries.Length == 0)
						continue;

					var pointsCount = chartSeries.Max(s => s.Points.Count);

					if (pointsCount <= 60)
						continue;

					var scrollAxis = chartArea.AxisX;
					var scrollBar = scrollAxis.ScrollBar;

					scrollBar.Size = 15;
					scrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
					scrollBar.IsPositionedInside = false;
					scrollBar.Enabled = true;
					scrollBar.BackColor = Color.FromArgb(235, 235, 235);
					scrollBar.ButtonColor = Color.Gainsboro;
					scrollBar.LineColor = Color.DarkGray;

					scrollAxis.ScaleView.Zoom(0, 40);
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

		private IEnumerable<Card> getCards(DataSource source, bool applyFilter)
		{
			IEnumerable<Card> result;

			switch (source)
			{
				case DataSource.Collection:
					result = _repository.Cards.Where(_ => _.CollectionCount(_ui) > 0);
					break;
				case DataSource.Deck:
					result = _repository.Cards.Where(_ => _.DeckCount(_ui) > 0);
					break;
				case DataSource.AllCards:
					result = _repository.Cards;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(source), source, null);
			}

			if (applyFilter)
				result = result.Where(_ => _.IsSearchResult);

			return result;
		}



		private void display(ReportSettings settings)
		{
			_applyingSettings = true;

			foreach (var tab in _tabs)
				tab.SuspendLayout();

			// _panelFields.Visible = false;

			displayAxis(_tabCols, settings.ColumnFields, settings.ColumnFieldsSort);
			displayAxis(_tabRows, settings.SeriesFields, settings.SeriesFieldsSort);

			_tabSumm.Count = 0;
			_tabSummSort.Count = 0;
			for (int i = 0; i < settings.SummaryFields.Count; i++)
			{
				_tabSummSort.AddTab(
					createTabId(settings.SummaryFields[i]),
					_fields.ByName[settings.SummaryFields[i]].Alias,
					_sortIconsOrder[(int) settings.SummarySort[i]]);

				_tabSumm.AddTab(
					createTabId(settings.SummaryFields[i]),
					_fields.ByName[settings.SummaryFields[i]].Alias,
					_aggregateIconsOrder[_aggregatesOrder.IndexOf(settings.SummaryFunctions[i])]);
			}

			_menuDataSource.SelectedIndex = (int) settings.DataSource;
			_buttonApplyFilter.Checked = settings.ApplyFilter;
			_menuLabelDataElement.SelectedIndex = (int) settings.LabelDataElement;
			_menuChartType.SelectedIndex = _menuChartType.Items.IndexOf(settings.ChartType.ToString());

			_buttonArgumentTotals.Checked = settings.ShowArgumentTotal;
			_buttonSeriesTotal.Checked = settings.ShowSeriesTotal;
			_buttonExplainTotal.Checked = settings.ExplainTotal;

			foreach (var tab in _tabs)
				tab.ResumeLayout();

			//_panelFields.Visible = true;
			_applyingSettings = false;
		}

		private void displayAxis(TabHeaderControl tab, List<string> fields, List<SortOrder> fieldsSort)
		{
			tab.Count = 0;

			for (int i = 0; i < fields.Count; i++)
			{
				if (string.IsNullOrEmpty(fields[i]))
					continue;

				tab.AddTab(createTabId(fields[i]),
					_fields.ByName[fields[i]].Alias,
					_sortIconsOrder[(int) fieldsSort[i]]);
			}
		}

		private ReportSettings readSettings()
		{
			var result = new ReportSettings
			{
				DataSource = (DataSource) Enum.Parse(typeof(DataSource), (string) _menuDataSource.SelectedItem),
				ApplyFilter = _buttonApplyFilter.Checked,
				LabelDataElement = (DataElement) Enum.Parse(typeof(DataElement), (string) _menuLabelDataElement.SelectedItem),
				ChartType = (SeriesChartType) Enum.Parse(typeof(SeriesChartType), (string) _menuChartType.SelectedItem),
				ColumnFields = readFields(_tabCols),
				ColumnFieldsSort = readEnum<SortOrder>(_tabCols, _sortIconsOrder),
				SeriesFields = readFields(_tabRows),
				SeriesFieldsSort = readEnum<SortOrder>(_tabRows, _sortIconsOrder),
				SummaryFields = readFields(_tabSummSort),
				SummarySort = readEnum<SortOrder>(_tabSummSort, _sortIconsOrder),
				SummaryFunctions = _tabSumm.Icons
					.Select(icn => _aggregatesOrder[_aggregateIconsOrder.IndexOf(icn)])
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

		private static List<T> readEnum<T>(TabHeaderControl tab, IList<Bitmap> iconsOrder)
		{
			return tab.Icons
				.Select(iconsOrder.IndexOf)
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

		private static readonly IList<string> _aggregatesOrder = new[]
		{
			Aggregates.Sum,
			Aggregates.Count,
			Aggregates.CountDistinct,
			Aggregates.Min,
			Aggregates.Average,
			Aggregates.Max
		};

		private IList<Bitmap> _aggregateIconsOrder;

		private IList<Bitmap> _sortIconsOrder;

		private readonly Dictionary<Button, TabHeaderControl> _tabByButton;
		private readonly TabHeaderControl[] _summTabs;
		private readonly TabHeaderControl[] _tabs;

		private const string SeriesSeparator = " / ";
		private const string ArgumentSeparator = " / ";

		private readonly IList<string> _fieldsOrder;

		private readonly CardRepository _repository;
		private bool _applyingSettings;
		private readonly CheckBox[] _headerButtons;

		private static readonly string[] _priceFields =
		{
			nameof(Card.PriceLow),
			nameof(Card.PriceMid),
			nameof(Card.PriceHigh)
		};

		private static readonly string[] _deckPriceTotalFields =
		{
			nameof(Card.DeckTotalLow),
			nameof(Card.DeckTotalMid),
			nameof(Card.DeckTotalHigh)
		};

		private static readonly string[] _collectionPriceTotalFields =
		{
			nameof(Card.CollectionTotalLow),
			nameof(Card.CollectionTotalMid),
			nameof(Card.CollectionTotalHigh)
		};

		private readonly Button[] _buttons;
		private readonly ComboBox[] _menus;

		private readonly Fields _fields;
		private readonly UiModel _ui;
	}
}