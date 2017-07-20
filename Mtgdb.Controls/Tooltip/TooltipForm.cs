using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Lucene.Net.Contrib;

namespace Mtgdb.Controls
{
	public class TooltipForm : ShadowedForm
	{
		public TooltipForm()
		{
			BackColor = Color.White;
			FormBorderStyle = FormBorderStyle.None;
			ControlBox = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(-10000, -10000);
			TopMost = true;
			KeyPreview = false;

			var panel = new CustomPanel
			{
				BorderColor = BorderColor,
				BackColor = BackColor,
				Dock = DockStyle.Fill
			};

			Controls.Add(panel);

			_tooltipTextbox = new FixedRichTextBox
			{
				Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
				ReadOnly = true,
				TabStop = false,
				ScrollBars = RichTextBoxScrollBars.None,
				WordWrap = true,
				Size = new Size(Width - TextPadding*2, Height - TextPadding*2),
				Location = new Point(TextPadding, TextPadding),
				Margin = new Padding(TextPadding),
				BackColor = BackColor,
				BorderStyle = BorderStyle.None,
				HideSelection = true,
				Font = new Font(new FontFamily("Tahoma"), 8.25f, FontStyle.Regular),
				AutoWordSelection = false
			};

			_tooltipTextbox.AutoWordSelection = true;
			_tooltipTextbox.AutoWordSelection = false;

			_tooltipTextbox.MouseDown += text_MouseDown;
			_tooltipTextbox.MouseClick += text_MouseClick;
			_tooltipTextbox.MouseMove += text_MouseMove;
			_tooltipTextbox.MouseUp += text_MouseUp;
			_tooltipTextbox.KeyDown += text_keyDown;
			_tooltipTextbox.LostFocus += text_lostFocus;

			_titleTextbox = new FixedRichTextBox
			{
				Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
				ReadOnly = true,
				TabStop = false,
				ScrollBars = RichTextBoxScrollBars.None,
				WordWrap = true,
				Size = new Size(Width - TextPadding*2, Height - TextPadding*2),
				Location = new Point(TextPadding, TextPadding),
				Margin = new Padding(TextPadding),
				BackColor = BackColor,
				BorderStyle = BorderStyle.None,
				HideSelection = true,
				Font = new Font(new FontFamily("Tahoma"), 8.25f, FontStyle.Bold)
			};

			panel.Controls.Add(_tooltipTextbox);

			_tooltipFocusTarget = new Control
			{
				Size = new Size(1, 1),
				Location = new Point(-1, -1)
			};

			panel.Controls.Add(_tooltipFocusTarget);

			_buttonClose = new Button
			{
				Anchor = AnchorStyles.Right | AnchorStyles.Top,
				FlatStyle = FlatStyle.Flat,
				BackgroundImage = Properties.Resources.close_tab_hovered_16,
				Size = Properties.Resources.close_tab_hovered_16.Size,
				Location = new Point(panel.Width - Properties.Resources.close_tab_hovered_16.Width - 1, 1),
				TabStop = false
			};

			_buttonClose.FlatAppearance.BorderSize = 0;
			_buttonClose.FlatAppearance.MouseOverBackColor = Color.Transparent;
			_buttonClose.FlatAppearance.MouseDownBackColor = Color.Transparent;
			_buttonClose.Click += closeClick;

			panel.Controls.Add(_buttonClose);
			_buttonClose.BringToFront();
			_buttonClose.Visible = false;

			this.Resize += resize;

			Show();
		}

		private void resize(object sender, EventArgs e)
		{
			if (WindowState != FormWindowState.Normal)
				WindowState = FormWindowState.Normal;
		}



