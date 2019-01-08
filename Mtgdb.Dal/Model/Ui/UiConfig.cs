using System.ComponentModel;

namespace Mtgdb.Dal
{
	public class UiConfig
	{
		[DefaultValue(DefaultUiScalePercent)]
		public int UiScalePercent { get; set; } = DefaultUiScalePercent;
		
		[DefaultValue(true)]
		public bool DisplaySmallImages { get; set; } = true;

		[DefaultValue(true)]
		public bool SuggestDownloadMissingImages { get; set; } = true;

		[DefaultValue(DefaultCacheCapacity)]
		public int ImageCacheCapacity { get; set; } = DefaultCacheCapacity;

		[DefaultValue(DefaultUndoDepth)]
		public int UndoDepth { get; set; } = DefaultUndoDepth;

		public const int DefaultUiScalePercent = 100;
		private const int DefaultCacheCapacity = 100;
		private const int DefaultUndoDepth = 100;
	}
}