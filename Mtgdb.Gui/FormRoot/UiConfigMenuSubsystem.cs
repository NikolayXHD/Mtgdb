using System;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class UiConfigMenuSubsystem
	{
		public UiConfigMenuSubsystem(ComboBox menuUiScale, ComboBox menuUiSmallImageQuality, UiConfigRepository uiConfig)
		{
			_menuUiScale = menuUiScale;
			_menuUiSmallImageQuality = menuUiSmallImageQuality;
			_uiConfig = uiConfig;
		}

		public void SetupMenu()
		{
			var menuItemTexts = _uiConfig.UiScaleValues
				.Select(scale => scale.ToString(Str.Culture) + " %")
				.Cast<object>()
				.ToArray();

			_menuUiScale.Items.AddRange(menuItemTexts);
			UiScalePercent = _uiConfig.Config.UiScalePercent;

			_menuUiSmallImageQuality.Items.AddRange(new object[] { "Normal (LQ)", "High (MQ)" });
			UseSmallImages = _uiConfig.Config.DisplaySmallImages;

			_menuUiSmallImageQuality.SelectedIndexChanged += handleUiSmallImageQualityChanged;
			_menuUiScale.SelectedIndexChanged += handleUiScalePercentChanged;
		}

		private void handleUiScalePercentChanged(object sender, EventArgs e)
		{
			if (_updatingMenuValues)
				return;

			if (UiScalePercent > 100)
				UseSmallImages = false;

			saveConfig();
		}

		private void handleUiSmallImageQualityChanged(object sender, EventArgs e)
		{
			if (_updatingMenuValues)
				return;

			saveConfig();
		}

		private int UiScalePercent
		{
			get => _uiConfig.UiScaleValues[_menuUiScale.SelectedIndex];
			set
			{
				_updatingMenuValues = true;
				_menuUiScale.SelectedIndex = _uiConfig.UiScaleValues.IndexOf(value);
				_updatingMenuValues = false;
			}
		}

		private bool UseSmallImages
		{
			get => _menuUiSmallImageQuality.SelectedIndex == 0;
			set
			{
				_updatingMenuValues = true;
				_menuUiSmallImageQuality.SelectedIndex = value ? 0 : 1;
				_updatingMenuValues = false;
			}
		}

		private void saveConfig()
		{
			_uiConfig.Config.UiScalePercent = UiScalePercent;
			_uiConfig.Config.DisplaySmallImages = UseSmallImages;
			_uiConfig.Save();
		}

		private bool _updatingMenuValues;

		private readonly ComboBox _menuUiScale;
		private readonly ComboBox _menuUiSmallImageQuality;
		private readonly UiConfigRepository _uiConfig;
	}
}