using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using NLog;
using ButtonBase = Mtgdb.Controls.ButtonBase;

namespace Mtgdb.Gui
{
	public class ChartFilesSubsystem : IComponent
	{
		public ChartFilesSubsystem(
			FormChart formChart,
			ButtonBase buttonSave,
			ButtonBase buttonLoad,
			ContextMenuStrip menuMruFiles)
		{
			_formChart = formChart;
			_buttonSave = buttonSave;
			_buttonLoad = buttonLoad;
			_menuMruFiles = menuMruFiles;

			_buttonSave.Pressed += handleSaveClick;
			_buttonLoad.Pressed += handleLoadClick;
		}

		private void saveChart()
		{
			var dlg = new SaveFileDialog
			{
				DefaultExt = Ext,
				InitialDirectory = SaveDirectory.Value,
				FileName = DefaultFileName.Value,
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
				new FsPath(dlg.FileName).WriteAllText(serialized);
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
				InitialDirectory = SaveDirectory.Value,
				FileName = DefaultFileName.Value,
				AddExtension = true,
				Filter = _filter,
				Title = "Load chart settings",
				CheckFileExists = true
			};

			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			load(new FsPath(dlg.FileName));
		}

		private void load(FsPath fileName)
		{
			string serialized;
			try
			{
				serialized = fileName.ReadAllText();
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

			_formChart.Title = fileName.Basename(extension: false);
			_formChart.LoadSavedChart(settings);
		}

		private IEnumerable<string> getSavedCharts()
		{
			return SaveDirectory.EnumerateFiles("*" + Ext)
				.Select(_ => _.Basename(extension: false));
		}

		private void loadSavedChart(string name) =>
			load(SaveDirectory.Join(name + Ext));

		public void UpdateMruFilesMenu()
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

		public void Dispose()
		{
			_buttonSave.Pressed -= handleSaveClick;
			_buttonLoad.Pressed -= handleLoadClick;
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		public ISite Site { get; set; }
		public event EventHandler Disposed;

		private const string Ext = ".chart";
		private static readonly string _filter = $"Mtgdb.Gui chart settings (*{Ext})|*{Ext}";
		private FsPath SaveDirectory { get; } = AppDir.Charts;
		private FsPath DefaultFileName =>
			string.IsNullOrEmpty(_formChart.Title) ? FsPath.None : new FsPath(_formChart.Title + Ext);

		private readonly FormChart _formChart;
		private readonly ButtonBase _buttonSave;
		private readonly ButtonBase _buttonLoad;
		private readonly ContextMenuStrip _menuMruFiles;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
