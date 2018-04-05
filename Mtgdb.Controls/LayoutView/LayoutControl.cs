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

		protected void SubscribeToFieldEvents()
		{
			foreach (var field in Fields)
				field.Invalid += fieldInvalidated;
		}

		private void fieldInvalidated(FieldControl field)
		{
			var rectangle = field.Bounds;
			rectangle.Offset(field.Location);
			Invalid?.Invoke(this, field);
		}

		public void SetIconRecognizer(IconRecognizer value)
		{
			foreach (var field in Fields)
				field.IconRecognizer = value;
		}

		[Category("Settings")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public HighlightSettings HighlightSettings { get; set; } = new HighlightSettings();

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataSource
		{
			get { return _dataSource; }
			set
			{
				_dataSource = value;
				LoadData(_dataSource);
			}
		}

		protected virtual void LoadData(object dataSource)
		{
		}

		public virtual IEnumerable<FieldControl> Fields => Controls.Cast<FieldControl>();
		private object _dataSource;

		public virtual void CopyTo(LayoutControl other)
		{
			other.Font = Font;
			other.Size = Size;

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
					field.CustomSearchIcon = probeField.CustomSearchIcon;
					field.ShowSearchOnlyWhenHotTracked = probeField.ShowSearchOnlyWhenHotTracked;
				}
			}
		}
	}
}
