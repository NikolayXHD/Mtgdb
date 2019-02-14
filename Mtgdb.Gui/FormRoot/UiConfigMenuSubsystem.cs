using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class UiConfigMenuSubsystem : IComponent
	{
		public UiConfigMenuSubsystem(
			ComboBox menuUiScale,
			ComboBox menuUiSmallImageQuality,
			ComboBox menuSuggestDownloadMissingImages,
			ComboBox menuImagesCacheCapacity,
			ComboBox menuUndoDepth,
			UiConfigRepository configRepo)
		{
			_menuUiScale = menuUiScale;
			_menuUiSmallImageQuality = menuUiSmallImageQuality;
			_menuSuggestDownloadMissingImages = menuSuggestDownloadMissingImages;
			_menuImagesCacheCapacity = menuImagesCacheCapacity;
			_menuUndoDepth = menuUndoDepth;
			_configRepo = configRepo;

			_menuUiScale.Items.AddRange(new[] { 100, 125, 150, 200 }.Select(formatScalePercent).Cast<object>().ToArray());
			_menuUiSmallImageQuality.Items.AddRange(new object[] { "Normal (LQ)", "High (MQ)" });
			_menuSuggestDownloadMissingImages.Items.AddRange(new object[] { "No", "Yes" });
			_menuImagesCacheCapacity.Items.AddRange(new [] { 100, 300, 1000, 3000 }.Select(formatInt).Cast<object>().ToArray());
			_menuUndoDepth.Items.AddRange(new [] { 100, 300, 1000, 3000 }.Select(formatInt).Cast<object>().ToArray());

			var config = _configRepo.Config;

			UiScalePercent = config.UiScalePercent;
			UseSmallImages = config.DisplaySmallImages;
			SuggestDownloadMissingImages = config.SuggestDownloadMissingImages;
			ImageCacheCapacity = config.ImageCacheCapacity;
			UndoDepth = config.UndoDepth;

			_menuUiScale.SelectedIndexChanged += handleUiScalePercentChanged;
			_menuUiSmallImageQuality.SelectedIndexChanged += handleMenuChanged;
			_menuSuggestDownloadMissingImages.SelectedIndexChanged += handleMenuChanged;
			_menuImagesCacheCapacity.SelectedIndexChanged += handleMenuChanged;
			_menuUndoDepth.SelectedIndexChanged += handleMenuChanged;
		}

		private void handleUiScalePercentChanged(object sender, EventArgs e)
		{
			if (UiScalePercent > 100)
			{
				_updating = true;
				UseSmallImages = false;
				_updating = false;
			}

			handleConfigChanged();
		}

		private void handleMenuChanged(object sender, EventArgs e)
		{
			if (_updating)
				return;

			handleConfigChanged();
		}

		private static string formatInt(int val) =>
			val.ToString(Str.Culture);

		private static string formatScalePercent(int percent) =>
			percent.ToString(Str.Culture) + ScalePercentSuffix;

		private static int parseScalePercent(string value) =>
			int.Parse(value.Substring(0, value.Length - ScalePercentSuffix.Length), Str.Culture);

		private int UiScalePercent
		{
			get => parseScalePercent((string) _menuUiScale.SelectedItem);
			set => _menuUiScale.SelectedIndex = _menuUiScale.Items.IndexOf(formatScalePercent(value));
		}

		private bool UseSmallImages
		{
			get => _menuUiSmallImageQuality.SelectedIndex == 0;
			set => _menuUiSmallImageQuality.SelectedIndex = value ? 0 : 1;
		}

		private bool SuggestDownloadMissingImages
		{
			get => _menuSuggestDownloadMissingImages.SelectedIndex == 1;
			set => _menuSuggestDownloadMissingImages.SelectedIndex = value ? 1 : 0;
		}

		private int ImageCacheCapacity
		{
			get => int.Parse((string) _menuImagesCacheCapacity.SelectedItem, Str.Culture);
			set => _menuImagesCacheCapacity.SelectedIndex = _menuImagesCacheCapacity.Items.IndexOf(value.ToString(Str.Culture));
		}

		private int UndoDepth
		{
			get => int.Parse((string) _menuUndoDepth.SelectedItem, Str.Culture);
			set => _menuUndoDepth.SelectedIndex = _menuUndoDepth.Items.IndexOf(value.ToString(Str.Culture));
		}

		private void handleConfigChanged()
		{
			var config = _configRepo.Config;

			config.UiScalePercent = UiScalePercent;
			config.DisplaySmallImages = UseSmallImages;
			config.SuggestDownloadMissingImages = SuggestDownloadMissingImages;
			config.ImageCacheCapacity = ImageCacheCapacity;
			config.UndoDepth = UndoDepth;

			_configRepo.Save();

			Dpi.Set(config.UiScalePercent);
		}

		public void Dispose()
		{
			_menuUiScale.SelectedIndexChanged -= handleUiScalePercentChanged;
			_menuUiSmallImageQuality.SelectedIndexChanged -= handleMenuChanged;
			_menuSuggestDownloadMissingImages.SelectedIndexChanged -= handleMenuChanged;
			_menuImagesCacheCapacity.SelectedIndexChanged -= handleMenuChanged;
			_menuUndoDepth.SelectedIndexChanged -= handleMenuChanged;
		}

		public ISite Site { get; set; }
		public event EventHandler Disposed;

		private bool _updating;

		private const string ScalePercentSuffix = " %";

		private readonly ComboBox _menuUiScale;
		private readonly ComboBox _menuUiSmallImageQuality;
		private readonly ComboBox _menuSuggestDownloadMissingImages;
		private readonly ComboBox _menuImagesCacheCapacity;
		private readonly ComboBox _menuUndoDepth;
		private readonly UiConfigRepository _configRepo;
	}
}