using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Gui
{
	public partial class TestLayoutForm : Form
	{
		public TestLayoutForm()
		{
			InitializeComponent();

			_layout.SizeChanged += sizeChanged;
		}

		private void sizeChanged(object sender, EventArgs e)
		{
			var cell = _layout.GetCellPosition(_verticalFlow);
			var preferredSize = _verticalFlow.GetPreferredSize(new Size(int.MaxValue, _verticalFlow.Height));
			int width = preferredSize.Width;

			_layout.ColumnStyles[cell.Column].Width = width + _verticalFlow.Margin.Right + _verticalFlow.Margin.Left;
		}
	}
}
