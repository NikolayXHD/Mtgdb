using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public class CardFormatter
	{
		public string[] CustomLayout
		{
			get => _customLayout;
			set
			{
				if (_customLayout == value)
					return;

				if (value != null)
				{
					var missingProps = value.ToHashSet();
					missingProps.ExceptWith(_properties.Keys);
					if (missingProps.Count > 0)
						throw new ArgumentException($"Fields {string.Join(", ", missingProps)} are not supported");
				}

				_customLayout = value;
			}
		}

		public string ToString(Card card)
		{
			return string.Join(
				" / ",
				(CustomLayout ?? _layout)
					.Select(_ => _properties[_](card))
					.Where(F.IsNotNull)
			);
		}

		private readonly Dictionary<string, Func<Card, object>> _properties =
			new Dictionary<string, Func<Card, object>>
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

		private string[] _customLayout;
		private static readonly string[] _layout = {nameof(Card.SetCode), nameof(Card.Number), nameof(Card.NameEn)};
	}
}
