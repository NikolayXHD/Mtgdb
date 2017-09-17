using System;
using System.IO;
using System.Linq;
using Mtgdb.Dal;
using Newtonsoft.Json;

namespace Mtgdb.Downloader
{
	public class PriceDownloader
	{
		public PriceDownloader(
			CardRepository repo,
			DownloaderPriceRepository priceRepository)
		{
			_repo = repo;
			_client = new PriceClient();
			_priceRepository = priceRepository;
		}

		public void LoadPendingProgress()
		{
			_priceRepository.Load();
		}

		public void ResetPendingProgress()
		{
			_priceRepository.ResetPendingProgress();
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
					foreach (var card in set.Cards)
					{
						if (_aborted)
							return;

						if (!_priceRepository.IsDefined(card) || _priceRepository.ContainsSid(card))
							continue;

						var sid = _client.DownloadSid(set.MagicCardsInfoCode, card.MciNumber);
						_priceRepository.AddSid(card, sid);

						SidAdded?.Invoke();

						var serialized = JsonConvert.SerializeObject(_priceRepository.GetPriceId(card), Formatting.None);
						writer.WriteLine(serialized);
					}
		}

		private void downloadPrices()
		{
			var sets = _repo.SetsByCode.Values.OrderByDescending(_ => _.ReleaseDate).ToArray();

			using (var stream = _priceRepository.AppendPriceInProgressStream())
			using (var writer = new StreamWriter(stream))
				foreach (var set in sets)
					foreach (var card in set.Cards)
					{
						if (_aborted)
							return;

						if (!_priceRepository.IsDefined(card))
							continue;

						var sid = _priceRepository.GetPriceId(card);

						if (_priceRepository.ContainsPrice(sid))
							continue;

						var price = _client.DownloadPrice(sid);

						_priceRepository.AddPrice(sid, price);

						PriceAdded?.Invoke();

						var serialized = JsonConvert.SerializeObject(_priceRepository.GetPrice(sid), Formatting.None);
						writer.WriteLine(serialized);
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
		private readonly DownloaderPriceRepository _priceRepository;
		private readonly PriceClient _client;
	}
}