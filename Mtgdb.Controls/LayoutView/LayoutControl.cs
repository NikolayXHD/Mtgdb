using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class LayoutControl : UserControl
	{
		public event Action<LayoutControl, FieldControl> Invalid;

		public void SetIconRecognizer(IconRecognizer value)
		{
			foreach (var field in Fields)
				field.IconRecognizer = value;
		}

		public virtual void CopyFrom(LayoutControl other)
		{
			Font = other.Font;
			Size = other.Size;
			HighlightOptions = other.HighlightOptions.Clone();

			using (var thisEnumerator = Fields.GetEnumerator())
			using (var otherEnumerator = other.Fields.GetEnumerator())
				while (thisEnumerator.MoveNext() && otherEnumerator.MoveNext())
					thisEnumerator.Current.CopyFrom(otherEnumerator.Current);
		}



		protected void SubscribeToFieldEvents()
		{
			foreach (var field in Fields)
				field.Invalid += fieldInvalidated;
		}

		protected virtual void LoadData(object dataSource)
		{
		}

		public IEnumerable<ButtonLayout> GetFieldButtons(FieldControl field, SearchOptions searchOptions, SortOptions sortOptions)
		{
			// usually the visual order will be reversed because default alignment is top-right

			if (field.IsSortVisible)
				yield return sortOptions.GetButtonLayout(field);

			if (field.IsSearchVisible)
				yield return searchOptions.GetButtonLayout(field);

			for (int i = 0; i < field.CustomButtons.Count; i++)
			{
				var button = field.CustomButtons[i];
				
				if (!field.IsHotTracked && (button.ShowOnlyWhenHotTracked ?? true))
					continue;

				var icon = field.HotTrackedCustomButtonIndex == i
					? button.Icon
					: button.IconTransp;

					yield return new ButtonLayout(icon, button.Margin, button.Alignment, button.BreaksLayout);
			}
		}

		private void fieldInvalidated(FieldControl field)
		{
			var rectangle = field.Bounds;
			rectangle.Offset(field.Location);
			Invalid?.Invoke(this, field);
		}



		public virtual IEnumerable<FieldControl> Fields => Controls.Cast<FieldControl>();

		[Category("Settings")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public HighlightOptions HighlightOptions { get; set; } = new HighlightOptions();

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataSource
		{
			get => _dataSource;
			set
			{
				_dataSource = value;
				LoadData(_dataSource);
			}
		}

		private object _dataSource;
	}
}
