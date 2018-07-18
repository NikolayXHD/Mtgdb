using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mtgdb.Dal;
using Newtonsoft.Json;

namespace Mtgdb.Controls
{
	public class DeckListModel
	{
		[UsedImplicitly]
		public DeckListModel()
		{
		}

		public bool Add(Deck deck)
		{
			var duplicate = findDupliate(deck);
			if (duplicate != null)
			{
				duplicate.Saved = deck.Saved;
				return false;
			}

			_decks.Add(deck);
			addDeckByName(deck);
			return true;
		}

		public void Remove(Deck deck)
		{
			_decks.Remove(deck);
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
			var serialized = JsonConvert.SerializeObject(_decks, Formatting.Indented);
			File.WriteAllText(_fileName, serialized);
		}

		public void Load()
		{
			if (File.Exists(_fileName))
			{
				var serialized = File.ReadAllText(_fileName);
				_decks = JsonConvert.DeserializeObject<List<Deck>>(serialized);

				_decksByName = _decks
					.GroupBy(_ => _.Name, Str.Comparer)
					.ToDictionary(_ => _.Key, _ => _.ToList(), Str.Comparer);
			}

			IsLoaded = true;
			Loaded?.Invoke();
		}

		public bool IsLoaded { get; private set; }
		public event Action Loaded;

		public IEnumerable<DeckModel> GetModels(UiModel ui) =>
			_decks.Select((d, i) => new DeckModel(d, ui)
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

		private List<Deck> _decks = new List<Deck>();

		[JsonIgnore]
		private Dictionary<string, List<Deck>> _decksByName =
			new Dictionary<string, List<Deck>>(Str.Comparer);

		private static readonly string _fileName = AppDir.History.AddPath("decks.json");
	}
}