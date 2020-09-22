using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtgdb.Data
{
	internal class CardPatch
	{
		public string Name { get; [UsedImplicitly] set; }
		public string Text { get; [UsedImplicitly] set; }
		public string Flavor { get; [UsedImplicitly] set; }
		public string Set { get; [UsedImplicitly] set; }
		public List<string> Sets { get; [UsedImplicitly] set; }
		public List<string> GeneratedMana { get; [UsedImplicitly] set; }
		public bool FlipDuplicate { get; [UsedImplicitly] set; }
		public bool FullDuplicate { get; [UsedImplicitly] set; }
		public bool Remove { get; [UsedImplicitly] set; }
		public string Loyalty { get; [UsedImplicitly] set; }
		public string Type { get; [UsedImplicitly] set; }
		public List<string> Types { get; [UsedImplicitly] set; }
		public List<string> Subtypes { get; [UsedImplicitly] set; }
		public string OriginalText { get; [UsedImplicitly] set; }
		public string OriginalType { get; [UsedImplicitly] set; }
		public string Layout { get; [UsedImplicitly] set; }
		public string Number { get;  [UsedImplicitly] set; }

		public int? Hand { get; [UsedImplicitly] set; }
		public int? Life { get; [UsedImplicitly] set; }

		public bool? HasNoSide { get; [UsedImplicitly] set; }
	}
}
