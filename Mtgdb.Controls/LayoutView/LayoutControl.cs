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

		public LayoutControl()
		{
			ControlAdded += controlAdded;
		}

		private void controlAdded(object sender, ControlEventArgs e)
		{
			var field = e.Control as FieldControl;
			if (field != null)
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

		public IEnumerable<FieldControl> Fields => Controls.Cast<FieldControl>();
		private object _dataSource;
	}
}
