using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public static class ControlHelpers
	{
		public static int GetTrueIndexPositionFromPoint(this FixedRichTextBox rtb, Point pt)
		{
			POINT wpt = new POINT(pt.X, pt.Y);
			int index = (int)SendMessage(new HandleRef(rtb, rtb.Handle), EM_CHARFROMPOS, 0, wpt);

			return index;
		}

		[DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, POINT lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(Point pt);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool SetForegroundWindow(HandleRef hWnd);

		public static void ScaleDpi(this Control control)
		{
			control.Size = control.Size.ByDpi();
		}

		public static void Invoke(this Control value, Action method)
		{
			if (value.IsDisposed || value.Disposing || !value.IsHandleCreated)
				return;

			try
			{
				value.Invoke(method);
			}
			catch (ObjectDisposedException)
			{
			}
		}

		public static TextFormatFlags ToTextFormatFlags(this StringFormat strFormat)
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



		public static List<T> Reorder<T>(this IList<T> originalArray, int fromIndex, int toIndex)
		{
			var copy = originalArray.ToList();

			if (fromIndex >= 0)
				copy.RemoveAt(fromIndex);

			if (toIndex >= 0)
				copy.Insert(toIndex, originalArray[fromIndex]);
			else
				copy.Add(originalArray[fromIndex]);

			return copy;
		}



		public static Point PointToClient(this Control control, Control targetControl, Point targetLocation)
		{
			targetLocation = targetControl.PointToScreen(targetLocation);
			targetLocation = control.PointToClient(targetLocation);
			return targetLocation;
		}


		public static void SetTag<TValue>(this Control control, string key, TValue value)
		{
			if (control.Tag == null)
				control.Tag = new Dictionary<string, object>();

			var dict = (Dictionary<string, object>) control.Tag;
			dict[key] = value;
		}

		public static void SetTag<TValue>(this Control control, TValue value)
		{
			SetTag(control, typeof(TValue).FullName, value);
		}

		public static TValue GetTag<TValue>(this Control control, string key)
		{
			if (control.Tag == null)
				control.Tag = new Dictionary<string, object>();

			var dict = (Dictionary<string, object>)control.Tag;

			if (!dict.TryGetValue(key, out var result))
				return default(TValue);

			return (TValue) result;
		}

		public static TValue GetTag<TValue>(this Control control)
		{
			return GetTag<TValue>(control, typeof(TValue).FullName);
		}

		public static void PaintPanel(this Control c, Graphics graphics, AnchorStyles borders, Color borderColor)
		{
			graphics.Clear(c.BackColor);
			var pen = new Pen(borderColor);

			if (c.BackgroundImage != null)
				graphics.DrawImage(c.BackgroundImage, new Rectangle(Point.Empty, c.BackgroundImage.Size));

			if ((borders & AnchorStyles.Top) > 0)
				graphics.DrawLine(pen, 0, 0, c.Width - 1, 0);

			if ((borders & AnchorStyles.Bottom) > 0)
				graphics.DrawLine(pen, 0, c.Height - 1, c.Width - 1, c.Height - 1);

			if ((borders & AnchorStyles.Left) > 0)
				graphics.DrawLine(pen, 0, 0, 0, c.Height - 1);

			if ((borders & AnchorStyles.Right) > 0)
				graphics.DrawLine(pen, c.Width - 1, 0, c.Width - 1, c.Height - 1);
		}

		public static bool IsUnderMouse(this Control c)
		{
			return c.Handle.Equals(WindowFromPoint(Cursor.Position));
		}

		public static bool IsChildUnderMouse(this Control c)
		{
			var position = Cursor.Position;
			var handle = WindowFromPoint(position);

			while (true)
			{
				var clientPosition = c.PointToClient(position);
				var child = c.GetChildAtPoint(clientPosition);

				if (child == null || child == c)
				{
					bool result = c.Handle.Equals(handle);
					return result;
				}

				c = child;
			}
		}

		private const int EM_CHARFROMPOS = 0x00D7;

		[StructLayout(LayoutKind.Sequential)]
		private class POINT
		{
			public int x;
			public int y;

			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}
	}
}