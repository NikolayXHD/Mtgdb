using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Newtonsoft.Json;
using NLog;

namespace Mtgdb.Gui
{
	public class ChartFilesSubsystem
	{
		public ChartFilesSubsystem(
			FormChart formChart, 
			CustomCheckBox buttonSave,
			CustomCheckBox buttonLoad,
			CustomCheckBox buttonMruFiles,
			ContextMenuStrip menuMruFiles)
		{
			_formChart = formChart;
			_buttonSave = buttonSave;
			_buttonLoad = buttonLoad;
			_buttonMruFiles = buttonMruFiles;
			_menuMruFiles = menuMruFiles;

			_buttonSubsystem.SetupPopup(new Popup(_menuMruFiles, _buttonMruFiles, HorizontalAlignment.Right, beforeShow: updateMruFilesMenu));
		}

		public void SubscribeToEvents()
		{
			_buttonSubsystem.SubscribeToEvents();

			_buttonSave.Click += handleSaveClick;
			_buttonLoad.Click += handleLoadClick;
		}

		private void saveChart()
		{
			var dlg = new SaveFileDialog
			{
				DefaultExt = Ext,
				InitialDirectory = SaveDirectory,
				FileName = DefaultFileName,
				AddExtension = true,
				Filter = _filter,
				Title = "Save chart settings",
				CheckPathExists = true
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			var settings = _formChart.ReadUiSettings();
			var serialized = JsonConvert.SerializeObject(settings, Formatting.Indented);

			try
			{
				File.WriteAllText(dlg.FileName, serialized);
			}
			catch (Exception ex)
			{
				_log.Error(ex);
				MessageBox.Show($"Failed to write `{dlg.FileName}`, {ex}");
				return;
			}

			_formChart.Title = Path.GetFileNameWithoutExtension(dlg.FileName);
		}

		private void loadChart()
		{
			var dlg = new OpenFileDialog
			{
				DefaultExt = Ext,
				InitialDirectory = SaveDirectory,
				FileName = DefaultFileName,
				AddExtension = true,
				Filter = _filter,
				Title = "Load chart settings",
				CheckFileExists = true
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			load(dlg.FileName);
		}

		private void load(string fileName)
		{
			string serialized;
			try
			{
				serialized = File.ReadAllText(fileName);
			}
			catch (Exception ex)
			{
				_log.Error(ex);
				MessageBox.Show($"Failed to open `{fileName}`, {ex}");
				return;
			}

			ReportSettings settings;
			try
			{
				settings = JsonConvert.DeserializeObject<ReportSettings>(serialized);
			}
			catch (Exception ex)
			{
				_log.Error(ex);
				MessageBox.Show($"Failed to read chart from `{fileName}`, {ex}");
				return;
			}

			_formChart.Title = Path.GetFileNameWithoutExtension(fileName);
			_formChart.LoadSavedChart(settings);
		}

		private IEnumerable<string> getSavedCharts()
		{
			return Directory
				.GetFiles(SaveDirectory, "*" + Ext, SearchOption.TopDirectoryOnly)
				.Select(Path.GetFileNameWithoutExtension);
		}

		private void loadSavedChart(string name) =>
			load(SaveDirectory.AddPath(name + Ext));

		private void updateMruFilesMenu()
		{
			foreach (ToolStripMenuItem menuItem in _menuMruFiles.Items)
				menuItem.Click -= handleMruMenuClick;
			
			_menuMruFiles.Items.Clear();

			foreach (string chartName in getSavedCharts())
			{
				var menuItem = new ToolStripMenuItem(chartName);
				menuItem.Click += handleMruMenuClick;
				_menuMruFiles.Items.Add(menuItem);
			}
		}

		private void handleMruMenuClick(object s, EventArgs e) =>
			loadSavedChart(((ToolStripMenuItem) s).Text);

		private void handleSaveClick(object s, EventArgs e) =>
			saveChart();

		private void handleLoadClick(object s, EventArgs e) =>
			loadChart();

		private const string Ext = ".chart";
		private static readonly string _filter = $"Mtgdb.Gui chart settings (*{Ext})|*{Ext}";
		private string SaveDirectory { get; } = AppDir.Charts;
		private string DefaultFileName =>
			string.IsNullOrEmpty(_formChart.Title) ? null : _formChart.Title + Ext;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		private readonly FormChart _formChart;
		private readonly CustomCheckBox _buttonSave;
		private readonly CustomCheckBox _buttonLoad;
		private readonly CustomCheckBox _buttonMruFiles;
		private readonly ContextMenuStrip _menuMruFiles;
		private readonly ButtonSubsystem _buttonSubsystem = new ButtonSubsystem();
	}
}