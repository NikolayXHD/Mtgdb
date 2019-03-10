using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	[Localizable(false)]
	public static class Aggregates
	{
		public static readonly Dictionary<string, Func<HashSet<Card>, IField<Card>, object>> ByName = new Dictionary
			<string, Func<HashSet<Card>, IField<Card>, object>>
		{
			{ Count, (cards, field) => field.Count(cards) },
			{ CountDistinct, (cards, field) => field.CountDistinct(cards) },
			{ Min, (cards, field) => field.Min(cards) },
			{ Max, (cards, field) => field.Max(cards) },
			{ Average, (cards, field) => field.Average(cards) },
			{ Sum, (cards, field) => field.Sum(cards) }
		};

		public static readonly Dictionary<string, string> Alias = new Dictionary<string, string>
		{
			{ Count, "count" },
			{ CountDistinct, "count distinct" },
			{ Min, "minimum" },
			{ Max, "maximum" },
			{ Average, "average" },
			{ Sum, "sum" }
		};

		public const string Count = nameof(IField<Card>.Count);
		public const string CountDistinct = nameof(IField<Card>.CountDistinct);
		public const string Min = nameof(IField<Card>.Min);
		public const string Max = nameof(IField<Card>.Max);
		public const string Average = nameof(IField<Card>.Average);
		public const string Sum = nameof(IField<Card>.Sum);
	}
}