using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mtgdb.Data;
using Newtonsoft.Json;

namespace Mtgdb.Downloader
{
	public class PriceDownloader
	{
		public PriceDownloader(
			CardRepository repo,
			PriceDownloaderRepository priceRepository)
		{
			_repo = repo;
			_client = new PriceClient();
			_priceRepository = priceRepository;
		}

		public void LoadPendingProgress()
		{
			_priceRepository.Load();
		}

		public void Download()
		{
			if (!_repo.IsLoadingComplete)
				throw new InvalidOperationException("Card repository is not loaded");

			if (!_priceRepository.IsLoadingComplete)
				throw new InvalidOperationException("Price repository is not loaded");

			if (_downloading)
				throw new InvalidOperationException("Another price downloading is in progress");

			_aborted = false;
			_downloading = true;

			downloadIds();
			downloadPrices();

			if (!_aborted)
			{
				_priceRepository.CommitProgress();
				PricesDownloaded?.Invoke();
			}

			_downloading = false;
		}

		public void Abort()
		{
			_aborted = true;
		}

		private void downloadIds()
		{
			var sets = _repo.SetsByCode.Values.OrderByDescending(_ => _.ReleaseDate).ToArray();

			using (var stream = _priceRepository.AppendPriceIdStream())
			using (var writer = new StreamWriter(stream))
				foreach (var set in sets)
					Parallel.ForEach(set.Cards, new ParallelOptions { MaxDegreeOfParallelism = Parallelism }, card =>
					{
						lock (_sync)
						{
							if (_aborted)
								return;

							if (!_priceRepository.IsDefined(card) || _priceRepository.ContainsSid(card))
								return;
						}

						var sid = _client.DownloadSid(card);
						if (sid == null)
							return;

						var serialized = JsonConvert.SerializeObject(sid, Formatting.None);

						lock (_sync)
						{
							_priceRepository.AddSid(card, sid);
							SidAdded?.Invoke();
							writer.WriteLine(serialized);
						}
					});
		}

		private void downloadPrices()
		{
			var sets = _repo.SetsByCode.Values.OrderByDescending(_ => _.ReleaseDate).ToArray();

			using (var stream = _priceRepository.AppendPriceInProgressStream())
			using (var writer = new StreamWriter(stream))
				foreach (var set in sets)
				{
					var sids = set.Cards
						.Where(_priceRepository.IsDefined)
						.Select(_priceRepository.GetPriceId)
						.Where(F.IsNotNull)
						.Select(_ => _.Sid)
						.Where(F.IsNotNull)
						.Where(sid => !_priceRepository.ContainsPrice(sid))
						.Distinct()
						.ToArray();

					Parallel.ForEach(sids, new ParallelOptions { MaxDegreeOfParallelism = Parallelism }, sid =>
					{
						if (sid.EndsWith("&utm_source=scryfall"))
						{
							PriceAdded?.Invoke();
							return;
						}

						PriceValues price;
						try
						{
							price = _client.DownloadPrice(sid);
						}
						catch (WebException)
						{
							return;
						}

						lock (_sync)
						{
							var serialized = JsonConvert.SerializeObject(price, Formatting.None);
							_priceRepository.AddPrice(sid, price);
							writer.WriteLine(serialized);
							PriceAdded?.Invoke();
						}
					});
				}
		}

		public event Action PriceAdded;
		public event Action SidAdded;
		public event Action PricesDownloaded;

		public int SidCount => _priceRepository.SidCount;
		public int PricesCount => _priceRepository.PricesCount;

		public int DefinedCardsCount
		{
			get
			{
				if (_definedCardsCount.HasValue)
					return _definedCardsCount.Value;

				if (!_repo.IsLoadingComplete)
					return 0;

				_definedCardsCount = _repo.Cards.Count(_priceRepository.IsDefined);
				return _definedCardsCount.Value;
			}
		}

		private int? _definedCardsCount;

		private bool _aborted;
		private bool _downloading;

		private readonly CardRepository _repo;
		private readonly PriceDownloaderRepository _priceRepository;
		private readonly PriceClient _client;

		private const int Parallelism = 1;

		private readonly object _sync = new object();
	}
}