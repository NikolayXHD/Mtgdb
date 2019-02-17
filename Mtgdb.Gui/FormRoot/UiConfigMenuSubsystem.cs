using System;
using System.ComponentModel;
using System.Linq;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class UiConfigMenuSubsystem : IComponent
	{
		public UiConfigMenuSubsystem(
			DropDown menuUiScale,
			DropDown menuUiSmallImageQuality,
			DropDown menuSuggestDownloadMissingImages,
			DropDown menuImagesCacheCapacity,
			DropDown menuUndoDepth,
			UiConfigRepository configRepo)
		{
			_menuUiScale = menuUiScale;
			_menuUiSmallImageQuality = menuUiSmallImageQuality;
			_menuSuggestDownloadMissingImages = menuSuggestDownloadMissingImages;
			_menuImagesCacheCapacity = menuImagesCacheCapacity;
			_menuUndoDepth = menuUndoDepth;
			_configRepo = configRepo;

			_menuUiScale.SetMenuValues(new[] { 100, 125, 150, 200 }.Select(formatScalePercent));
			_menuUiSmallImageQuality.SetMenuValues("Normal (LQ)", "High (MQ)");
			_menuSuggestDownloadMissingImages.SetMenuValues("No", "Yes");
			_menuImagesCacheCapacity.SetMenuValues(new [] { 100, 300, 1000, 3000 }.Select(formatInt));
			_menuUndoDepth.SetMenuValues(new [] { 100, 300, 1000, 3000 }.Select(formatInt));

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
			get => parseScalePercent(_menuUiScale.SelectedValue);
			set => _menuUiScale.SelectedIndex = _menuUiScale.MenuValues.IndexOf(formatScalePercent(value));
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
			get => int.Parse(_menuImagesCacheCapacity.SelectedValue, Str.Culture);
			set => _menuImagesCacheCapacity.SelectedIndex = _menuImagesCacheCapacity.MenuValues.IndexOf(value.ToString(Str.Culture));
		}

		private int UndoDepth
		{
			get => int.Parse(_menuUndoDepth.SelectedValue, Str.Culture);
			set => _menuUndoDepth.SelectedIndex = _menuUndoDepth.MenuValues.IndexOf(value.ToString(Str.Culture));
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

			Disposed?.Invoke(this, EventArgs.Empty);
		}

		public ISite Site { get; set; }
		public event EventHandler Disposed;

		private bool _updating;

		private const string ScalePercentSuffix = " %";

		private readonly DropDown _menuUiScale;
		private readonly DropDown _menuUiSmallImageQuality;
		private readonly DropDown _menuSuggestDownloadMissingImages;
		private readonly DropDown _menuImagesCacheCapacity;
		private readonly DropDown _menuUndoDepth;
		private readonly UiConfigRepository _configRepo;
	}
}