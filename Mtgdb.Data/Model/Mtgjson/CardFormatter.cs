using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public class CardFormatter
	{
		public Func<Card, string> CustomFormat { get; set; }

		public string[][] CustomLayout
		{
			get => _customLayout;
			set
			{
				if (_customLayout == value)
					return;

				if (value != null)
				{
					var missingProps = value.SelectMany(_ => _).ToHashSet();
					missingProps.ExceptWith(_properties.Keys);
					if (missingProps.Count > 0)
						throw new ArgumentException($"Fields {string.Join(", ", missingProps)} are not supported");
				}

				_customLayout = value;
			}
		}

		public string ToString(Card card)
		{
			if (CustomFormat != null)
				return CustomFormat(card);

			return string.Join(
				" / ",
				(CustomLayout ?? _layout).Select(
					line => string.Concat(
						line
							.Select(_ => _properties[_](card))
							.Where(F.IsNotNull)
							.Select(_ => $"[ {_} ]")))
				.Where(formatted => !string.IsNullOrEmpty(formatted)));
		}

		private readonly Dictionary<string, Func<Card, object>> _properties =
			new Dictionary<string, Func<Card, object>>()
			{
				[nameof(Card.NameEn)] = _ => _.NameEn,
				[nameof(Card.TypeEn)] = _ => _.TypeEn,
				[nameof(Card.TextEn)] = _ => _.TextEn?.Replace("\r\n", "¶ ").Replace("\n", "¶ "),
				[nameof(Card.FlavorEn)] = _ => _.FlavorEn,
				[nameof(Card.ManaCost)] = _ => _.ManaCost,
				[nameof(Card.Layout)] = _ => _.Layout,
				[nameof(Card.Loyalty)] = _ => _.Loyalty,
				[nameof(Card.Power)] = _ => _.Power,
				[nameof(Card.Toughness)] = _ => _.Toughness,
				[nameof(Card.Rulings)] = _ => _.Rulings,
				[nameof(Card.SetCode)] = _ => _.SetCode,
				[nameof(Card.Number)] = _ => _.Number,
				[nameof(Card.MultiverseId)] = _ => _.MultiverseId,
				[nameof(Card.Artist)] = _ => _.Artist,
				[nameof(Card.ImageName)] = _ => _.ImageName
			};

		private string[][] _customLayout;

		private static readonly string[][] _layout =
		{
			new [] { nameof(Card.NameEn), nameof(Card.ManaCost), nameof(Card.SetCode), nameof(Card.Number) },
			new [] { nameof(Card.TypeEn), nameof(Card.Layout) },
			new [] { nameof(Card.Loyalty), nameof(Card.Power), nameof(Card.Toughness) },
			new [] { nameof(Card.TextEn) },
			new [] { nameof(Card.FlavorEn) },
			new [] { nameof(Card.Artist) },
		};
	}
}
