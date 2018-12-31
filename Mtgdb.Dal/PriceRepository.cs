using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class PriceRepository
	{
		public PriceRepository()
		{
			var directory = AppDir.Data;

			PriceFile = directory.AddPath("price.json");
			IdFile = directory.AddPath("price.id.json");
		}

		public virtual void Load()
		{
			Load(IdFile, PriceFile);
		}

		protected void Load(string idFile, string priceFile)
		{
			IsLoadingComplete = false;

			if (File.Exists(idFile))
				_sidsByMultiverseId = readJsonLines(idFile)
					.Select(JsonConvert.DeserializeObject<PriceId>)
					.ToDictionary(_ => _.MultiverseId);
			else
				_sidsByMultiverseId = new Dictionary<int, PriceId>();

			if (File.Exists(priceFile))
			{
				_priceBySid = readJsonLines(priceFile)
					.Select(JsonConvert.DeserializeObject<PriceValues>)
					.ToDictionary(_ => _.Sid);
			}
			else
				_priceBySid = new Dictionary<string, PriceValues>();

			SidCount = _sidsByMultiverseId.Count;

			IsLoadingComplete = true;
		}

		public bool IsDefined(Card c) =>
			c.MultiverseId.HasValue;

		public bool ContainsSid(Card c) =>
			c.MultiverseId.HasValue && _sidsByMultiverseId.ContainsKey(c.MultiverseId.Value);

		public bool ContainsPrice(string sid) =>
			_priceBySid.ContainsKey(sid);

		public PriceId GetPriceId(Card c) =>
			c.MultiverseId?.Invoke2(CollectionExtensions.TryGet, _sidsByMultiverseId);

		public void AddSid(Card card, PriceId sid)
		{
			if (!IsDefined(card))
				throw new ArgumentException("card is not defined", nameof(card));

			if (sid.MultiverseId != card.MultiverseId.Value)
				throw new ArgumentException("sid doesn't match the card");

			_sidsByMultiverseId.Add(card.MultiverseId.Value, sid);

			SidCount++;
		}

		public void AddPrice(string sid, PriceValues priceValues) =>
			_priceBySid.Add(sid, priceValues);

		public PriceValues GetPrice(Card c)
		{
			var priceId = GetPriceId(c);

			if (priceId?.Sid == null)
				return null;

			return _priceBySid.TryGet(priceId.Sid);
		}

		public PriceValues GetPrice(PriceId priceId)
		{
			if (priceId.Sid == null)
				return null;

			return _priceBySid[priceId.Sid];
		}

		private static IEnumerable<string> readJsonLines(string file)
		{
			var content = File.ReadAllText(file);

			if (!content.EndsWith(Str.Endl))
			{
				var index = content.LastIndexOf(Str.Endl, StringComparison.Ordinal);
				if (index < 0)
					content = string.Empty;
				else
					content = content.Substring(0, index + Str.Endl.Length);

				File.WriteAllText(file, content);
			}

			var lines = content.Split(Array.From(Str.Endl), StringSplitOptions.RemoveEmptyEntries);
			return lines;
		}

		public int SidCount { get; private set; }

		public int PricesCount => _priceBySid.Count;

		private Dictionary<int, PriceId> _sidsByMultiverseId;
		private Dictionary<string, PriceValues> _priceBySid;

		protected readonly string PriceFile;
		protected readonly string IdFile;

		public bool IsLoadingComplete { get; private set; }
	}
}