using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using CheckBox = Mtgdb.Controls.CheckBox;

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
			CheckBox checkboxAllPanels,
			CheckBox checkboxTopPanel,
			CheckBox checkboxRightPanel,
			CheckBox checkboxSearchBar,
			CardRepository cardRepository,
			UiConfigRepository configRepo)
		{
			_menuUiScale = menuUiScale;
			_menuUiSmallImageQuality = menuUiSmallImageQuality;
			_menuSuggestDownloadMissingImages = menuSuggestDownloadMissingImages;
			_menuImagesCacheCapacity = menuImagesCacheCapacity;
			_menuUndoDepth = menuUndoDepth;
			_checkboxAllPanels = checkboxAllPanels;
			_checkboxTopPanel = checkboxTopPanel;
			_checkboxRightPanel = checkboxRightPanel;
			_checkboxSearchBar = checkboxSearchBar;

			_allPanelCheckboxes = new[]
			{
				_checkboxTopPanel,
				_checkboxRightPanel,
				_checkboxSearchBar
			};

			_configRepo = configRepo;
			_cardRepository = cardRepository;

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

			ShowTopPanel = config.ShowTopPanel;
			ShowRightPanel = config.ShowRightPanel;
			ShowSearchBar = config.ShowSearchBar;

			_checkboxAllPanels.Checked = allPanelsChecked(true);

			_menuUiScale.SelectedIndexChanged += handleUiScalePercentChanged;
			_menuUiSmallImageQuality.SelectedIndexChanged += handleSmallImageQualityChanged;
			_menuSuggestDownloadMissingImages.SelectedIndexChanged += handleMenuChanged;
			_menuImagesCacheCapacity.SelectedIndexChanged += handleMenuChanged;
			_menuUndoDepth.SelectedIndexChanged += handleMenuChanged;

			_checkboxAllPanels.CheckedChanged += handleAllPanelsChanged;
			_checkboxTopPanel.CheckedChanged += handlePanelVisibilityChanged;
			_checkboxRightPanel.CheckedChanged += handlePanelVisibilityChanged;
			_checkboxSearchBar.CheckedChanged += handlePanelVisibilityChanged;
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

		private void handleSmallImageQualityChanged(object sender, EventArgs e)
		{
			handleConfigChanged();

			foreach (var card in _cardRepository.Cards)
				card.ResetImageModel();
		}

		private void handlePanelVisibilityChanged(object sender, EventArgs e)
		{
			bool allChecked = allPanelsChecked(true);
			if (_checkboxAllPanels.Checked != allChecked)
			{
				_updating = true;
				_checkboxAllPanels.Checked = allChecked;
				_updating = false;
			}

			handleConfigChanged();
		}



		private bool allPanelsChecked(bool value) =>
			_allPanelCheckboxes.All(_ => _.Checked == value);

		private void setAllPanelsChecked(bool value) =>
			_allPanelCheckboxes.ForEach(_ => _.Checked = value);



		private void handleAllPanelsChanged(object sender, EventArgs e)
		{
			if (_updating)
				return;

			if (allPanelsChecked(_checkboxAllPanels.Checked))
				return;

			_updating = true;
			setAllPanelsChecked(_checkboxAllPanels.Checked);
			_updating = false;

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

		private bool ShowTopPanel
		{
			get => _checkboxTopPanel.Checked;
			set => _checkboxTopPanel.Checked = value;
		}

		private bool ShowRightPanel
		{
			get => _checkboxRightPanel.Checked;
			set => _checkboxRightPanel.Checked = value;
		}

		private bool ShowSearchBar
		{
			get => _checkboxSearchBar.Checked;
			set => _checkboxSearchBar.Checked = value;
		}

		private void handleConfigChanged()
		{
			var config = _configRepo.Config;

			config.UiScalePercent = UiScalePercent;
			config.DisplaySmallImages = UseSmallImages;
			config.SuggestDownloadMissingImages = SuggestDownloadMissingImages;
			config.ImageCacheCapacity = ImageCacheCapacity;
			config.UndoDepth = UndoDepth;
			config.ShowTopPanel = ShowTopPanel;
			config.ShowRightPanel = ShowRightPanel;
			config.ShowSearchBar = ShowSearchBar;

			_configRepo.Save();

			Dpi.Set(config.UiScalePercent);
			Application.OpenForms.OfType<FormRoot>().ForEach(f =>
			{
				for (int i = 0; i < f.TabsCount; i++)
					f.GetTab(i).SetPanelVisibility(config);
			});
		}

		public void Dispose()
		{
			_menuUiScale.SelectedIndexChanged -= handleUiScalePercentChanged;
			_menuUiSmallImageQuality.SelectedIndexChanged -= handleMenuChanged;
			_menuSuggestDownloadMissingImages.SelectedIndexChanged -= handleMenuChanged;
			_menuImagesCacheCapacity.SelectedIndexChanged -= handleMenuChanged;
			_menuUndoDepth.SelectedIndexChanged -= handleMenuChanged;

			_checkboxTopPanel.CheckedChanged -= handleAllPanelsChanged;
			_checkboxTopPanel.CheckedChanged -= handlePanelVisibilityChanged;
			_checkboxRightPanel.CheckedChanged -= handlePanelVisibilityChanged;
			_checkboxSearchBar.CheckedChanged -= handlePanelVisibilityChanged;

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
		private readonly CheckBox _checkboxAllPanels;
		private readonly CheckBox _checkboxTopPanel;
		private readonly CheckBox _checkboxRightPanel;
		private readonly CheckBox _checkboxSearchBar;
		private readonly UiConfigRepository _configRepo;
		private readonly CardRepository _cardRepository;

		private readonly CheckBox[] _allPanelCheckboxes;
	}
}