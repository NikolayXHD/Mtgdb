using System;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class UiConfigMenuSubsystem
	{
		public UiConfigMenuSubsystem(ComboBox menuUiScale, ComboBox menuUiSmallImageQuality, UiConfigRepository configRepo)
		{
			_menuUiScale = menuUiScale;
			_menuUiSmallImageQuality = menuUiSmallImageQuality;
			_configRepo = configRepo;
		}

		public void SetupMenu()
		{
			var menuItemTexts = _configRepo.UiScaleValues
				.Select(scale => scale.ToString(Str.Culture) + " %")
				.Cast<object>()
				.ToArray();

			_menuUiScale.Items.AddRange(menuItemTexts);
			UiScalePercent = _configRepo.Config.UiScalePercent;

			_menuUiSmallImageQuality.Items.AddRange(new object[] { "Normal (LQ)", "High (MQ)" });
			UseSmallImages = _configRepo.Config.DisplaySmallImages;

			_menuUiSmallImageQuality.SelectedIndexChanged += handleUiSmallImageQualityChanged;
			_menuUiScale.SelectedIndexChanged += handleUiScalePercentChanged;
		}

		private void handleUiScalePercentChanged(object sender, EventArgs e)
		{
			if (_updatingMenuValues)
				return;

			if (UiScalePercent > 100)
				UseSmallImages = false;

			handleConfigChanged();
		}

		private void handleUiSmallImageQualityChanged(object sender, EventArgs e)
		{
			if (_updatingMenuValues)
				return;

			handleConfigChanged();
		}

		private int UiScalePercent
		{
			get => _configRepo.UiScaleValues[_menuUiScale.SelectedIndex];
			set
			{
				_updatingMenuValues = true;
				_menuUiScale.SelectedIndex = _configRepo.UiScaleValues.IndexOf(value);
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

		private void handleConfigChanged()
		{
			_configRepo.Config.UiScalePercent = UiScalePercent;
			_configRepo.Config.DisplaySmallImages = UseSmallImages;
			_configRepo.Save();

			Dpi.Initialize(_configRepo.Config.UiScalePercent);
		}

		private bool _updatingMenuValues;

		private readonly ComboBox _menuUiScale;
		private readonly ComboBox _menuUiSmallImageQuality;
		private readonly UiConfigRepository _configRepo;
	}
}