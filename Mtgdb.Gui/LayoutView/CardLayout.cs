using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public partial class CardLayout : CardLayoutControlBase
	{
		public CardLayout()
		{
			InitializeComponent();

			_fieldImage.FieldName = nameof(Card.Image);
			_fieldName.FieldName = nameof(Card.Name);
			_fieldManaCost.FieldName = nameof(Card.ManaCost);
			_fieldType.FieldName = nameof(Card.Type);
			_fieldCmc.FieldName = nameof(Card.Cmc);
			_fieldSetCode.FieldName = nameof(Card.SetCode);
			_fieldSetName.FieldName = nameof(Card.SetName);
			_fieldText.FieldName = nameof(Card.Text);
			_fieldFlavor.FieldName = nameof(Card.Flavor);
			_fieldArtist.FieldName = nameof(Card.Artist);
			_fieldReleaseDate.FieldName = nameof(Card.ReleaseDate);
			_fieldRarity.FieldName = nameof(Card.Rarity);
			_fieldPricingLow.FieldName = nameof(Card.PricingLow);
			_fieldPricingMid.FieldName = nameof(Card.PricingMid);
			_fieldPricingHigh.FieldName = nameof(Card.PricingHigh);
			_fieldLoyalty.FieldName = nameof(Card.Loyalty);
			_fieldPower.FieldName = nameof(Card.Power);
			_fieldToughness.FieldName = nameof(Card.Toughness);
			_fieldRulings.FieldName = nameof(Card.Rulings);

			_fieldImage.AllowSort = false;
			_fieldImage.AllowSearch = false;

			_fieldText.AllowSort = false;
			_fieldFlavor.AllowSort = false;

			_fieldRulings.AllowSort = false;
			_fieldRulings.AllowSearch = false;

			HighlightSettings.HighlightBorderColor = Color.CadetBlue;
			HighlightSettings.HighlightColor = Color.LightBlue;
			HighlightSettings.HighlightContextColor = Color.LightCyan;

			SubscribeToFieldEvents();
		}

		protected override void LoadData(object dataSource)
		{
			var card = (Card) dataSource;

			_fieldImage.Image = card?.Image(Ui);
			_fieldName.Text = card?.Name(Ui);
			_fieldManaCost.Text = card?.ManaCost;
			_fieldType.Text = card?.Type(Ui);
			_fieldCmc.Text = card?.Cmc.ToString(Str.Culture);
			_fieldSetCode.Text = card?.SetCode;
			_fieldSetName.Text = card?.SetName;
			_fieldText.Text = card?.Text(Ui);
			_fieldFlavor.Text = card?.Flavor(Ui);
			_fieldArtist.Text = card?.Artist;
			_fieldReleaseDate.Text = card?.ReleaseDate;
			_fieldRarity.Text = card?.Rarity;
			_fieldPricingLow.Text = card?.PricingLow?.ToString(@"$0.##");
			_fieldPricingMid.Text = card?.PricingMid?.ToString(@"$0.##");
			_fieldPricingHigh.Text = card?.PricingHigh?.ToString(@"$0.##");
			_fieldLoyalty.Text = card?.Loyalty;
			_fieldPower.Text = card?.Power;
			_fieldToughness.Text = card?.Toughness;
			_fieldRulings.Text = card?.Rulings;
		}

		public override IEnumerable<FieldControl> Fields => _layout.Controls.Cast<FieldControl>();
	}
}
