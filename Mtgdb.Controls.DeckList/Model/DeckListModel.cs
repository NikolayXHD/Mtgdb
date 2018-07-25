using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Mtgdb.Dal;
using Mtgdb.Ui;
using Newtonsoft.Json;

namespace Mtgdb.Controls
{
	public class DeckListModel
	{
		[UsedImplicitly]
		public DeckListModel(
			CardRepository repo,
			IDeckTransformation transformation,
			CollectionEditorModel collection)
		{
			_repo = repo;
			_transformation = transformation;
			_collectionEditor = collection;

			_collectionEditor.CollectionChanged += collectionChanged;
			_state.Collection = new CollectionSnapshot(_collectionEditor);
		}

		private void collectionChanged(bool listChanged, bool countChanged, Card card)
		{
			var snapshot = new CollectionSnapshot(_collectionEditor);

			if (_repo.IsLoadingComplete)
			{
				var affectedCardIds = snapshot.GetAffectedCardIds(_state.Collection);
				var affectedNames = affectedCardIds
					.Select(id => _repo.CardsById[id].NameEn)
					.ToHashSet(Str.Comparer);

				foreach (var model in _deckModels)
				{
					if (model.MayContainCardNames(affectedNames))
						model.Collection = snapshot;
				}
			}

			_state.Collection = snapshot;
		}

		public bool Add(Deck deck)
		{
			var duplicate = findDupliate(deck);
			if (duplicate != null)
			{
				duplicate.Saved = deck.Saved;
				return false;
			}

			var model = CreateModel(deck);
			var index = _deckModels.Count;
			deck.Id = Interlocked.Increment(ref _state.Id);

			_deckModels.Add(model);
			_indexByDeck.Add(model, index);
			_decksByName.Add(deck.Name, model);

			return true;
		}

		public DeckModel CreateModel(Deck deck) =>
			new DeckModel(deck, _repo, _state.Collection, _transformation);

		public void Remove(DeckModel deck)
		{
			_deckModels.RemoveAt(_indexByDeck[deck]);
			_indexByDeck.Remove(deck);
			_decksByName.Remove(deck.Name, deck);
		}

		public void Rename(DeckModel deck, string name)
		{
			_decksByName.Remove(deck.Name, deck);

			deck.Name = name;
			var duplicate = findDupliate(deck.OriginalDeck);
			if (duplicate != null)
				duplicate.Saved = deck.Saved;
			else
				_decksByName.Add(deck.Name, deck);
		}



		public void Save()
		{
			_state.Decks = _deckModels.Select(_ => _.Deck).ToList();
			var serialized = JsonConvert.SerializeObject(_state, Formatting.Indented);
			File.WriteAllText(_fileName, serialized);
		}

		public void Load()
		{
			if (File.Exists(_fileName))
			{
				var serialized = File.ReadAllText(_fileName);
				_state = JsonConvert.DeserializeObject<State>(serialized);

				_deckModels = _state.Decks
					.Select(d => new DeckModel(d, _repo, _collectionEditor, _transformation))
					.ToList();

				_decksByName = _deckModels.ToMultiDictionary(_ => _.Name, Str.Comparer);

				_indexByDeck = Enumerable.Range(0, _deckModels.Count)
					.ToDictionary(i => _deckModels[i]);
			}

			IsLoaded = true;
			Loaded?.Invoke();
		}

		public bool IsLoaded { get; private set; }
		public event Action Loaded;

		private DeckModel findDupliate(Deck deck)
		{
			if (!_decksByName.TryGetValues(deck.Name, out var decks))
				return null;

			var duplicate = decks.FirstOrDefault(_ => _.IsEquivalentTo(deck));
			return duplicate;
		}

		private List<DeckModel> _deckModels = new List<DeckModel>();

		private MultiDictionary<string, DeckModel> _decksByName =
			new MultiDictionary<string, DeckModel>(Str.Comparer);

		private Dictionary<DeckModel, int> _indexByDeck =
			new Dictionary<DeckModel, int>();

		private readonly CardRepository _repo;
		private readonly IDeckTransformation _transformation;
		private readonly CollectionEditorModel _collectionEditor;

		private static readonly string _fileName = AppDir.History.AddPath("decks.json");
		private State _state = new State();

		private class State
		{
			[JsonIgnore]
			public long Id;

			[UsedImplicitly] // when deserializing
			public long IdCounter
			{
				get => Id;
				set => Id = value;
			}
			public CollectionSnapshot Collection { get; set; }
			public List<Deck> Decks { get; set; } = new List<Deck>();
		}
	}
}