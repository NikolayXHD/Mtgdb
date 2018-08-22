using JetBrains.Annotations;

namespace Mtgdb.Gui
{
	/// <summary>
	/// Helps identifying identical strings as different objects
	/// </summary>
	internal class TabField
	{
		[UsedImplicitly]  // to find usages in IDE
		public TabField()
		{
		}

		public string FieldName { get; set; }
	}
}