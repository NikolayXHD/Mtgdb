using System.IO;
using System.Linq;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckListLegacyConverter
	{
		public DeckListLegacyConverter(DeckListModel model, DeckConverter deckConverter)
		{
			_model = model;
			_deckConverter = deckConverter;

			IsLegacyConversionRequired = File.Exists(_legacyFileName) &&
				!File.Exists(_v2FileName) &&
				!File.Exists(_model.FileName);

			IsV2ConversionRequired = File.Exists(_v2FileName) &&
				!File.Exists(_model.FileName);
		}

		public void ConvertLegacyList()
		{
			string legacyFileContent = File.ReadAllText(_legacyFileName);
			var deserialized = _model.Deserialize(legacyFileContent);

			deserialized.Decks = deserialized.Decks
				.Select(_deckConverter.ConvertLegacyDeck)
				.ToList();

			deserialized.Collection = deserialized.Collection?.Invoke0(convertLegacyCollection);

			var serialized = _model.Serialize(deserialized);
			File.WriteAllText(_model.FileName, serialized);

			IsLegacyConversionCompleted = true;
		}

		private CollectionSnapshot convertLegacyCollection(CollectionSnapshot collection)
		{
			var deck = Deck.Create(collection.CountById, collection.CountById.Keys.ToList(), null, null);
			var converted = _deckConverter.ConvertLegacyDeck(deck);

			return new CollectionSnapshot
			{
				CountById = converted.MainDeck.Count
			};
		}

		public void ConvertV2List()
		{
			string v2FileContent = File.ReadAllText(_v2FileName);
			var deserialized = _model.Deserialize(v2FileContent);

			deserialized.Decks = deserialized.Decks
				.Select(_deckConverter.ConvertV2Deck)
				.ToList();

			deserialized.Collection = deserialized.Collection?.Invoke0(convertV2Collection);

			var serialized = _model.Serialize(deserialized);
			File.WriteAllText(_model.FileName, serialized);

			IsLegacyConversionCompleted = true;
		}

		private CollectionSnapshot convertV2Collection(CollectionSnapshot collection)
		{
			var deck = Deck.Create(collection.CountById, collection.CountById.Keys.ToList(), null, null);
			var converted = _deckConverter.ConvertV2Deck(deck);

			return new CollectionSnapshot
			{
				CountById = converted.MainDeck.Count
			};
		}

		public bool IsLegacyConversionRequired { get; }
		public bool IsLegacyConversionCompleted { get; private set; }

		public bool IsV2ConversionRequired { get; }

		private static readonly string _legacyFileName = AppDir.History.AddPath("decks.json");
		private static readonly string _v2FileName = AppDir.History.AddPath("decks.v2.json");
		private readonly DeckListModel _model;
		private readonly DeckConverter _deckConverter;
	}
}