		public void ShowTooltip(TooltipModel tooltip)
		{
			_tooltip = tooltip;
			Clickable = tooltip.Clickable;

			Size = TooltipSize;

			var titleFont = new Font(_tooltipTextbox.Font, FontStyle.Bold);

			_tooltipTextbox.ResetText();
			if (!string.IsNullOrEmpty(tooltip.Title))
			{
				_tooltipTextbox.AppendText(tooltip.Title);
				_tooltipTextbox.Select(0, tooltip.Title.Length);
				_tooltipTextbox.SelectionFont = titleFont;
			}


			if (!string.IsNullOrWhiteSpace(tooltip.Text))
			{
				if (_tooltipTextbox.TextLength > 0)
					_tooltipTextbox.AppendText("\n\n");

				var titleLength = _tooltipTextbox.TextLength;

				_tooltipTextbox.AppendText(tooltip.Text);

				if (tooltip.HighlightRanges != null)
					foreach (var range in tooltip.HighlightRanges)
					{
						_tooltipTextbox.SelectionStart = titleLength + range.Index;
						_tooltipTextbox.SelectionLength = range.Length;

						if (range.IsContext)
							_tooltipTextbox.SelectionBackColor = tooltip.HighlightSettings.HighlightContextColor;
						else
							_tooltipTextbox.SelectionBackColor = tooltip.HighlightSettings.HighlightColor;
					}
			}

			_tooltipTextbox.SelectionStart = 0;
			_tooltipTextbox.SelectionLength = 0;

			var size = measureTooltip(tooltip, titleFont);
			var bounds = allocateTooltip(size, tooltip.Control.RectangleToScreen(tooltip.ObjectBounds));

			if (_tooltipTextbox.Focused)
				_tooltipFocusTarget.Focus();

			if (tooltip.Clickable)
				_tooltipTextbox.Cursor = Cursors.IBeam;
			else
				_tooltipTextbox.Cursor = Cursors.Arrow;

			Size = bounds.Size;
			Application.DoEvents();
			Location = bounds.Location;
		}

		private static Rectangle allocateTooltip(Size size, Rectangle target)
		{
			var cursor = Cursor.Position;
			var workingArea = Screen.FromPoint(cursor).WorkingArea;
			var bounds = selectTooltipBounds(size, target, workingArea, cursor);
			bounds = fitTooltipBounds(bounds, workingArea);
			return bounds;
		}

		private static Rectangle fitTooltipBounds(Rectangle bounds, Rectangle workingArea)
		{
			if (bounds.Right > workingArea.Right)
				bounds.X = workingArea.Right - bounds.Width;

			if (bounds.X < workingArea.Left)
				bounds.X = workingArea.Left;

			if (bounds.Bottom > workingArea.Bottom)
				bounds.Y = workingArea.Bottom - bounds.Height;

			if (bounds.Y < workingArea.Top)
				bounds.Y = workingArea.Top;

			return bounds;
		}

