using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Mtgdb.Bitmaps;

namespace Mtgdb.Controls
{
	public static class TextDrawingHelper
	{
		public static Size MeasureText(this Graphics g, string text, Font font) =>
			TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), _textFormatFlags);

		public static Size MeasureText(this Graphics g, string text, Font font, Size size, TextFormatFlags flags) =>
			TextRenderer.MeasureText(g, text, font, size, flags);

		public static void DrawText(this Graphics g, string text, Font font, Rectangle rect, Color color) =>
			TextRenderer.DrawText(g, text, font, rect, color, _textFormatFlags);

		public static void DrawText(this Graphics g, string text, Font font, Color color, Color borderColor, float opaqueBorder, float fadeBorder, Rectangle rect)
		{
			var dw = (int) fadeBorder;
			using (var bmp = new Bitmap(rect.Width + 2 * dw, rect.Height + 2 * dw))
			using (var gb = Graphics.FromImage(bmp))
			{
				gb.Clear(borderColor);
				TextRenderer.DrawText(gb, text, font, new Point(dw, dw), color, borderColor, _textFormatFlags);
				new SemiTransparentShadowTransformation(bmp, borderColor, opaqueBorder, fadeBorder).Execute();
				g.DrawImageUnscaled(bmp, rect.X - dw, rect.Y - dw);
			}
		}

		private static readonly TextFormatFlags _textFormatFlags =
			new StringFormat(default(StringFormatFlags)).toTextFormatFlags();

		private static TextFormatFlags toTextFormatFlags(this StringFormat strFormat)
		{
			TextFormatFlags textFormatFlags = TextFormatFlags.Default;
			switch (strFormat.Trimming)
			{
				case StringTrimming.Character:
				case StringTrimming.Word:
				case StringTrimming.EllipsisCharacter:
					textFormatFlags |= TextFormatFlags.EndEllipsis;
					break;
				case StringTrimming.EllipsisWord:
					textFormatFlags |= TextFormatFlags.WordEllipsis;
					break;
				case StringTrimming.EllipsisPath:
					textFormatFlags |= TextFormatFlags.PathEllipsis;
					break;
			}

			switch (strFormat.HotkeyPrefix)
			{
				case HotkeyPrefix.None:
					textFormatFlags |= TextFormatFlags.NoPrefix;
					break;
				case HotkeyPrefix.Hide:
					textFormatFlags |= TextFormatFlags.HidePrefix;
					break;
			}

			StringFormatFlags formatFlags = strFormat.FormatFlags;
			bool rightToLeft = (formatFlags & StringFormatFlags.DirectionRightToLeft) != 0;
			switch (strFormat.Alignment)
			{
				case StringAlignment.Near:
					textFormatFlags |= rightToLeft ? TextFormatFlags.Right : TextFormatFlags.Default;
					break;
				case StringAlignment.Center:
					textFormatFlags |= TextFormatFlags.HorizontalCenter;
					break;
				case StringAlignment.Far:
					textFormatFlags |= rightToLeft ? TextFormatFlags.Default : TextFormatFlags.Right;
					break;
			}

			switch (strFormat.LineAlignment)
			{
				case StringAlignment.Near:
					break;
				case StringAlignment.Center:
					textFormatFlags |= TextFormatFlags.VerticalCenter;
					break;
				case StringAlignment.Far:
					textFormatFlags |= TextFormatFlags.Bottom;
					break;
			}

			if ((formatFlags & StringFormatFlags.NoWrap) == 0)
				textFormatFlags |= TextFormatFlags.WordBreak;

			if (rightToLeft)
				textFormatFlags |= TextFormatFlags.RightToLeft;

			TextFormatFlags result =
				textFormatFlags |
				TextFormatFlags.NoPadding | TextFormatFlags.NoClipping |
				TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.TextBoxControl;

			return result;
		}
	}
}