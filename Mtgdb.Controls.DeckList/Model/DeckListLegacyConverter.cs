using System.IO;
using System.Linq;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckListLegacyConverter
	{
		public DeckListLegacyConverter(DeckListModel model, DeckLegacyConverter deckConverter)
		{
			_model = model;
			_deckConverter = deckConverter;
			IsConversionRequired = File.Exists(_fileName) && !File.Exists(_model.FileName);
		}

		public bool IsConversionRequired { get; }
		public bool IsConversionCompleted { get; private set; }

		public void Convert()
		{
			string legacyFileContent = File.ReadAllText(_fileName);
			var deserialized = _model.Deserialize(legacyFileContent);

			deserialized.Decks = deserialized.Decks
				.Select(_deckConverter.Convert)
				.ToList();

			deserialized.Collection = deserialized.Collection?.Invoke0(convertCollection);

			var serialized = _model.Serialize(deserialized);
			File.WriteAllText(_model.FileName, serialized);

			IsConversionCompleted = true;
		}

		private CollectionSnapshot convertCollection(CollectionSnapshot collection)
		{
			var deck = Deck.Create(collection.CountById, collection.CountById.Keys.ToList(), null, null);
			var converted = _deckConverter.Convert(deck);

			return new CollectionSnapshot
			{
				CountById = converted.MainDeck.Count
			};
		}

		private static readonly string _fileName = AppDir.History.AddPath("decks.json");
		private readonly DeckListModel _model;
		private readonly DeckLegacyConverter _deckConverter;
	}
}