using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;
using Mtgdb.Gui.Resx;
using ReadOnlyCollectionsExtensions;

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
				_buttonCollectionColors
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

			RegisterDragControl(_flowTitle);

			foreach (var button in _headerButtons)
				RegisterDragControl(button);

			_labelTitle.Text = string.Empty;

			_menuMruFiles.ForeColor = SystemColors.ControlText;
			_menuMruFiles.BackColor = SystemColors.Control;
		}

		public FormChart(CardRepository repository, Func<UiModel> uiSnapshotFactory, CardFields fields)
			: this()
		{
			_fields = fields;
			_fieldsOrder = fields.ChartFields.OrderBy(_ => _fields.ByName[_].Alias)
				.ToArray();

			_repository = repository;
			_uiSnapshotFactory = uiSnapshotFactory;

			Load += load;

			foreach (var button in _headerButtons)
			{
				button.AutoCheck = false;
				button.Click += buttonClick;
			}

			_tabByButton = Enumerable.Range(0, _buttons.Length)
				.ToDictionary(i => _buttons[i], i => _tabs[i]);

			foreach (var button in _buttons)
				button.Click += buttonAddFieldClick;

			_buttonApply.Click += buttonApplyClick;

			for (int i = 0; i < _tabs.Length; i++)
			{
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

			_menuPrice.Items.AddRange(new object[] { "Low", "Mid", "High" });
			_menuPrice.SelectedIndex = 1;

			_menuPrice.SelectedIndexChanged += priceMenuIndexChanged;
			_menuPriceChartType.SelectedIndexChanged += priceMenuIndexChanged;
			_menuChartType.SelectedIndexChanged += chartTypeChanged;

			foreach (var menu in _menus)
				ManualMenuPainter.SetupComboBox(menu, allowScroll: true);

			_filesSubsystem = new ChartFilesSubsystem(this, _buttonSave, _buttonLoad, _buttonMruFiles, _menuMruFiles);
			_filesSubsystem.SubscribeToEvents();

			_sortIconsOrder = new[]
			{
				LayoutControlBitmaps.SortNone,
				LayoutControlBitmaps.SortAsc,
				LayoutControlBitmaps.SortDesc
			}.AsReadOnlyList();

			_aggregateIconsOrder = new[]
			{
				ResourcesFilter.sum_hovered,
				ResourcesFilter.count_hovered,
				ResourcesFilter.count_distinct_hovered,
				ResourcesFilter.min_hovered,
				ResourcesFilter.avg_hovered,
				ResourcesFilter.max_hovered
			}.AsReadOnlyList();

			var defaultIcons = new[]
			{
				_sortIconsOrder[0],
				_sortIconsOrder[0],
				_aggregateIconsOrder[0],
				_sortIconsOrder[0]
			};

			for (int i = 0; i < _tabs.Length; i++)
				_tabs[i].DefaultIcon = defaultIcons[i];

			scale();
		}

		private void scale()
		{
			this.ScaleDpi();

			_tabs.ForEach(t => t.ScaleDpi(bmp => bmp?.HalfResizeDpi()));

			_buttons.Cast<Control>()
				.Concat(_headerButtons)
				.Append(_buttonApply)
				.Append(_buttonSave)
				.Append(_buttonLoad)
				.ForEach(ControlScaler.ScaleDpi);

			_menus.ForEach(ControlScaler.ScaleDpi);

			_sortIconsScaler.Setup(this);
			_aggregateIconsScaler.Setup(this);

			new Control[]
			{
				_labelField,
				_labelDataElement,
				_labelDataSource,
				_labelChartType,
				_labelCols,
				_labelRows,
				_labelSum,
				_labelSummarySort,

				_buttonArgumentTotals,
				_buttonSeriesTotal,
				_buttonExplainTotal,
				_buttonFilterBySearchResult,

				_menuMruFiles,
				_labelTitle,

				_chart
			}.ForEach(ControlScaler.ScaleDpiFont);

			new DpiScaler<FormChart>(form =>
			{
				form._buttonSubsystem.SetupButton(form._buttonSave, ButtonImages.ScaleDpi((Resources.save_16, Resources.save_32)));
				form._buttonSubsystem.SetupButton(form._buttonLoad, ButtonImages.ScaleDpi((Resources.open_16, Resources.open_32)));
				form._buttonSubsystem.SetupButton(form._buttonMruFiles, ButtonImages.ScaleDpi((null, Resources.down_32)));
			}).Setup(this);
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

			buildChartFromUi();
		}

		private void buttonApplyClick(object sender, EventArgs e)
		{
			buildChartFromUi();
		}

		private void buildChartFromUi()
		{
			foreach (var checkBox in _headerButtons)
				checkBox.Checked = false;

			buildChart();
		}

		public void LoadSavedChart(ReportSettings settings)
		{
			foreach (var checkBox in _headerButtons)
				checkBox.Checked = false;

			buildChart(settings);
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

			var index = ((int) tabSetting.Tag + 1) % _sortIconsOrder.Count;
			var sortIcon = _sortIconsOrder[index];

			tab.SetTabSetting(tabSetting.TabId, new TabSettings(sortIcon)
			{
				Tag = index
			});
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

			var index = (int) tabSetting.Tag;
			while (true)
			{
				index = (index + 1) % _aggregateIconsOrder.Count;
				var aggregate = _aggregatesOrder[index];
				if (field.IsNumeric || aggregate == Aggregates.Count || aggregate == Aggregates.CountDistinct)
					break;
			}

			var aggregateIcon = _aggregateIconsOrder[index];
			tab.SetTabSetting(tabSetting.TabId, new TabSettings(aggregateIcon)
			{
				Tag = index
			});
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
					tab.AddTab(tabId, field.Alias, tag: 0);
				else
				{
					int index = _aggregatesOrder.IndexOf(Aggregates.Count);
					tab.AddTab(tabId, field.Alias, _aggregateIconsOrder[index], tag: index);
				}
			}
			else
			{
				if (tab.TabIds.Select(getFieldName).Any(fn => fn == fieldName))
					return;

				tab.AddTab(tabId, field.Alias, tag: 0);
			}
		}



		private static ChartArea createArea()
		{
			var area = new ChartArea();
			area.AxisX.MajorGrid.LineWidth = 0;
			area.AxisX.MinorGrid.LineWidth = 0;
			area.AxisX.Interval = 1;

			area.BackColor = SystemColors.Window;
			area.BorderColor = SystemColors.ActiveBorder;
			area.BackSecondaryColor = SystemColors.Control;

			foreach (var ax in area.Axes)
			{
				ax.TitleForeColor = SystemColors.WindowText;
				ax.LineColor = SystemColors.WindowText;
				ax.MajorTickMark.LineColor = SystemColors.WindowText;
				ax.MinorTickMark.LineColor = SystemColors.WindowText;
				ax.InterlacedColor = SystemColors.WindowText;
				ax.LabelStyle.ForeColor = SystemColors.WindowText;
				ax.MajorGrid.LineColor = SystemColors.ActiveBorder;

				ax.TitleFont = ax.TitleFont.ByDpi();
			}

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
			foreach (CustomCheckBox button in _headerButtons)
				button.Checked = button == sender;

			var checkBox = (CheckBox) sender;
			loadReport(checkBox);
		}

		private void loadReport(CheckBox button)
		{
			ReportSettings settings = null;

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
			}

			if (settings != null)
			{
				Title = button.Text;
				buildChart(settings);
			}
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

		private void buildChartData(ReportSettings settings)
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
				.Select(_ => applySort(_, sortedArgumentIndices))
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
			_chart = createChart();

			var colorTransformation = new ColorSchemeTransformation(null);

			var originalPalette = ChartPalettes.OriginalByName[_chart.Palette];
			var palette = ChartPalettes.ByName[_chart.Palette];

			for (int i = 0; i < palette.Length; i++)
				palette[i] = colorTransformation.TransformColor(originalPalette[i]);

			var summaryFields = settings.SummaryFields;

			var metadata = ChartTypeMetadata.ByType[settings.ChartType];

			if (metadata.CanDisplayMultipleSeries)
				_chart.Legends.Add(createLegend());

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
							ChartArea = area.Name,

							LabelForeColor = SystemColors.WindowText,
							BorderColor = SystemColors.ActiveBorder,
							MarkerColor = SystemColors.ActiveBorder
						};

						chartSeries.Font = chartSeries.Font.ByDpi();

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
					var legend = createLegend();

					legend.DockedToChartArea = area.Name;
					legend.IsDockedInsideChartArea = false;

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
							legend.CustomItems.Add(SystemColors.ActiveBorder, argumentLegend[0]);
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

		private Chart createChart()
		{
			var cellPosition = _panelTable.GetCellPosition(_chart);
			_panelTable.Controls.Remove(_chart);
			_chart.Dispose();

			var chart = new Chart
			{
				BackColor = SystemColors.Window,
				ForeColor = SystemColors.WindowText,
				BorderColor = SystemColors.ActiveBorder,
				BorderlineColor = SystemColors.ActiveBorder,

				Dock = DockStyle.Fill
			};

			_panelTable.Controls.Add(chart, cellPosition.Column, cellPosition.Row);

			return chart;
		}

		private static Legend createLegend()
		{
			var legend = new Legend
			{
				BackColor = SystemColors.Window,
				ForeColor = SystemColors.WindowText,
				TitleForeColor = SystemColors.WindowText,
				TitleBackColor = SystemColors.Window,
				BorderColor = SystemColors.ActiveBorder
			};

			legend.Font = legend.Font.ByDpi();
			legend.TitleFont = legend.TitleFont.ByDpi();

			return legend;
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

					scrollBar.BackColor = SystemColors.Window;
					scrollBar.ButtonColor = SystemColors.Control;
					scrollBar.LineColor = SystemColors.Control;

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

		private void buildChart(ReportSettings settings = null)
		{
			if (settings == null)
			{
				settings = ReadUiSettings();
				if (settings.SetDefaultValues())
					display(settings);
			}
			else
			{
				settings.SetDefaultValues();
				display(settings);
			}

			buildChartData(settings);
		}

		private IEnumerable<Card> getCards(DataSource source, bool applyFilter)
		{
			IEnumerable<Card> result;

			switch (source)
			{
				case DataSource.Collection:
				{
					var uiSnapshotFactory = _uiSnapshotFactory();
					result = _repository.Cards.Where(_ => _.CollectionCount(uiSnapshotFactory) > 0);
					break;
				}
				case DataSource.Deck:
				{
					var uiSnapshotFactory = _uiSnapshotFactory();
					result = _repository.Cards.Where(_ => _.DeckCount(uiSnapshotFactory) > 0);
					break;
				}
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
				int sortIconIndex = (int) settings.SummarySort[i];
				var tabId = createTabId(settings.SummaryFields[i]);

				_tabSummSort.AddTab(
					tabId,
					_fields.ByName[settings.SummaryFields[i]].Alias,
					_sortIconsOrder[sortIconIndex],
					tag: sortIconIndex);

				int aggregateIconIndex = _aggregatesOrder.IndexOf(settings.SummaryFunctions[i]);
				_tabSumm.AddTab(
					tabId,
					_fields.ByName[settings.SummaryFields[i]].Alias,
					_aggregateIconsOrder[aggregateIconIndex],
					tag: aggregateIconIndex);
			}

			_menuDataSource.SelectedIndex = (int) settings.DataSource;
			_buttonFilterBySearchResult.Checked = settings.ApplyFilter;
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

				int sortIconIndex = (int) fieldsSort[i];

				tab.AddTab(
					createTabId(fields[i]),
					_fields.ByName[fields[i]].Alias,
					_sortIconsOrder[sortIconIndex],
					tag: sortIconIndex);
			}
		}

		public ReportSettings ReadUiSettings()
		{
			var result = new ReportSettings
			{
				DataSource = (DataSource) Enum.Parse(typeof(DataSource), (string) _menuDataSource.SelectedItem),
				ApplyFilter = _buttonFilterBySearchResult.Checked,
				LabelDataElement = (DataElement) Enum.Parse(typeof(DataElement), (string) _menuLabelDataElement.SelectedItem),
				ChartType = (SeriesChartType) Enum.Parse(typeof(SeriesChartType), (string) _menuChartType.SelectedItem),
				ColumnFields = readFields(_tabCols),
				ColumnFieldsSort = readEnum<SortOrder>(_tabCols),
				SeriesFields = readFields(_tabRows),
				SeriesFieldsSort = readEnum<SortOrder>(_tabRows),
				SummaryFields = readFields(_tabSummSort),
				SummarySort = readEnum<SortOrder>(_tabSummSort),
				SummaryFunctions = _tabSumm.Tags
					.Cast<int>()
					.Select(i => _aggregatesOrder[i])
					.ToList(),
				ShowArgumentTotal = _buttonArgumentTotals.Checked,
				ShowSeriesTotal = _buttonSeriesTotal.Checked,
				ExplainTotal = _buttonExplainTotal.Checked
			};

			return result;
		}

		public string Title
		{
			get => _labelTitle.Text;
			set
			{
				_labelTitle.Text = value;
				Text = string.IsNullOrEmpty(value) ? " " : value;
			}
		}

		private static List<string> readFields(TabHeaderControl tab)
		{
			return tab.TabIds.Select(getFieldName).ToList();
		}

		private static List<T> readEnum<T>(TabHeaderControl tab) =>
			tab.Tags.Cast<T>().ToList();



		private void tabSummAdded(TabHeaderControl tab, int addedIndex)
		{
			if (_applyingSettings)
				return;

			_applyingSettings = true;

			var settings = _tabSumm.GetTabSetting(addedIndex);

			var mirrors = _summTabs.Where(_ => _ != tab);
			foreach (var mirror in mirrors)
				mirror.AddTab(settings.TabId, settings.Text, tag: 0);

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

		private IReadOnlyList<Bitmap> _aggregateIconsOrder;

		private IReadOnlyList<Bitmap> _sortIconsOrder;

		private readonly Dictionary<Button, TabHeaderControl> _tabByButton;
		private readonly TabHeaderControl[] _summTabs;
		private readonly TabHeaderControl[] _tabs;

		private const string SeriesSeparator = " / ";
		private const string ArgumentSeparator = " / ";

		private readonly IList<string> _fieldsOrder;

		private readonly CardRepository _repository;
		private readonly Func<UiModel> _uiSnapshotFactory;
		private bool _applyingSettings;
		private readonly CustomCheckBox[] _headerButtons;

		private static readonly DpiScaler<FormChart, IReadOnlyList<Bitmap>> _sortIconsScaler =
			new DpiScaler<FormChart, IReadOnlyList<Bitmap>>(
				f => f._sortIconsOrder,
				(f, bitmaps) => f._sortIconsOrder = bitmaps,
				bitmaps => bitmaps.Select(bmp => bmp.HalfResizeDpi()).ToReadOnlyList());

		private static readonly DpiScaler<FormChart, IReadOnlyList<Bitmap>> _aggregateIconsScaler =
			new DpiScaler<FormChart, IReadOnlyList<Bitmap>>(
				f => f._aggregateIconsOrder,
				(f, bitmaps) => f._aggregateIconsOrder = bitmaps,
				bitmaps => bitmaps.Select(bmp => bmp.HalfResizeDpi()).ToReadOnlyList());

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

		private readonly CardFields _fields;
		private readonly ButtonSubsystem _buttonSubsystem = new ButtonSubsystem();
		private readonly ChartFilesSubsystem _filesSubsystem;
	}
}