		private static Rectangle selectTooltipBounds(Size size, Rectangle target, Rectangle workingArea, Point cursor)
		{
			var bounds = new Rectangle(target.Location, size);

			if (bounds.Width > workingArea.Width)
				bounds.Width = workingArea.Width;

			if (bounds.Height > workingArea.Height)
				bounds.Height = workingArea.Height;

			var candidates = new List<TooltipPosition>
			{
				// bottom - left
				new TooltipPosition
				{
					Bounds = new Rectangle(
						new Point(
							target.Left,
							target.Bottom),
						size),
					Offset = new Point(0, TooltipMargin),
					IsNearCursor = target.Bottom - cursor.Y < target.Width,
					IsAlongCursor = target.Left + size.Width > cursor.X,
					IsWhithinScreen = target.Bottom + size.Height <= workingArea.Bottom,
				},

				// bottom - cursor
				new TooltipPosition
				{
					Bounds = new Rectangle(
						new Point(
							cursor.X - size.Width / 2,
							target.Bottom),
						size),
					Offset = new Point(0, TooltipMargin),
					IsNearCursor = target.Bottom - cursor.Y < target.Width,
					IsAlongCursor = true,
					IsWhithinScreen = target.Bottom + size.Height <= workingArea.Bottom
				},

				// right - top
				new TooltipPosition
				{
					Bounds = new Rectangle(
						new Point(
							target.Right,
							target.Top),
						size),
					Offset = new Point(TooltipMargin, 0),
					IsNearCursor = target.Right - cursor.X < target.Height,
					IsAlongCursor = target.Top + size.Height > cursor.Y,
					IsWhithinScreen = target.Right + size.Width <= workingArea.Right
				},

				// right - cursor
				new TooltipPosition
				{
					Bounds = new Rectangle(
						new Point(
							target.Right,
							cursor.Y - size.Height / 2),
						size),
					Offset = new Point(TooltipMargin, 0),
					IsNearCursor = target.Right - cursor.X < target.Height,
					IsAlongCursor = true,
					IsWhithinScreen = target.Right + size.Width <= workingArea.Right
				},

				// left - top
				new TooltipPosition
				{
					Bounds = new Rectangle(
						new Point(
							target.Left - size.Width,
							target.Top),
						size),
					Offset = new Point(-TooltipMargin, 0),
					IsNearCursor = cursor.X - target.Left < target.Height,
					IsAlongCursor = target.Top + size.Height > cursor.Y,
					IsWhithinScreen = target.Left - size.Width > workingArea.Left
				},

				// left - cursor
				new TooltipPosition
				{
					Bounds = new Rectangle(
						new Point(
							target.Left - size.Width,
							cursor.Y - size.Height / 2),
						size),
					Offset = new Point(-TooltipMargin, 0),
					IsNearCursor = cursor.X - target.Left < target.Height,
					IsAlongCursor = true,
					IsWhithinScreen = target.Left - size.Width > workingArea.Left
				},

				// top - left
				new TooltipPosition
				{
					Bounds = new Rectangle(
						new Point(
							target.Left,
							target.Top - size.Height),
						size),
					Offset = new Point(0, -TooltipMargin),
					IsNearCursor = cursor.Y - target.Top < target.Width,
					IsAlongCursor = target.Left + size.Width > cursor.X,
					IsWhithinScreen = target.Top - size.Height > workingArea.Top
				},

				// top - cursor
				new TooltipPosition
				{
					Bounds = new Rectangle(
						new Point(
							cursor.X - size.Width / 2,
							target.Top - size.Height),
						size),
					Offset = new Point(0, -TooltipMargin),
					IsNearCursor = cursor.Y - target.Top < target.Width,
					IsAlongCursor = true,
					IsWhithinScreen = target.Top - size.Height > workingArea.Top
				}
			};

			var candidate = candidates
				.OrderByDescending(_ => _.IsAlongCursor)
				.ThenByDescending(_ => _.IsNearCursor && _.IsWhithinScreen)
				.ThenByDescending(_ => _.IsWhithinScreen)
				.ThenByDescending(_ => _.IsNearCursor)
				.First();

			var rectangle = candidate.Bounds;
			rectangle.Offset(candidate.Offset);

			return rectangle;
		}

		private Size measureTooltip(TooltipModel tooltip, Font titleFont)
		{
			string measuredContentText;
			if (!string.IsNullOrEmpty(tooltip.Title) && !string.IsNullOrEmpty(tooltip.Text))
				measuredContentText = "\n" + tooltip.Text;
			else
				measuredContentText = tooltip.Text;

			var formatFlags =
				TextFormatFlags.GlyphOverhangPadding |
				TextFormatFlags.NoPadding |
				TextFormatFlags.Left |
				TextFormatFlags.TextBoxControl |
				TextFormatFlags.WordBreak;

			Size titleSize;
			if (string.IsNullOrEmpty(tooltip.Title))
				titleSize = new Size(0, 0);
			else
				titleSize = TextRenderer.MeasureText(_titleTextbox.CreateGraphics(), tooltip.Title, titleFont, _tooltipTextbox.Size, formatFlags);

			Size contentSize;
			if (string.IsNullOrEmpty(measuredContentText))
				contentSize = new Size(0, 0);
			else
				contentSize = TextRenderer.MeasureText(_tooltipTextbox.CreateGraphics(),
					measuredContentText,
					_tooltipTextbox.Font,
					_tooltipTextbox.Size,
					formatFlags);

			int contentWidth = Math.Max(titleSize.Width, contentSize.Width);

			// строки из иероглифов недополучают при измерении
			int cjkTermH = tooltip.Text.IsCjk() ? 16 : 0;
			int cjkTermV = tooltip.Text.IsCjk() ? 6 : 0;

			if (Clickable)
				cjkTermH = Math.Max(cjkTermH, titleSize.Width - contentWidth + (int) (_buttonClose.Width*1.25f) - TextPadding);


			var size = new Size(
				contentWidth + _tooltipTextbox.Margin.Horizontal + 2 + cjkTermH,
				titleSize.Height + contentSize.Height + _tooltipTextbox.Margin.Vertical + cjkTermV);

			return size;
		}

