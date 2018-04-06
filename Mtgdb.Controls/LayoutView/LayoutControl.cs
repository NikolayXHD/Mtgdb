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

		public virtual void CopyTo(LayoutControl other)
		{
			other.Font = Font;
			other.Size = Size;
			other.HighlightOptions = HighlightOptions.Clone();

			using (var probeEnumerator = Fields.GetEnumerator())
			using (var enumerator = other.Fields.GetEnumerator())
			{
				while (probeEnumerator.MoveNext())
				{
					enumerator.MoveNext();

					var probeField = probeEnumerator.Current;
					var field = enumerator.Current;

					field.Location = probeField.Location;
					field.Size = probeField.Size;
					field.Font = probeField.Font;
					field.BackColor = probeField.BackColor;
					field.ForeColor = probeField.ForeColor;
					field.HorizontalAlignment = probeField.HorizontalAlignment;
					field.IconRecognizer = probeField.IconRecognizer;
					field.SearchOptions = probeField.SearchOptions.Clone();
				}
			}
		}



		protected void SubscribeToFieldEvents()
		{
			foreach (var field in Fields)
				field.Invalid += fieldInvalidated;
		}

		protected virtual void LoadData(object dataSource)
		{
		}

		public virtual IEnumerable<ButtonLayout> GetFieldButtons(FieldControl field, SearchOptions searchOptions, SortOptions sortOptions)
		{
			// the order is reversed because alignment is top-right

			if (field.IsSortVisible)
				yield return sortOptions.GetButtonLayout(field);

			if (field.IsSearchVisible)
				yield return searchOptions.GetButtonLayout(field);
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
