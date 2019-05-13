using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Mtgdb.Controls
{
	public static class ControlHelpers
	{
		public const AnchorStyles AnchorAll =
			AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		public static int GetTrueIndexPositionFromPoint(this FixedRichTextBox rtb, Point point)
		{
			// https://docs.microsoft.com/en-us/windows/desktop/controls/em-charfrompos
			const int EM_CHARFROMPOS = 0x00D7;
			var pointPtr = Marshal.AllocHGlobal(Marshal.SizeOf(point));
			try
			{
				Marshal.StructureToPtr(point, pointPtr, fDeleteOld: false);
				int index = (int) SendMessage(rtb.Handle, EM_CHARFROMPOS, IntPtr.Zero, pointPtr);
				return index;
			}
			finally
			{
				Marshal.FreeHGlobal(pointPtr);
			}
		}

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(Point pt);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool SetForegroundWindow(HandleRef hWnd);

		public static Form ParentForm(this Control c)
		{
			var current = c;
			while (current.Parent != null)
				current = current.Parent;

			return current as Form;
		}

		public static bool Invoke(this Control value, Action method)
		{
			if (value.IsDisposed || value.Disposing || !value.IsHandleCreated)
				return false;

			try
			{
				value.Invoke(method);
			}
			catch (ObjectDisposedException)
			{
			}

			return true;
		}

		public static void TouchColorProperties(this Control control)
		{
			var color = control.BackColor;
			control.BackColor = Color.Black;
			control.BackColor = color;

			color = control.ForeColor;
			control.ForeColor = Color.Black;
			control.ForeColor = color;
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

		public static void SetTag<TValue>(this Control control, TValue value) =>
			SetTag(control, typeof(TValue).FullName, value);

		public static TValue GetTag<TValue>(this Control control, string key)
		{
			if (control.Tag == null)
				control.Tag = new Dictionary<string, object>();

			var dict = (Dictionary<string, object>) control.Tag;

			if (!dict.TryGetValue(key, out var result))
				return default;

			return (TValue) result;
		}

		public static TValue GetTag<TValue>(this Control control) =>
			GetTag<TValue>(control, typeof(TValue).FullName);

		public static Rectangle GetBorderRectangle(this Control control, int focusBorderWidth)
		{
			var rectangle = new Rectangle(default, control.Size);
			rectangle.Inflate(-focusBorderWidth / 2, -focusBorderWidth / 2);
			if (focusBorderWidth % 2 == 1)
			{
				rectangle.Width -= 1;
				rectangle.Height -= 1;
			}

			return rectangle;
		}

		public static void PaintBorder(this Control c, Graphics graphics, AnchorStyles borders, Color borderColor, DashStyle dashStyle)
		{
			if (borderColor == Color.Transparent || borderColor == Color.Empty || borderColor.A == 0)
				return;

			var pen = new Pen(borderColor)
			{
				DashStyle = dashStyle
			};

			if ((borders & AnchorStyles.Top) > 0)
				graphics.DrawLine(pen, 0, 0, c.Width - 1, 0);

			if ((borders & AnchorStyles.Bottom) > 0)
				graphics.DrawLine(pen, 0, c.Height - 1, c.Width - 1, c.Height - 1);

			if ((borders & AnchorStyles.Left) > 0)
				graphics.DrawLine(pen, 0, 0, 0, c.Height - 1);

			if ((borders & AnchorStyles.Right) > 0)
				graphics.DrawLine(pen, c.Width - 1, 0, c.Width - 1, c.Height - 1);
		}

		public static void PaintPanelBack(this Control c, Graphics g, Rectangle clipRect, Image backImage, Color backColor, bool paintBack)
		{
			var isVisualStyleSupported = VisualStyleRenderer.IsSupported;

			if (!paintBack || isTransparent(backColor))
			{
				if (isVisualStyleSupported)
					ButtonRenderer.DrawParentBackground(g, clipRect, c);
				else
				{
					if (!paintBack || !isVisible(backColor))
					{
						var current = c.Parent;
						while (current != null)
						{
							var bg = current.BackColor;
							if (isVisible(bg))
							{
								g.FillRectangle(new SolidBrush(Color.FromArgb(255, bg)), c.ClientRectangle);
								break;
							}

							current = current.Parent;
						}
					}
				}
			}

			if (!paintBack)
				return;

			if (isVisible(backColor))
			{
				if (isVisualStyleSupported)
					g.FillRectangle(new SolidBrush(backColor), c.ClientRectangle);
				else
					g.FillRectangle(new SolidBrush(Color.FromArgb(255, backColor)), c.ClientRectangle);
			}

			if (backImage != null)
				g.DrawImage(backImage, backImage.GetRect());

			bool isVisible(Color color) =>
				color != Color.Empty && color != Color.Transparent && color.A > 0;

			bool isTransparent(Color color) =>
				color == Color.Empty || color == Color.Transparent || color.A < 255;
		}

		public static bool IsUnderMouse(this Control c) =>
			c.Handle.Equals(WindowFromPoint(Cursor.Position));

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

		public static bool TryCopyToClipboard(this string selectedText)
		{
			if (!string.IsNullOrEmpty(selectedText))
			{
				try
				{
					Clipboard.SetText(selectedText);
					return true;
				}
				catch (ExternalException)
				{
				}
			}

			return false;
		}

		/// <summary>
		/// Convert <see cref="Message.LParam"/> to <see cref="Point"/>
		/// </summary>
		public static Point ToPoint(this IntPtr lParam) =>
			new Point(unchecked((int)lParam.ToInt64()));

		public static IntPtr ToIntPtr(this Point point)
		{
			unchecked
			{
				return ToIntPtr((short) point.X, (short) point.Y);
			}
		}

		public static IntPtr ToIntPtr(short lowWord, short highWord) =>
			new IntPtr(((highWord & 0x0000ffff) << 16) | (lowWord & 0x0000ffff));

		public static short HighWord(this IntPtr number) =>
			HighWord(number.ToInt64());

		public static short LowWord(this IntPtr number) =>
			LowWord(number.ToInt64());

		public static short HighWord(this long number) =>
			unchecked((short)(number >> 16));

		public static short LowWord(this long number) =>
			unchecked((short)(number & 0x0000ffff));
	}
}