using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

		public async Task DownloadPriceFile(CancellationToken token)
		{
			if (PricesFile.IsValid() && !PricesFile.IsFile() && !PriceCacheExists())
				await Downloader.DownloadPrices(token);

			IsDownloadPriceComplete.Signal();
		}

		public void LoadPriceFile(bool ignoreCache = false)
		{
			if (!ignoreCache && PriceCacheExists())
			{
				_priceCacheContent = loadPriceCacheFile();
			}
			else
			{
				_priceContent = PricesFile.IsFile()
					? PricesFile.ReadAllBytes()
					: null;
			}
		}

		public void LoadPrice(bool ignoreCache = false)
		{
			if (!ignoreCache && _priceCacheContent != null)
				_priceCache = deserializePriceCache(_priceCacheContent);
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
				var cache = createPriceCache(repo);
				savePriceCache(cache);
			}
		}

		public void Clear()
		{
			_prices = null; // free memory
			_priceCache = null;
		}

		public bool PriceCacheExists()
		{
			lock (_syncPriceCacheFile)
				return PriceCacheFile.IsFile();
		}

		public void DeletePriceCache()
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

		private byte[] loadPriceCacheFile()
		{
			lock (_syncPriceCacheFile)
			{
				if (PriceCacheFile.IsFile())
					return PriceCacheFile.ReadAllBytes();

				return null;
			}
		}

		private void savePriceCache(Dictionary<string, float> cache)
		{
			var cacheContent = serializePriceCache(cache);
			PriceCacheFile.WriteAllBytes(cacheContent);
		}

		private Dictionary<string, float> createPriceCache(CardRepository repo)
		{
			var priceByCard = repo.Cards
				.Where(_ => _.Price.HasValue) // ReSharper disable once PossibleInvalidOperationException
				.ToDictionary(_ => _.Id, _ => _.Price.Value);
			return priceByCard;
		}

		private byte[] serializePriceCache(Dictionary<string, float> priceByCardId)
		{
			using var stream = new MemoryStream();

			using (var streamWriter = new StreamWriter(stream))
			using (var jsonTextWriter = new JsonTextWriter(streamWriter))
				new JsonSerializer().Serialize(jsonTextWriter, priceByCardId);

			return stream.ToArray();
		}

		private Dictionary<string, float> deserializePriceCache(byte[] priceCacheContent)
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

			using Stream stream = new MemoryStream(_priceContent);
			using var stringReader = new StreamReader(stream);
			using var jsonReader = new JsonTextReader(stringReader);
			jsonReader.Read(); // {
			jsonReader.Read(); //   "data":
			jsonReader.Read(); //   {
			var result = new JsonSerializer().Deserialize<Dictionary<string, MtgjsonPrices>>(jsonReader);
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
