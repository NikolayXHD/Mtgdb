using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mtgdb.Dal;
using Newtonsoft.Json;

namespace Mtgdb.Controls
{
	public class DeckListModel
	{
		public void Insert(int index, Deck deck)
		{
			Decks.Insert(index, deck);
			save();
		}

		public void Remove(Deck deck)
		{
			Decks.Remove(deck);
			save();
		}

		public void Rename(Deck deck, string name)
		{
			deck.Name = name;
			save();
		}



		private void save()
		{
			var serialized = JsonConvert.SerializeObject(Decks, Formatting.Indented);
			File.WriteAllText(_fileName, serialized);
		}

		public void Load()
		{
			if (!File.Exists(_fileName))
				Decks = new List<Deck>();
			else
			{
				var serialized = File.ReadAllText(_fileName);
				Decks = JsonConvert.DeserializeObject<List<Deck>>(serialized);
			}
		}

		private static readonly string _fileName = AppDir.History.AddPath("decks.json");

		public List<Deck> Decks { get; set; }

		public IEnumerable<DeckModel> GetModels(UiModel ui) =>
			Decks.Select(d => new DeckModel(d, ui));
	}
}