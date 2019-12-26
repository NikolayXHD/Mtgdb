using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Mtgdb.Ui;
using Newtonsoft.Json;

namespace Mtgdb.Data.Model
{
	public class DeckListModel
	{
		public event Action Changed;

		[UsedImplicitly]
		public DeckListModel(
			CardRepository repo,
			CollectedCardsDeckTransformation transformation,
			CollectionEditorModel collection,
			IApplication app)
		{
			_app = app;
			_repo = repo;
			_transformation = transformation;
			_collectionEditor = collection;
			_state.Collection = _collectionEditor.Snapshot();

			_serializer = new JsonSerializer();

			_serializer.Converters.Add(
				new UnformattedJsonConverter(type =>
					typeof(IEnumerable<int>).IsAssignableFrom(type)));
		}

		public void SubscribeToEvents()
		{
			_collectionEditor.CollectionChanged += collectionChanged;
		}

		private void collectionChanged(bool listChanged, bool countChanged, Card card)
		{
			if (!_repo.IsLoadingComplete.Signaled)
				throw new InvalidOperationException();

			if (!listChanged && !countChanged)
				return;

			_app.CancellationToken.Run(token =>
			{
				_abort = true;
				lock (_syncCollection)
				{
					_abort = false;

					var snapshot = _collectionEditor.Snapshot();

					var affectedCardIds = snapshot.GetAffectedCardIds(_state.Collection);
					if (affectedCardIds.Count == 0)
						return;

					var affectedNames = affectedCardIds
						.Select(id => _repo.CardsById[id].NameEn)
						.ToHashSet(Str.Comparer);

					lock (_syncModels)
						foreach (var model in _deckModels)
						{
							if (_abort)
								return;

							model.UpdateCollection(snapshot, affectedNames);
						}

					_state.Collection = snapshot;
					Save();
				}
			});
		}

		public bool Add(Deck deck)
		{
			var duplicate = findDuplicate(deck);
			if (duplicate != null)
			{
				duplicate.Saved = deck.Saved;
				return false;
			}

			var model = CreateModel(deck);
			var index = _deckModels.Count;

			lock (_syncModels)
			{
				deck.Id = Interlocked.Increment(ref _state.Id);
				_deckModels.Add(model);
				_indexByDeck.Add(model, index);
				_decksByName.Add(deck.Name, model);
			}

			// Save not called here to optimize mass-import
			return true;
		}

		public DeckModel CreateModel(Deck deck) =>
			new DeckModel(deck, _repo, _state.Collection, _transformation);

		public void Remove(DeckModel deck)
		{
			lock (_syncModels)
			{
				_deckModels.RemoveAt(_indexByDeck[deck]);
				_indexByDeck = Enumerable.Range(0, _deckModels.Count)
					.ToDictionary(i => _deckModels[i]);
				_decksByName.Remove(deck.Name, deck);
			}
		}

		public void Rename(DeckModel deck, string name)
		{
			lock (_syncModels)
			{
				_decksByName.Remove(deck.Name, deck);

				deck.Name = name;
				var duplicate = findDuplicate(deck.OriginalDeck);
				if (duplicate != null)
					duplicate.Saved = deck.Saved;
				else
					_decksByName.Add(deck.Name, deck);
			}
		}

		public IReadOnlyList<DeckModel> GetModelCopies()
		{
			lock (_syncModels)
				return _state.Decks.Select(CreateModel).ToList();
		}

		public IEnumerable<DeckModel> GetModels()
		{
			lock (_syncModels)
				foreach (var model in _deckModels)
					yield return model;
		}


		public void Save()
		{
			Changed?.Invoke();

			string serialized;
			lock (_syncModels)
			{
				_state.Decks = _deckModels.Select(_ => _.OriginalDeck).ToList();
				serialized = serialize(_state);
			}

			File.WriteAllText(FileName, serialized);
		}

		private string serialize(State state)
		{
			var result = new StringBuilder();
			using (var writer = new StringWriter(result))
			using (var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' })
				_serializer.Serialize(jsonWriter, state);

			return result.ToString();
		}

		public void Load()
		{
			State state;
			if (File.Exists(FileName))
			{
				string serialized = File.ReadAllText(FileName);
				state = deserialize(serialized);
			}
			else
				state = new State();

			lock (_syncModels)
			{
				_state = state;

				_deckModels = _state.Decks
					.Select(d => new DeckModel(d, _repo, _state.Collection, _transformation))
					.ToList();

				_decksByName = _deckModels.ToMultiDictionary(_ => _.Name, Str.Comparer);

				_indexByDeck = Enumerable.Range(0, _deckModels.Count)
					.ToDictionary(i => _deckModels[i]);
			}

			IsLoaded = true;
			Loaded?.Invoke();
		}

		public void FillCardNames()
		{
			if (_repo.IsLoadingComplete.Signaled)
				throw new InvalidOperationException();

			lock (_syncModels)
				foreach (var model in _deckModels)
				{
					model.FillCardNames();
					model.ClearCaches();
				}
		}

		private State deserialize(string serialized)
		{
			try
			{
				return JsonConvert.DeserializeObject<State>(serialized);
			}
			catch (JsonException)
			{
				var decks = JsonConvert.DeserializeObject<List<Deck>>(serialized);

				return new State
				{
					Collection = _state.Collection,
					Decks = decks,
					IdCounter = decks.Max(_=>_.Id)
				};
			}
		}

		public bool IsLoaded { get; private set; }
		public event Action Loaded;

		public void TransformDecks(Func<bool> interrupt)
		{
			if (!IsLoaded)
				return;

			lock (_syncModels)
			{
				var count = _deckModels.Count;

				for (int i = 0; i < count; i++)
				{
					if (interrupt())
						return;

					_deckModels[i].UpdateTransformedDeck();
				}
			}
		}

		private DeckModel findDuplicate(Deck deck)
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
		private readonly CollectedCardsDeckTransformation _transformation;
		private readonly CollectionEditorModel _collectionEditor;

		public string FileName { get; set; } = AppDir.History.AddPath("decks.v4.json");
		private State _state = new State();

		private readonly object _syncCollection = new object();
		private readonly object _syncModels = new object();
		private bool _abort;

		public class State
		{
			[JsonIgnore]
			public long Id;

			public long IdCounter
			{
				[UsedImplicitly] // by serializer
				get => Id;
				set => Id = value;
			}
			public CollectionSnapshot Collection { get; set; } = CollectionSnapshot.Empty();
			public List<Deck> Decks { get; set; } = new List<Deck>();
		}

		private readonly JsonSerializer _serializer;
		private readonly IApplication _app;
	}
}
