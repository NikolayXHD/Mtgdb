using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	// TODO suggest list content depends on history of caret movement
	// Reproduce:
	// Artist: "Aarron Boyd"
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

			Input = new FixedRichTextBox
			{
				Anchor = ControlHelpers.AnchorAll,
				Multiline = false,
				BackColor = BackColor,
				ForeColor = ForeColor,
				BorderStyle = System.Windows.Forms.BorderStyle.None
			};

			Controls.Add(Input);

			ResumeLayout(false);
			PerformLayout();

			GotFocus += gotFocus;
			Layout += layout;

			_searchTextSelection = new RichTextBoxSelectionSubsystem(Input);
			_searchTextSelection.SubscribeToEvents();
		}

		private void layout(object sender, LayoutEventArgs e)
		{
			Input.SetBounds(
				Padding.Left,
				Padding.Top,
				Width - Padding.Horizontal,
				Height - Padding.Vertical,
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

		protected override void OnPopupOwnerPressed()
		{
		}

		protected override ButtonBase CreateMenuItem(string value, int index)
		{
			var result = base.CreateMenuItem(value, index);
			result.HighlightCheckedOpacity = 64;
			result.HighlightMouseOverOpacity = 32;
			return result;
		}

		private void gotFocus(object sender, EventArgs e) =>
			Input.Focus();



		protected override void Dispose(bool disposing)
		{
			GotFocus -= gotFocus;
			Layout -= layout;
			_searchTextSelection.UnsubscribeFromEvents();
			base.Dispose(disposing);
		}

		protected override Padding DefaultPadding => new Padding(3);

		public override bool Focused =>
			base.Focused || Input.Focused;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TextImageRelation TextImageRelation
		{
			get => base.TextImageRelation;
			set => base.TextImageRelation = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override StringAlignment TextAlign
		{
			get => base.TextAlign;
			set => base.TextAlign = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Bitmap Image
		{
			get => base.Image;
			set => base.Image = value;
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
		public override int HighlightMouseOverOpacity
		{
			get => base.HighlightMouseOverOpacity;
			set => base.HighlightMouseOverOpacity = value;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FixedRichTextBox Input { get; }

		private readonly RichTextBoxSelectionSubsystem _searchTextSelection;
	}
}