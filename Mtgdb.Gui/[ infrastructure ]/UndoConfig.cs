using System.Runtime.Serialization;

namespace Mtgdb.Gui
{
	[DataContract(Name = "Undo")]
	public class UndoConfig
	{
		[DataMember(Name = "MaxDepth")]
		public int? MaxDepth { get; set; }
	}
}
