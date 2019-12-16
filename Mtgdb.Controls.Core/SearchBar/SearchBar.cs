using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	// TODO suggest list content depends on history of caret movement
	// Reproduce:
	// Artist: "Aaron Boyd"
	// move caret around quoted name
	// Result:
	// suggest content on the same position depends on direction it was moved from

	public class SearchBar : DropDownBase
	{
		public SearchBar()
		{
			SuspendLayout();

			Padding = DefaultPadding;
			HighlightMouseOverOpacity = 0;
			HighlightCheckedOpacity = 0;
			TextImageRelation = TextImageRelation.ImageBeforeText;
			ImagePosition = StringAlignment.Near;

			Input = new FixedRichTextBox
			{
				Anchor = ControlHelpers.AnchorAll,
				AutoWordSelection = false,
				Multiline = false,
				BackColor = BackColor,
				ForeColor = ForeColor,
				BorderStyle = System.Windows.Forms.BorderStyle.None,
				Margin = new Padding(3)
			};

			Controls.Add(Input);

			ResumeLayout(false);
			PerformLayout();

			GotFocus += gotFocus;
			Input.GotFocus += inputFocusedChanged;
			Input.LostFocus += inputFocusedChanged;
			Layout += layout;
		}

		private void layout(object sender, LayoutEventArgs e)
		{
			int imageWidth = SelectImage()?.Width ?? 0;

			Input.SetBounds(
				Input.Margin.Left + Padding.Left + imageWidth,
				Input.Margin.Top,
				Width - Input.Margin.Horizontal - Padding.Left - imageWidth,
				Height - Input.Margin.Vertical,
				BoundsSpecified.All);
		}

		protected override void HandlePopupItemKeyDown(KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Right:
				case Keys.Left:
					return;
			}

			base.HandlePopupItemKeyDown(e);
		}

		protected override void HandlePopupOwnerPressed()
		{
		}

		protected override ButtonBase CreateMenuItem(string value, int index)
		{
			var result = base.CreateMenuItem(value, index);
			result.HighlightCheckedOpacity = 64;
			result.HighlightMouseOverOpacity = 32;
			return result;
		}

		protected override void PaintDropDownIndication(Graphics g)
		{
		}


		private void gotFocus(object sender, EventArgs e) =>
			Input.Focus();

		private void inputFocusedChanged(object sender, EventArgs e) =>
			Invalidate();


		protected override void Dispose(bool disposing)
		{
			GotFocus -= gotFocus;
			Input.GotFocus -= inputFocusedChanged;
			Input.LostFocus -= inputFocusedChanged;
			Layout -= layout;

			base.Dispose(disposing);
		}

		protected override Padding DefaultPadding => new Padding(0);

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TextImageRelation TextImageRelation
		{
			get => base.TextImageRelation;
			set => base.TextImageRelation = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override StringAlignment TextPosition
		{
			get => base.TextPosition;
			set => base.TextPosition = value;
		}

		[DefaultValue(typeof(StringAlignment), "Near")]
		public override StringAlignment ImagePosition
		{
			get => base.ImagePosition;
			set => base.ImagePosition = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoSize
		{
			get => base.AutoSize;
			set => base.AutoSize = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoCheck
		{
			get => base.AutoCheck;
			set => base.AutoCheck = value;
		}

		[DefaultValue(0)]
		public override int HighlightCheckedOpacity
		{
			get => base.HighlightCheckedOpacity;
			set => base.HighlightCheckedOpacity = value;
		}

		[DefaultValue(0)]
		public override int HighlightMouseOverOpacity
		{
			get => base.HighlightMouseOverOpacity;
			set => base.HighlightMouseOverOpacity = value;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FixedRichTextBox Input { get; }
	}
}
