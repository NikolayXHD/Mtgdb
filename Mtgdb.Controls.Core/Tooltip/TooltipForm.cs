using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public class TooltipForm : Form
	{
		[UsedImplicitly] // by ninject
		public TooltipForm()
		{
			FormBorderStyle = FormBorderStyle.None;
			ControlBox = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.Manual;
			if (!Runtime.IsMono)
			{
				TopMost = true;
				Location = _hiddenLocation;
			}

			KeyPreview = false;

			_panel = new BorderedPanel
			{
				Dock = DockStyle.Fill
			};

			Controls.Add(_panel);

			var font = new Font(FontFamily.GenericSansSerif, 9.75f, FontStyle.Regular, GraphicsUnit.Point);
			_titleFont = new Font(font, FontStyle.Bold);
			_tooltipTextbox = new FixedRichTextBox
			{
				Dock = DockStyle.Fill,
				ReadOnly = true,
				TabStop = false,
				ScrollBars = RichTextBoxScrollBars.None,
				WordWrap = true,
				BorderStyle = BorderStyle.None,
				HideSelection = true,
				Font = font,
				AutoWordSelection = false
			};

			TextPadding = new Padding(6, 3, 6, 3);

			_panel.Controls.Add(_tooltipTextbox);

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
					// setting Color.Transparent leads to an exception
					BorderColor = Color.FromArgb(0, 255, 255, 255)
				}
			};

			_panel.Controls.Add(_buttonClose);
			_buttonClose.BringToFront();

			_tooltipTextbox.MouseDown += text_MouseDown;
			_tooltipTextbox.MouseClick += text_MouseClick;

			_tooltipTextbox.KeyDown += text_keyDown;
			_tooltipTextbox.LostFocus += text_lostFocus;

			_buttonClose.Click += closeClick;

			ColorSchemeController.SystemColorsChanging += systemColorsChanging;

			BackColor = SystemColors.Window;

			setupIcons();
		}

		protected override bool ShowWithoutActivation => true;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams baseParams = base.CreateParams;

				if (Runtime.IsMono)
				{
					const int WS_EX_NOACTIVATE = 0x08000000;
					const int WS_EX_TOOLWINDOW = 0x00000080;
					baseParams.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
				}

				return baseParams;
			}
		}

		private void setupIcons()
		{
			_closeIcon = Properties.Resources.close_tab_hovered_32.HalfResizeDpi();
			_selectableTextIcon = Properties.Resources.selectable_transp_64.HalfResizeDpi();
			setCloseEnabled(_closeEnabled);
		}

		public void ScaleDpi()
		{
			_tooltipTextbox.ScaleDpiFont();

			new DpiScaler<TooltipForm>(form => form.setupIcons())
				.Setup(this);

			DpiScalers.Combine(
				new DpiScaler<TooltipForm, int>(
					f => f.TooltipMargin,
					(f, m) => f.TooltipMargin = m,
					m => m.ByDpiWidth()
				),
				new DpiScaler<TooltipForm, Padding>(
					f => f.TextPadding,
					(f, p) => f.TextPadding = p,
					p => p.ByDpi()
				)
			).Setup(this);
		}

		private void systemColorsChanging() =>
			_tooltipTextbox.TouchColorProperties();

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

		public void ShowTooltip(TooltipModel tooltip)
		{
			_tooltip = tooltip;
			Clickable = tooltip.Clickable;
			_buttonClose.Visible = Clickable;

			_tooltipTextbox.ResetText();
			if (!string.IsNullOrEmpty(tooltip.Title))
			{
				_tooltipTextbox.AppendText(tooltip.Title);
				_tooltipTextbox.Select(0, tooltip.Title.Length);
				_tooltipTextbox.SelectionFont = _titleFont;
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

			var size = measureTooltip(tooltip, _titleFont);
			var screenBounds = tooltip.Control.RectangleToScreen(tooltip.ObjectBounds);

			Rectangle bounds;
			if (tooltip.UnderMouse)
			 	bounds = new Rectangle(Cursor.Position - size.MultiplyBy(0.5f).Round(), size);
			else
			{
				var cursor = tooltip.Cursor?.Invoke0(tooltip.Control.PointToScreen);
				bounds = allocateTooltip(size, screenBounds, cursor, TooltipMargin, tooltip.PositionPreference?.Invoke(screenBounds));
			}

			if (_tooltipTextbox.Focused)
				_tooltipFocusTarget.Focus();

			if (tooltip.Clickable)
				_tooltipTextbox.Cursor = Cursors.IBeam;
			else
				_tooltipTextbox.Cursor = Cursors.Arrow;

			Size = bounds.Size;
			if (!Runtime.IsMono)
				Application.DoEvents();
			Location = bounds.Location;
			if (Runtime.IsMono)
				Show();
		}

		private Rectangle allocateTooltip(Size size, Rectangle target, Point? cursor, int margin,
			Func<ExtremumFinder<TooltipPosition>, ExtremumFinder<TooltipPosition>> positionPreference = null)
		{
			cursor ??= Cursor.Position;
			var workingArea = Screen.FromPoint(cursor.Value).WorkingArea;
			var bounds = selectTooltipBounds(size, target, workingArea, cursor.Value, margin, positionPreference);
			return bounds;
		}

		private Rectangle selectTooltipBounds(
			Size size,
			Rectangle target,
			Rectangle workingArea,
			Point cursor,
			int margin,
			Func<ExtremumFinder<TooltipPosition>, ExtremumFinder<TooltipPosition>> positionPreference = null)
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

			var toBtm = new Point(0, margin);
			var toTop = new Point(0, -margin);
			var toRht = new Point(margin, 0);
			var toLft = new Point(-margin, 0);

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
			}

			positionPreference ??=
				_ => _.ThenAtMin(c => c.Bounds.Center().Minus(cursor)
					.MultiplyIfNegative(new PointF(1.5f, 1.5f)) // prefer bottom right
					.SquareNorm());

			var candidate = positionPreference(
					candidates.AtMin(c => target.IntersectsWith(c.Bounds)))
				.Find();

			return candidate.Bounds;
		}

		private Size measureTooltip(TooltipModel tooltip, Font titleFont)
		{
			string measuredContentText;
			if (!string.IsNullOrEmpty(tooltip.Title) && !string.IsNullOrEmpty(tooltip.Text))
				measuredContentText = "\n" + tooltip.Text;
			else
				measuredContentText = tooltip.Text;

			const TextFormatFlags formatFlags =
				TextFormatFlags.GlyphOverhangPadding |
				TextFormatFlags.NoPadding |
				TextFormatFlags.TextBoxControl |
				TextFormatFlags.WordBreak;

			var graphics = _tooltipTextbox.CreateGraphics();

			var baseSize = new Size(400, 300).ByDpi();

			Size titleSize;
			if (string.IsNullOrEmpty(tooltip.Title))
				titleSize = new Size(0, 0);
			else
			{
				titleSize = graphics.MeasureText(
					tooltip.Title,
					titleFont,
					baseSize,
					formatFlags);
			}

			Size contentSize;
			if (string.IsNullOrEmpty(measuredContentText))
				contentSize = new Size(0, 0);
			else
			{
				contentSize = graphics.MeasureText(
					measuredContentText,
					_tooltipTextbox.Font,
					baseSize,
					formatFlags);
			}

			int contentWidth = Math.Max(titleSize.Width, contentSize.Width);

			// ideographic strings are under-measured
			Size cjkDelta = tooltip.Text.IsCjk()
				? new Size(32, 6).ByDpi()
				: default;

			int clickableWidthDelta = Clickable
				? titleSize.Width + (int) (_buttonClose.Width * 1.25f) - TextPadding.Horizontal / 2 - contentWidth
				: 0;

			var size = new Size(
				contentWidth + TextPadding.Horizontal + 2 + Math.Max(cjkDelta.Width, clickableWidthDelta),
				titleSize.Height + contentSize.Height + TextPadding.Vertical + cjkDelta.Height);

			return size;
		}

		public void HideTooltip()
		{
			if (_tooltipTextbox.Focused || _buttonClose.Focused)
				_tooltip.Control.Focus();

			_tooltipTextbox.SelectionStart = 0;
			_tooltipTextbox.SelectionLength = 0;

			if (Runtime.IsMono)
				Hide();
			else
				Location = _hiddenLocation;

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
			set
			{
				base.BackColor = value;

				if (_panel != null)
					_panel.BackColor = value;

				if (_tooltipTextbox != null)
					_tooltipTextbox.BackColor = value;
			}
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


		private Padding _textPadding;
		public Padding TextPadding
		{
			get => _textPadding;
			set
			{
				_textPadding = value;
				_panel.Padding = value;
			}
		}

		public int TooltipMargin { get; set; } = 12;

		public AnchorStyles VisibleBorders
		{
			get => _panel.VisibleBorders;
			set => _panel.VisibleBorders = value;
		}

		public DashStyle TooltipBorderStyle
		{
			get => _panel.BorderDashStyle;
			set => _panel.BorderDashStyle = value;
		}

		private readonly FixedRichTextBox _tooltipTextbox;
		private readonly Control _tooltipFocusTarget;
		private readonly Button _buttonClose;

		public bool Clickable { get; private set; }

		private bool _userInteracted;
		private TooltipModel _tooltip;
		private readonly BorderedPanel _panel;
		private bool _closeEnabled;
		private Bitmap _closeIcon;
		private Bitmap _selectableTextIcon;
		private readonly Font _titleFont;
		private static readonly Point _hiddenLocation = new Point(-10000, -10000);
	}
}
