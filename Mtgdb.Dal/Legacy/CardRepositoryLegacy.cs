using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class CardRepositoryLegacy
	{
		public CardRepositoryLegacy()
		{
			SetsFile = AppDir.Data.AddPath("AllSets-x.json");
			PatchFile = AppDir.Data.AddPath("patch.json");

			Cards = new List<CardLegacy>();
		}

		public void LoadFile()
		{
			if (File.Exists(SetsFile))
				_streamContent = File.ReadAllBytes(SetsFile);
			else
				_streamContent = Encoding.UTF8.GetBytes("{}");

			if (File.Exists(PatchFile))
				Patch = JsonConvert.DeserializeObject<Patch>(File.ReadAllText(PatchFile));
			else
				Patch = new Patch
				{
					ImageOrder = new Dictionary<string, Dictionary<string, ImageNamePatch>>(),
					Legality = new Dictionary<string, LegalityPatch>(),
					Cards = new Dictionary<string, CardPatch>()
				};

			Patch.IgnoreCase();
		}

		public void Load()
		{
			var serializer = new JsonSerializer();

			Stream stream = new MemoryStream(_streamContent);
			using (stream)
			using (var stringReader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(stringReader))
			{
				jsonReader.Read();

				while (true)
				{
					jsonReader.Read();

					if (jsonReader.TokenType == JsonToken.EndObject)
						// sets are over, all json was read
						break;

					var setCode = (string) jsonReader.Value;

					// skip set name
					jsonReader.Read();

					if (!FilterSetCode(setCode))
					{
						jsonReader.Skip();
						continue;
					}

					var set = serializer.Deserialize<SetLegacy>(jsonReader);

					for (int i = 0; i < set.Cards.Count; i++)
					{
						var card = set.Cards[i];
						card.Set = set;

						preProcessCard(card);
					}

					for (int i = set.Cards.Count - 1; i >= 0; i--)
						if (set.Cards[i].Remove)
							set.Cards.RemoveAt(i);

					// after preProcessCard, to have NameNormalized field set non empty
					set.CardsByName = set.Cards.GroupBy(_ => _.NameNormalized)
						.ToDictionary(
							gr => gr.Key,
							gr => gr.ToList(),
							Str.Comparer);

					lock (SetsByCode)
						SetsByCode.Add(set.Code, set);

					lock (Cards)
						foreach (var card in set.Cards)
							Cards.Add(card);

					foreach (var card in set.Cards)
						CardsById[card.Id] = card;
				}
			}

			// release RAM
			_streamContent = null;
			Patch = null;
			Cards.Capacity = Cards.Count;
		}

		private void preProcessCard(CardLegacy card)
		{
			if (Patch.Cards.TryGetValue(card.SetCode, out var patch))
				card.PatchCard(patch);

			if (Patch.Cards.TryGetValue(card.NameEn, out patch))
				card.PatchCard(patch);

			if (Patch.Cards.TryGetValue(card.Id, out patch))
				card.PatchCard(patch);

			card.NameNormalized = string.Intern(card.NameEn.RemoveDiacritics());

			card.TextEn = card.TextEn?.Invoke1(LocalizationRepository.IncompleteChaosPattern.Replace, "{CHAOS}");

			if (!string.IsNullOrEmpty(card.OriginalText) && Str.Equals(card.OriginalText, card.TextEn))
				card.OriginalText = null;

			if (Str.Equals(card.SetCode, "BBD"))
			{
				if (card.Number.EndsWith("b", Str.Comparison))
					card.Remove = true;
				else if (card.Number.EndsWith("a", Str.Comparison))
				{
					card.Number = card.Number.Substring(0, card.Number.Length - 1);
					card.Names = null;
				}
			}
		}

		public Func<string, bool> FilterSetCode { get; set; } = _ => true;
		internal Patch Patch { get; private set; }

		public List<CardLegacy> Cards { get; }
		public IDictionary<string, CardLegacy> CardsById { get; } = new Dictionary<string, CardLegacy>(Str.Comparer);
		public IDictionary<string, SetLegacy> SetsByCode { get; } = new Dictionary<string, SetLegacy>(Str.Comparer);

		private string SetsFile { get; }
		private string PatchFile { get; }

		private byte[] _streamContent;
	}
}