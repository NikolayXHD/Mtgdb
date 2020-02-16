using System.Collections.Generic;

namespace Mtgdb.Data
{
	public static class CardTypes
	{
		public const string Creature = "Creature";
		public const string Instant = "Instant";
		public const string Sorcery = "Sorcery";
		public const string Planeswalker = "Planeswalker";
		public const string Enchantment = "Enchantment";
		public const string Artifact = "Artifact";
		public const string Land = "Land";
		public const string Token = "Token";
		public const string Card = "Card";
		public const string Emblem = "Emblem";



		public static bool IsCreature(this Card c) =>
			c.TypesArr.Contains(Creature);

		public static bool IsInstant(this Card c) =>
			c.TypesArr.Contains(Instant);

		public static bool IsSorcery(this Card c) =>
			c.TypesArr.Contains(Sorcery);

		public static bool IsPlaneswalker(this Card c) =>
			c.TypesArr.Contains(Planeswalker);

		public static bool IsEnchantment(this Card c) =>
			c.TypesArr.Contains(Enchantment);

		public static bool IsArtifact(this Card c) =>
			c.TypesArr.Contains(Artifact);

		public static bool IsLand(this Card c) =>
			c.TypesArr.Contains(Land);
	}
}
