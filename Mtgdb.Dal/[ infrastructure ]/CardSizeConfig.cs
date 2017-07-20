using System.Runtime.Serialization;

namespace Mtgdb.Gui
{
	[DataContract(Name = "CardSize")]
	public class CardSizeConfig
	{
		[DataMember(Name = "Width")]
		public int? Width { get; set; }

		[DataMember(Name = "Height")]
		public int? Height { get; set; }
	}
}