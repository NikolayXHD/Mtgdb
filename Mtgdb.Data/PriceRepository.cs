using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Mtgdb.Data
{
	public class PriceRepository
	{
		public PriceRepository(Func<IDataDownloader> downloaderFactory)
		{
			_downloaderFactory = downloaderFactory;
			PricesFile = AppDir.Data.Join("AllPrices.json");
			PriceCacheFile = AppDir.Data.Join("AllPrices.cache.json");
		}

		public async Task DownloadFile(CancellationToken token)
		{
			if (PricesFile.IsValid() && !PricesFile.IsFile() && !CacheExists())
				await Downloader.DownloadPrices(token);

			IsDownloadPriceComplete.Signal();
		}

		public void LoadFile(bool ignoreCache = false)
		{
			if (!ignoreCache && CacheExists())
			{
				_priceCacheContent = loadCacheFile();
			}
			else
			{
				_priceContent = PricesFile.IsFile()
					? PricesFile.ReadAllBytes()
					: null;
			}
		}

		public void Load(bool ignoreCache = false)
		{
			if (!ignoreCache && _priceCacheContent != null)
				_priceCache = deserializeCache(_priceCacheContent);
			else
				_prices = deserializePrices();

			_priceContent = null;
			_priceCacheContent = null;
		}

		public void FillPrice(CardRepository repo)
		{
			if (!repo.IsLoadingComplete.Signaled)
				throw new InvalidOperationException("Cards must be loaded before filling price");

			if (_priceCache != null)
			{
				foreach (var set in repo.SetsByCode.Values)
				foreach (var card in set.Cards)
					card.Price = _priceCache.TryGetValue(card.Id, out float price)
						? (float?)price
						: null;
			}
			else if (_prices != null)
			{
				foreach (var set in repo.SetsByCode.Values)
				foreach (var card in set.Cards)
					card.Price = _prices.TryGetValue(card.MtgjsonId, out var mtgjsonPrices)
						? mtgjsonPrices.Paper
							?.SelectMany(entry =>
								(IEnumerable<KeyValuePair<string, float?>>)entry.Value?.Retail?.Normal ??
								Enumerable.Empty<KeyValuePair<string, float?>>())
							.AtMin(entry => entry.Key)
							.FindOrDefault().Value
						: null;
			}

			IsLoadingPriceComplete.Signal();
		}

		public void SaveCache(CardRepository repo)
		{
			if (_priceCache == null)
			{
				var cache = createCache(repo);
				saveCache(cache);
			}
		}

		public void Clear()
		{
			_prices = null; // free memory
			_priceCache = null;
		}

		public bool CacheExists()
		{
			lock (_syncPriceCacheFile)
				return PriceCacheFile.IsFile();
		}

		public void DeleteCache()
		{
			try
			{
				lock (_syncPriceCacheFile)
					PricesFile.DeleteFile();
			}
			catch (Exception ex)
			{
				_log.Error(ex);
			}
		}

		private byte[] loadCacheFile()
		{
			lock (_syncPriceCacheFile)
			{
				if (PriceCacheFile.IsFile())
					return PriceCacheFile.ReadAllBytes();

				return null;
			}
		}

		private void saveCache(Dictionary<string, float> cache)
		{
			var cacheContent = serializeCache(cache);
			PriceCacheFile.WriteAllBytes(cacheContent);
		}

		private Dictionary<string, float> createCache(CardRepository repo)
		{
			var priceByCard = repo.Cards
				.Where(_ => _.Price.HasValue) // ReSharper disable once PossibleInvalidOperationException
				.ToDictionary(_ => _.Id, _ => _.Price.Value);
			return priceByCard;
		}

		private byte[] serializeCache(Dictionary<string, float> priceByCardId)
		{
			using var stream = new MemoryStream();

			using (var streamWriter = new StreamWriter(stream))
			using (var jsonTextWriter = new JsonTextWriter(streamWriter))
				new JsonSerializer().Serialize(jsonTextWriter, priceByCardId);

			return stream.ToArray();
		}

		private Dictionary<string, float> deserializeCache(byte[] priceCacheContent)
		{
			using var stream = new MemoryStream(priceCacheContent);
			using var streamReader = new StreamReader(stream);
			using var jsonTextReader = new JsonTextReader(streamReader);
			return new JsonSerializer().Deserialize<Dictionary<string, float>>(jsonTextReader);
		}

		private Dictionary<string, MtgjsonPrices> deserializePrices()
		{
			if (_priceContent == null)
				return null;

			var serializer = new JsonSerializer();
			using Stream stream = new MemoryStream(_priceContent);
			using var stringReader = new StreamReader(stream);
			using var jsonReader = new JsonTextReader(stringReader);
			jsonReader.Read(); // {

			while (true)
			{
				jsonReader.Read(); //   "data":
				var rootField = (string)jsonReader.Value;
				if (Str.Equals(rootField, "data"))
					break;

				//   "meta":
				jsonReader.Read(); //   {
				serializer.Deserialize<JObject>(jsonReader);
			}

			jsonReader.Read(); //   {

			var result = serializer.Deserialize<Dictionary<string, MtgjsonPrices>>(jsonReader);
			return result;
		}

		public IReadOnlyDictionary<string, MtgjsonPrices> Prices => _prices;

		private FsPath PricesFile { get; set; }
		private FsPath PriceCacheFile { get; set; }


		private readonly Func<IDataDownloader> _downloaderFactory;
		private IDataDownloader _downloader;
		private IDataDownloader Downloader => _downloader ??= _downloaderFactory();

		public AsyncSignal IsDownloadPriceComplete { get; } = new AsyncSignal();
		public AsyncSignal IsLoadingPriceComplete { get; } = new AsyncSignal();

		private byte[] _priceContent;
		private byte[] _priceCacheContent;
		private Dictionary<string, MtgjsonPrices> _prices;
		private Dictionary<string, float> _priceCache;

		private static readonly object _syncPriceCacheFile = new object();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
