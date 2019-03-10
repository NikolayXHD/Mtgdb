using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public interface IPostPaintEvent
	{
		event PaintEventHandler PostPaint;
	}
}