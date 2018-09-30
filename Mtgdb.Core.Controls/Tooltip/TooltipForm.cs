using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class TooltipForm : ShadowedForm
	{
		public TooltipForm()
		{
			BackColor = SystemColors.Window;
			FormBorderStyle = FormBorderStyle.None;

			_tooltipSize = new Size(400, 300).ByDpi();
			_closeIcon = Properties.Resources.close_tab_hovered_32.HalfResizeDpi();
			_selectableTextIcon = Properties.Resources.selectable_transp_64.HalfResizeDpi();

			ControlBox = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(-10000, -10000);
			TopMost = true;
			KeyPreview = false;

			_panel = new BorderedPanel
			{
				BackColor = BackColor,
				Dock = DockStyle.Fill
			};

			Controls.Add(_panel);

			_tooltipTextbox = new FixedRichTextBox
			{
				Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
				ReadOnly = true,
				TabStop = false,
				ScrollBars = RichTextBoxScrollBars.None,
				WordWrap = true,
				Size = new Size(Width - TextPadding * 2, Height - TextPadding * 2),
				Location = new Point(TextPadding, TextPadding),
				Margin = new Padding(TextPadding),
				BackColor = BackColor,
				BorderStyle = BorderStyle.None,
				HideSelection = true,
				Font = new Font(new FontFamily("Tahoma"), 9.75f, FontStyle.Regular, GraphicsUnit.Point),
				AutoWordSelection = false
			};

			_tooltipTextbox.MouseDown += text_MouseDown;
			_tooltipTextbox.MouseClick += text_MouseClick;

			_tooltipTextbox.KeyDown += text_keyDown;
			_tooltipTextbox.LostFocus += text_lostFocus;

			_panel.Controls.Add(_tooltipTextbox);

			_selectionSubsystem = new RichTextBoxSelectionSubsystem(_tooltipTextbox);
			_selectionSubsystem.SubscribeToEvents();

			_tooltipFocusTarget = new Control
			{
				Size = new Size(1, 1),
				Location = new Point(-1, -1)
			};

			_panel.Controls.Add(_tooltipFocusTarget);

			_buttonClose = new Button
			{
				TabStop = false,
				Anchor = AnchorStyles.Right | AnchorStyles.Top,

				FlatStyle = FlatStyle.Flat,
				FlatAppearance =
				{
					BorderSize = 0,
					MouseOverBackColor = Color.Transparent,
					MouseDownBackColor = Color.Transparent,
					CheckedBackColor = Color.Transparent,
					// setting Color.Transparent would lead to an exception
					BorderColor = Color.FromArgb(0, 255, 255, 255)
				}
			};

			_buttonClose.Click += closeClick;

			_panel.Controls.Add(_buttonClose);
			_buttonClose.BringToFront();

			setCloseEnabled(false);

			Resize += resize;
			ColorSchemeController.SystemColorsChanging += systemColorsChanging;


			Show();
		}

		private void systemColorsChanging()
		{
			var color = _tooltipTextbox.BackColor;
			_tooltipTextbox.BackColor = Color.Black;
			_tooltipTextbox.BackColor = color;

			color = _tooltipTextbox.ForeColor;
			_tooltipTextbox.ForeColor = Color.Black;
			_tooltipTextbox.ForeColor = color;
		}

		private void setCloseEnabled(bool value)
		{
			_closeEnabled = value;

			if (value)
			{
				_buttonClose.Location = new Point(_panel.Width - _closeIcon.Width - 1, 1);
				_buttonClose.BackgroundImage = _closeIcon;
				_buttonClose.Size = _closeIcon.Size;
			}
			else
			{
				_buttonClose.Location = new Point(_panel.Width - _selectableTextIcon.Width - 3, 3);
				_buttonClose.Size = _selectableTextIcon.Size;
				_buttonClose.BackgroundImage = _selectableTextIcon;
			}
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
			_buttonClose.Visible = Clickable;

			Size = _tooltipSize;

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
							_tooltipTextbox.SelectionBackColor = tooltip.HighlightOptions.HighlightContextColor;
						else
							_tooltipTextbox.SelectionBackColor = tooltip.HighlightOptions.HighlightColor;
					}
			}

			_tooltipTextbox.SelectionStart = 0;
			_tooltipTextbox.SelectionLength = 0;

			var size = measureTooltip(tooltip);
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
			return bounds;
		}

		private static Rectangle selectTooltipBounds(Size size, Rectangle target, Rectangle workingArea, Point cursor)
		{
			var bounds = new Rectangle(target.Location, size);

			if (bounds.Width > workingArea.Width)
				bounds.Width = workingArea.Width;

			if (bounds.Height > workingArea.Height)
				bounds.Height = workingArea.Height;

			int atTop = target.Top - size.Height;
			int atBtm = target.Bottom;
			int atRht = target.Right;
			int atLft = target.Left - size.Width;

			int byTop = target.Top;
			int byMid = cursor.Y - size.Height / 2;
			int byBtm = target.Bottom - size.Height;

			int byRht = target.Right - size.Width;
			int byCtr = cursor.X - size.Width / 2;
			int byLft = target.Left;

			var toBtm = new Point(0, TooltipMargin);
			var toTop = new Point(0, -TooltipMargin);
			var toRht = new Point(TooltipMargin, 0);
			var toLft = new Point(-TooltipMargin, 0);

			var candidates = new List<TooltipPosition>
			{
				new TooltipPosition(size, byLft, atBtm, toBtm),
				new TooltipPosition(size, byCtr, atBtm, toBtm),
				new TooltipPosition(size, byRht, atBtm, toBtm),

				new TooltipPosition(size, byLft, atTop, toTop),
				new TooltipPosition(size, byCtr, atTop, toTop),
				new TooltipPosition(size, byRht, atTop, toTop),

				new TooltipPosition(size, atRht, byTop, toRht),
				new TooltipPosition(size, atRht, byMid, toRht),
				new TooltipPosition(size, atRht, byBtm, toRht),

				new TooltipPosition(size, atLft, byTop, toLft),
				new TooltipPosition(size, atLft, byMid, toLft),
				new TooltipPosition(size, atLft, byBtm, toLft)
			};

			for (int i = 0; i < candidates.Count; i++)
			{
				var c = candidates[i];

				c.Bounds = c.Bounds.OffsetInto(workingArea);
				c.IntersectsTarget = target.IntersectsWith(c.Bounds);
				c.DistanceToCursor = c.Bounds.Center().Minus(cursor)
					.MultiplyIfNegative(new PointF(1.5f, 1.5f)) // prefer bottom right
					.SquareNorm();
			}

			var candidate = candidates
				.AtMin(_ => _.IntersectsTarget)
				.ThenAtMin(_ => _.DistanceToCursor)
				.Find();

			return candidate.Bounds;
		}

		private Size measureTooltip(TooltipModel tooltip)
		{
			string measuredContentText;
			if (!string.IsNullOrEmpty(tooltip.Title) && !string.IsNullOrEmpty(tooltip.Text))
				measuredContentText = "\n" + tooltip.Text;
			else
				measuredContentText = tooltip.Text;

			const TextFormatFlags formatFlags = TextFormatFlags.GlyphOverhangPadding |
				TextFormatFlags.NoPadding |
				TextFormatFlags.Left |
				TextFormatFlags.TextBoxControl |
				TextFormatFlags.WordBreak;

			var graphics = _tooltipTextbox.CreateGraphics();

			Size titleSize;
			if (string.IsNullOrEmpty(tooltip.Title))
				titleSize = new Size(0, 0);
			else
				titleSize = graphics.MeasureText(
					tooltip.Title,
					new Font(_tooltipTextbox.Font, FontStyle.Bold),
					_tooltipTextbox.Size,
					formatFlags);

			Size contentSize;
			if (string.IsNullOrEmpty(measuredContentText))
				contentSize = new Size(0, 0);
			else
			{
				contentSize = graphics.MeasureText(
					measuredContentText,
					_tooltipTextbox.Font,
					_tooltipTextbox.Size,
					formatFlags);
			}

			int contentWidth = Math.Max(titleSize.Width, contentSize.Width);

			// ideographic strings are under-measured
			int cjkTermH = tooltip.Text.IsCjk() ? 32 : 0;

			if (Clickable)
				cjkTermH = Math.Max(cjkTermH, titleSize.Width - contentWidth + (int) (_buttonClose.Width * 1.25f) - TextPadding);

			int cjkTermV = tooltip.Text.IsCjk() ? 6 : 0;

			var cjkTermSize = new Size(cjkTermH, cjkTermV).ByDpi();

			var size = new Size(
				contentWidth + _tooltipTextbox.Margin.Horizontal + 2 + cjkTermSize.Width,
				titleSize.Height + contentSize.Height + _tooltipTextbox.Margin.Vertical + cjkTermSize.Height);

			return size;
		}

		public void HideTooltip()
		{
			if (_tooltipTextbox.Focused || _buttonClose.Focused)
				_tooltip.Control.Focus();

			_selectionSubsystem.Reset();

			Location = new Point(-10000, -10000);
			UserInteracted = false;
			Clickable = false;
		}


		private void text_MouseDown(object sender, MouseEventArgs e)
		{
			if (!Clickable)
				return;

			UserInteracted = true;
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
			if (_closeEnabled)
				HideTooltip();
			else
				setCloseEnabled(true);
		}


		[Category("Settings"), DefaultValue(typeof(Color), "Window")]
		public sealed override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UserInteracted
		{
			get => _userInteracted;
			private set
			{
				_userInteracted = value;
				setCloseEnabled(value);
			}
		}



		private const int TextPadding = 6;
		private const int TooltipMargin = 12;

		private readonly Size _tooltipSize;

		private readonly RichTextBoxSelectionSubsystem _selectionSubsystem;
		private readonly FixedRichTextBox _tooltipTextbox;
		private readonly Control _tooltipFocusTarget;
		private readonly Button _buttonClose;

		public bool Clickable
		{
			get => _clickable;

			private set
			{
				_clickable = value;
				_selectionSubsystem.SelectionEnabled = value;
			}
		}

		private bool _userInteracted;
		private TooltipModel _tooltip;
		private readonly BorderedPanel _panel;
		private bool _closeEnabled;
		private readonly Bitmap _closeIcon;
		private readonly Bitmap _selectableTextIcon;
		private bool _clickable;

		private class TooltipPosition
		{
			public TooltipPosition(Size size, int left, int top, Point offset)
			{
				Bounds = new Rectangle(left + offset.X, top + offset.Y, size.Width, size.Height);
			}

			public Rectangle Bounds { get; set; }

			public float DistanceToCursor { get; set; }
			public bool IntersectsTarget { get; set; }
		}
	}
}