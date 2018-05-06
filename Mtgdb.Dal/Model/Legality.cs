using System.Collections.Generic;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Dal
{
	public static class Legality
	{
		public const string Legal = "legal";
		public const string Restricted = "restricted";
		public const string Banned = "banned";
		public const string Illegal = "illegal";

		public const string AnyFormat = "[ any format ]";

		public static readonly IReadOnlyList<string> Formats = new[]
		{
			"Standard",
			"Modern",
			"Commander",
			"Legacy",
			"Vintage",
			"Un-Sets",
			"Amonkhet Block",
			"Battle for Zendikar Block",
			"Dominaria Block",
			"Ice Age Block",
			"Innistrad Block",
			"Ixalan Block",
			"Invasion Block",
			"Kaladesh Block",
			"Kamigawa Block",
			"Khans of Tarkir Block",
			"Lorwyn-Shadowmoor Block",
			"Masques Block",
			"Mirage Block",
			"Mirrodin Block",
			"Odyssey Block",
			"Onslaught Block",
			"Ravnica Block",
			"Return to Ravnica Block",
			"Scars of Mirrodin Block",
			"Shadows over Innistrad Block",
			"Shards of Alara Block",
			"Tempest Block",
			"Theros Block",
			"Time Spiral Block",
			"Urza Block",
			"Zendikar Block"
		}.AsReadOnlyList();
	}
}