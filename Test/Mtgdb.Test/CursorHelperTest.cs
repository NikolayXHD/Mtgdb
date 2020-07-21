using System.Drawing;
using Mtgdb.Controls;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class CursorHelperTest
	{
		[Test]
		public void Cursor_is_created()
		{
			var bmp = new Bitmap(16, 16);
			var hotSpot = new Point(8, 0);

			var cursor = CursorHelper.CreateCursor(bmp, hotSpot);
			Assert.That(cursor.Size, Is.EqualTo(bmp.Size));
		}
	}
}
