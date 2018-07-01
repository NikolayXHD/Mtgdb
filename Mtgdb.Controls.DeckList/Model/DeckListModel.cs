using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mtgdb.Dal;
using Newtonsoft.Json;

namespace Mtgdb.Controls
{
	public class DeckListModel
	{
		public void Add(Deck deck)
		{
			var duplicate = findDupliate(deck);
			if (duplicate != null)
				duplicate.Saved = deck.Saved;
			else
			{
				Decks.Add(deck);
				addDeckByName(deck);
			}
		}

		public void Remove(Deck deck)
		{
			Decks.Remove(deck);
			removeDeckByName(deck);
		}

		public void Rename(Deck deck, string name)
		{
			removeDeckByName(deck);
			deck.Name = name;

			var duplicate = findDupliate(deck);
			if (duplicate != null)
				duplicate.Saved = deck.Saved;
			else
				addDeckByName(deck);
		}

		public void Save()
		{
			var serialized = JsonConvert.SerializeObject(Decks, Formatting.Indented);
			File.WriteAllText(_fileName, serialized);
		}

		public void Load()
		{
			if (!File.Exists(_fileName))
			{
				Decks = new List<Deck>();
				_decksByName = new Dictionary<string, List<Deck>>(Str.Comparer);
			}
			else
			{
				var serialized = File.ReadAllText(_fileName);
				Decks = JsonConvert.DeserializeObject<List<Deck>>(serialized);

				_decksByName = Decks
					.GroupBy(_ => _.Name, Str.Comparer)
					.ToDictionary(_ => _.Key, _ => _.ToList());
			}
		}

		public IEnumerable<DeckModel> GetModels(UiModel ui) =>
			Decks.Select((d, i) => new DeckModel(d, ui)
			{
				Id = i
			});



		private void removeDeckByName(Deck deck)
		{
			var decks = _decksByName[deck.Name];
			decks.Remove(deck);
		}

		private void addDeckByName(Deck deck)
		{
			_decksByName.TryGetValue(deck.Name, out var decks);
			if (decks == null)
			{
				decks = new List<Deck>();
				_decksByName.Add(deck.Name, decks);
			}

			decks.Add(deck);
		}

		private Deck findDupliate(Deck deck)
		{
			if (!_decksByName.TryGetValue(deck.Name, out var decks))
				return null;

			var duplicate = decks.FirstOrDefault(_ => _.IsEquivalentTo(deck));
			return duplicate;
		}

		public List<Deck> Decks { get; set; }

		[JsonIgnore]
		private Dictionary<string, List<Deck>> _decksByName;

		private static readonly string _fileName = AppDir.History.AddPath("decks.json");
	}
}