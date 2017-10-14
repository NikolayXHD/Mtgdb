using System.Collections.Generic;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	internal static class QuickFilterSetup
	{
		public static QuickFilterControl[] GetQuickFilterControls(FormMain form)
		{
			return new[]
			{
				form.FilterManaCost,
				form.FilterAbility,
				form.FilterRarity,
				form.FilterType,
				form.FilterCmc,
				form.FilterManaAbility,
				form.FilterGeneratedMana
			};
		}

		public static IList<FilterValueState>[] GetButtonStates(FormMain form)
		{
			return new IList<FilterValueState>[]
			{
				form.FilterManaCost.States,
				form.FilterType.States,
				form.FilterRarity.States,
				form.FilterAbility.States,
				form.FilterCmc.States,
				form.FilterManaAbility.States,
				form.FilterGeneratedMana.States
			};
		}

		public static void SetQuickFilterProperties(FormMain form)
		{
			form.FilterManaCost.Properties = KeywordDefinitions.ManaCost;
			form.FilterAbility.Properties = KeywordDefinitions.Ability;
			form.FilterRarity.Properties = KeywordDefinitions.Rarity;
			form.FilterType.Properties = KeywordDefinitions.Type;
			form.FilterCmc.Properties = KeywordDefinitions.Cmc;
			form.FilterManaAbility.Properties = KeywordDefinitions.ManaAbility;
			form.FilterGeneratedMana.Properties = KeywordDefinitions.ManaGenerated;

			form.FilterManager.Properties = new[]
			{
				@"Buttons",
				@"Search",
				@"Legality",
				@"Collection",
				@"Deck"
			};

			form.FilterManager.StatesDefault = new[]
			{
				FilterValueState.Required,
				FilterValueState.Required,
				FilterValueState.Required,
				FilterValueState.Ignored,
				FilterValueState.Ignored
			};

			var costNeutralValues = new HashSet<string>
			{
				null,
				@"{X}"
			};

			form.FilterManaCost.CostNeutralValues = costNeutralValues;
			form.FilterManaAbility.CostNeutralValues = costNeutralValues;
		}
	}
}