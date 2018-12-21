using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using NLog;

namespace Mtgdb.Gui
{
	public class ChartFilesSubsystem
	{
		public ChartFilesSubsystem(IFormChart formChart)
		{
			_formChart = formChart;
		}

		public void SaveChart()
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

			var settings = _formChart.ReadSettings();
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

		public void LoadChart()
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

			string serialized;
			try
			{
				serialized = File.ReadAllText(dlg.FileName);
			}
			catch (Exception ex)
			{
				_log.Error(ex);
				MessageBox.Show($"Failed to open `{dlg.FileName}`, {ex}");
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
				MessageBox.Show($"Failed to read chart from `{dlg.FileName}`, {ex}");
				return;
			}

			_formChart.Title = Path.GetFileNameWithoutExtension(dlg.FileName);
			_formChart.BuildCustomChart(settings);
		}

		private const string Ext = ".chart";
		private static readonly string _filter = $"Mtgdb.Gui chart settings (*{Ext})|*{Ext}";
		private string SaveDirectory { get; } = AppDir.Charts;
		private string DefaultFileName =>
			string.IsNullOrEmpty(_formChart.Title) ? null : _formChart.Title + Ext;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		private readonly IFormChart _formChart;
	}
}