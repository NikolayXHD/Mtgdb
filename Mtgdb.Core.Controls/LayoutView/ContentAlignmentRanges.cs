using System.Drawing;

namespace Mtgdb.Controls
{
	public class ContentAlignmentRanges
	{
		public const ContentAlignment AnyLeft = ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft;
		public const ContentAlignment AnyCenter = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
		public const ContentAlignment AnyRight = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

		public const ContentAlignment AnyTop = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
		public const ContentAlignment AnyMiddle = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
		public const ContentAlignment AnyBottom = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
	}
}