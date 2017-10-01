using System.Runtime.Serialization;

namespace Mtgdb.Gui
{
	[DataContract(Name = "NormalCardSize")]
	public class SmallConfig
	{
		[DataMember(Name = "Width")]
		public int? Width { get; set; }

		[DataMember(Name = "Height")]
		public int? Height { get; set; }
	}
}