using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mtgdb.Dal
{
	public class LocalizationRepository
	{
		private readonly string[] _csvFiles =
		{
			@"gatherer_extractor.csv"
		};

		private readonly Dictionary<string, Dictionary<string, ConcurrentBag<CardLocalizationRaw>>> _cardsBySetByName =
			new Dictionary<string, Dictionary<string, ConcurrentBag<CardLocalizationRaw>>>(Str.Comparer);

		public bool IsFileLoadingComplete { get; private set; }
		public bool IsLoadingComplete { get; private set; }
		public int Count { get; private set; }

		public event Action CardAdded;
		public event Action LoadingComplete;

		private GathererExtractorCsvParser[] _parsers;

		public void LoadFile()
		{
			_parsers = _csvFiles
				.Select(_ => new GathererExtractorCsvParser(File.ReadAllText(AppDir.Data.AddPath(_))))
				.ToArray();
				
			IsFileLoadingComplete = true;
		}

		public void Load()
		{
			foreach (var parser in _parsers)
				for (int i = 0; i < parser.Count; i++)
					addCard(parser.Read(i));

			IsLoadingComplete = true;
			LoadingComplete?.Invoke();

			// освободить память
			_parsers = null;
		}

		/// <summary>
		/// Освободить память
		/// </summary>
		public void Clear()
		{
			lock (_cardsBySetByName)
				_cardsBySetByName.Clear();
		}

		public CardLocalization GetLocalization(string setCode, string name)
		{
			CardLocalizationRaw raw;

			lock (_cardsBySetByName)
			{
				Dictionary<string, ConcurrentBag<CardLocalizationRaw>> cardsByName;
				if (!_cardsBySetByName.TryGetValue(setCode, out cardsByName))
					return null;

				ConcurrentBag<CardLocalizationRaw> variants;

				if (!cardsByName.TryGetValue(name, out variants))
					return null;

				if (!variants.TryPeek(out raw))
				{
					// Из за многопоточности можно попасть в момент между созданием HashSet и последующим добавлением в него первого элемента
					return null;
				}
			}

			var result = new CardLocalization(raw);
			return result;
		}

		private void addCard(CardLocalizationRaw card)
		{
			lock (_cardsBySetByName)
			{
				Dictionary<string, ConcurrentBag<CardLocalizationRaw>> cardsByName;
				if (!_cardsBySetByName.TryGetValue(card.Set, out cardsByName))
				{
					cardsByName = new Dictionary<string, ConcurrentBag<CardLocalizationRaw>>(Str.Comparer);
					_cardsBySetByName.Add(card.Set, cardsByName);
				}

				ConcurrentBag<CardLocalizationRaw> variants;
				if (!cardsByName.TryGetValue(card.Name, out variants))
				{
					variants = new ConcurrentBag<CardLocalizationRaw>();
					cardsByName.Add(card.Name, variants);
				}

				variants.Add(card);
			}

			Count++;
			CardAdded?.Invoke();
		}
	}
}