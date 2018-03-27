using System.Drawing;
using Mtgdb.Controls;
using NLog;

namespace Mtgdb.Dal
{
	public class BitmapTransformationChain : TransformationChain<Bitmap>
	{
		public BitmapTransformationChain(Bitmap original) : base(original, bmp => (Bitmap) bmp.Clone()) =>
			TransformationException += _log.Error;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}