		public void HideTooltip()
		{
			if (_tooltipTextbox.Focused || _buttonClose.Focused)
				_tooltip.Control.Focus();

			_selectionStart = -1;
			_selectionManual = false;
			Location = new Point(-10000, -10000);
			UserInteracted = false;
			Clickable = false;
		}


		private void text_MouseDown(object sender, MouseEventArgs e)
		{
			if (!Clickable)
				return;

			if (e.Button == MouseButtons.Left)
			{
				int current = _tooltipTextbox.GetTrueIndexPositionFromPoint(e.Location);
				if (current == _selectionStart)
					_selectionManual = true;

				UserInteracted = true;
			}
		}

		private void text_MouseUp(object sender, MouseEventArgs e)
		{
			if (!Clickable)
				return;

			_selectionStart = -1;
			_selectionManual = false;
		}

		private void text_MouseMove(object sender, MouseEventArgs e)
		{
			if (!Clickable)
				return;

			if (!_selectionManual)
			{
				if (!_tooltipTextbox.Focused)
					_tooltipTextbox.SelectionStart = _selectionStart = _tooltipTextbox.GetTrueIndexPositionFromPoint(e.Location);
			}
			else
			{
				var current = _tooltipTextbox.GetTrueIndexPositionFromPoint(e.Location);
				if (current != _selectionStart)
				{
					_tooltipTextbox.SelectionStart = Math.Min(current, _selectionStart);
					_tooltipTextbox.SelectionLength = Math.Abs(current - _selectionStart);
				}
			}
		}



		private void text_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				HideTooltip();
		}

		private void text_lostFocus(object sender, EventArgs e)
		{
			if (UserInteracted)
				HideTooltip();
		}

		private void text_keyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
				HideTooltip();
		}

		private void closeClick(object sender, EventArgs e)
		{
			HideTooltip();
		}


		[Category("Settings"), DefaultValue(typeof(Color), "White")]
		public sealed override Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		[Category("Settings"), DefaultValue(typeof(Color), "Gray")]
		public Color BorderColor { get; set; } = Color.Gray;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UserInteracted
		{
			get { return _userInteracted; }
			private set
			{
				_userInteracted = value;
				_buttonClose.Visible = value;
			}
		}



		private const int TextPadding = 6;
		private const int TooltipMargin = 12;

		private static readonly Size TooltipSize = new Size(300, 300);

		private int _selectionStart;
		private bool _selectionManual;
		private readonly FixedRichTextBox _tooltipTextbox;
		private readonly FixedRichTextBox _titleTextbox;
		private readonly Control _tooltipFocusTarget;
		private readonly Button _buttonClose;

		public bool Clickable { get; private set; }
		private bool _userInteracted;
		private TooltipModel _tooltip;

		private class TooltipPosition
		{
			public Rectangle Bounds { get; set; }
			public Point Offset { get; set; }

			public bool IsNearCursor { get; set; }
			public bool IsAlongCursor { get; set; }
			public bool IsWhithinScreen { get; set; }
		}
	}
}