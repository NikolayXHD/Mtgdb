using System.IO;

namespace DrawingEx.IconEncoder
{
	/// <summary>
	/// simplereader can be used as binaryreader, which NOT closes the stream
	/// </summary>
	public class SimpleReader:BinaryReader
	{
		/// <summary>
		/// ctor
		/// </summary>
		public SimpleReader(Stream stream)
			:base(stream){}
		/// <summary>
		/// removes the reader form the stream WITHOUT closing it
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose (false);
		}
	}
	/// <summary>
	/// simplewriter can be used as binarywriter, which NOT closes the stream
	/// </summary>
	public class SimpleWriter:BinaryWriter
	{
		/// <summary>
		/// ctor
		/// </summary>
		public SimpleWriter(Stream stream):
			base(stream){}
		/// <summary>
		/// removes the reader form the stream WITHOUT closing it
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose (false);
		}
	}
}
