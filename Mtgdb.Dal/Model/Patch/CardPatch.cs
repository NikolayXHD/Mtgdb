using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtgdb.Dal
{
	internal class CardPatch
	{
		public string Name { get; [UsedImplicitly] set; }
		public string Text { get; [UsedImplicitly] set; }
		public List<string> GeneratedMana { get; [UsedImplicitly] set; }
		public bool FlipDuplicate { get; [UsedImplicitly] set; }
		public bool FullDuplicate { get; [UsedImplicitly] set; }
		public string Loyalty { get; [UsedImplicitly] set; }
		public string Type { get; [UsedImplicitly] set; }
		public List<string> Types { get; [UsedImplicitly] set; }
		public List<string> Subtypes { get; [UsedImplicitly] set; }
		public string OriginalType { get; [UsedImplicitly] set; }
		public string Layout { get; [UsedImplicitly] set; }
		public string[] Names { get; [UsedImplicitly] set; }
		public string Number { get;  [UsedImplicitly] set; }

		public int? Hand;
		public int? Life;
	}
}