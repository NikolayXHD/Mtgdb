using System.Drawing;

namespace Mtgdb.Controls
{
	public interface ILayoutElement
	{
		Size Size { get; }
		Size Margin { get; }
		ContentAlignment Alignment { get; }
		Point Location { get; set; }
	}
}