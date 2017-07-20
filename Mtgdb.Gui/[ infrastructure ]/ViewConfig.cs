using System.Runtime.Serialization;

namespace Mtgdb.Gui
{
	[DataContract(Name = "View")]
	public class ViewConfig
	{
		[DataMember(Name = "ShowTextualFields")]
		public bool? ShowTextualFields { get; set; }

		[DataMember(Name = "ShowDeck")]
		public bool? ShowDeck { get; set; }

		[DataMember(Name = "AllowPartialCards")]
		public bool? AllowPartialCards { get; set; }
	}
}