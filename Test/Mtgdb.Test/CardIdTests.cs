using System.IO;
using System.Linq;
using System.Text;
using Mtgdb.Dal;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class CardIdTests : TestsBase
	{
		[Test]
		public void Id_is_unique()
		{
			LoadCards();

			var groups = Repo.Cards.GroupBy(c => c.Id)
				.Select(gr => (Id: gr.Key, Cards: gr.ToList()))
				.OrderByDescending(gr => gr.Cards.Count)
				.ToList();

			Assert.That(groups[0].Cards.Count == 1);
		}

		[Test]
		public void Build_mtgjson_v42_id_to_custom_id_map()
		{
			var repo = new CardRepository
			{
				SetsFile = AppDir.Data.AddPath("AllSets.v42.json")
			};

			repo.LoadFile();

			var result = new StringBuilder();
			result.AppendLine("mtgjsonid\tscryfallId\tname");
			foreach (var set in repo.DeserializeSets())
				foreach (var card in set.Cards)
					result.AppendLine($"{card.MtgjsonId}\t{card.ScryfallId}\t{card.NameEn}");

			File.WriteAllText("d:\\temp\\mtgjson-42-id.csv", result.ToString());
		}

		[TestCase("DisplayWindowHeight", ExpectedResult = 2017415020ul)]
		[TestCase("DisplayWindowWidth", ExpectedResult = 3332824435ul)]
		[TestCase("DisplayWindowVertical", ExpectedResult = 2262072301ul)]
		public ulong Hash(string val)
		{
			uint hash = 5381;

			foreach (char c in val)
				hash = ((hash << 5) + hash) + c;

			return hash;
		}
	}
}