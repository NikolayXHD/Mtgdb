using System;
using System.Drawing;
using Mtgdb.Controls;

namespace Mtgdb
{
	public class BitmapTransformationChain : TransformationChain<Bitmap>
	{
		public BitmapTransformationChain(Bitmap original, Action<Exception> logger) : base(original, bmp => (Bitmap) bmp.Clone()) => 
			TransformationException += logger;
	}
}