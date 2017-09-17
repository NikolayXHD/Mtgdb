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

			PriceFile = Path.Combine(directory, "price.json");
			PriceFileInProgress = Path.Combine(directory, "price.inprogress.json");
			IdFile = Path.Combine(directory, "price.id.json");
		}

		public virtual void Load()
		{
			Load(IdFile, PriceFile);
		}

		protected void Load(string idFile, string priceFile)
		{
			IsLoadingComplete = false;

			if (File.Exists(idFile))
				_sidsBySetByCard = readJsonLines(idFile)
					.Select(JsonConvert.DeserializeObject<PriceId>)
					.GroupBy(_ => _.Set)
					.ToDictionary(
						gr => gr.Key,
						gr => gr.ToDictionary(_ => _.Card));
			else
				_sidsBySetByCard = new Dictionary<string, Dictionary<string, PriceId>>();

			if (File.Exists(priceFile))
			{
				_priceBySid = readJsonLines(priceFile)
					.Select(JsonConvert.DeserializeObject<PriceValues>)
					.ToDictionary(_ => _.Sid);
			}
			else
				_priceBySid = new Dictionary<string, PriceValues>();

			SidCount = _sidsBySetByCard.SelectMany(_ => _.Value.Values).Count();

			IsLoadingComplete = true;
		}

		public bool IsDefined(Card c)
		{
			return !string.IsNullOrEmpty(c.MciNumber) && !string.IsNullOrEmpty(c.Set.MagicCardsInfoCode);
		}

		public bool ContainsSid(Card c)
		{
			Dictionary<string, PriceId> sidsByCard;
			if (!_sidsBySetByCard.TryGetValue(c.Set.MagicCardsInfoCode, out sidsByCard))
				return false;

			return sidsByCard.ContainsKey(c.MciNumber);
		}

		public bool ContainsPrice(PriceId priceId)
		{
			return priceId.Sid == null || _priceBySid.ContainsKey(priceId.Sid);
		}

		public PriceId GetPriceId(Card c)
		{
			if (string.IsNullOrEmpty(c.MciNumber) || string.IsNullOrEmpty(c.Set.MagicCardsInfoCode))
				return null;

			Dictionary<string, PriceId> sidsByCard;
			if (!_sidsBySetByCard.TryGetValue(c.Set.MagicCardsInfoCode, out sidsByCard))
				return null;

			PriceId sid;
			if (!sidsByCard.TryGetValue(c.MciNumber, out sid))
				return null;

			return sid;
		}

		public void AddSid(Card card, PriceId sid)
		{
			if (!IsDefined(card))
				throw new ArgumentException("card is not defined", nameof(card));

			if (sid.Set != card.Set.MagicCardsInfoCode || sid.Card != card.MciNumber)
				throw new ArgumentException("sid doesnt match the card");

			Dictionary<string, PriceId> ids;
			if (!_sidsBySetByCard.TryGetValue(card.Set.MagicCardsInfoCode, out ids))
			{
				ids = new Dictionary<string, PriceId>();
				_sidsBySetByCard.Add(card.Set.MagicCardsInfoCode, ids);
			}
			
			ids.Add(card.MciNumber, sid);

			SidCount++;
		}

		public void AddPrice(PriceId priceId, PriceValues priceValues)
		{
			_priceBySid.Add(priceId.Sid, priceValues);
		}

		public PriceValues GetPrice(Card c)
		{
			var priceId = GetPriceId(c);

			if (priceId?.Sid == null)
				return null;

			return _priceBySid[priceId.Sid];
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

			if (!content.EndsWith(Environment.NewLine))
			{
				var index = content.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
				if (index < 0)
					content = string.Empty;
				else
					content = content.Substring(0, index + Environment.NewLine.Length);

				File.WriteAllText(file, content);
			}

			var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			return lines;
		}

		public int SidCount { get; private set; }

		public int PricesCount => _priceBySid.Count;

		private Dictionary<string, Dictionary<string, PriceId>> _sidsBySetByCard;
		private Dictionary<string, PriceValues> _priceBySid;
		
		protected readonly string PriceFile;
		protected readonly string PriceFileInProgress;
		protected readonly string IdFile;

		public bool IsLoadingComplete { get; private set; }
	}
}