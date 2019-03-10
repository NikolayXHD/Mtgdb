using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mtgdb.Controls;
using Mtgdb.Data;

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

			_fieldImage.AllowSort =
				_fieldText.AllowSort =
					_fieldFlavor.AllowSort =
						_fieldRulings.AllowSort = false;

			_fieldRulings.SearchOptions.Allow = false;

			_fieldImage.SearchOptions.Button.ShowOnlyWhenHotTracked = false;
			_fieldImage.SearchOptions.Button.Icon = Properties.Resources.search_like_hovered_32;
			_fieldImage.SearchOptions.Button.Margin = new Size(4, 4);

			BackColor = SystemColors.Window;

			foreach (var field in Fields)
				field.ForeColor = SystemColors.WindowText;

			_fieldRulings.ForeColor = SystemColors.GrayText;

			DeckEditorButtons.SetupButtons(_fieldImage);

			SubscribeToFieldEvents();
		}

		protected override void LoadData(object dataSource)
		{
			var card = (Card) dataSource;

			_fieldImage.Image = card?.Image(Ui());
			_fieldImage.DataText = card?.NameEn;

			_fieldName.DataText = card?.Name(Ui());
			_fieldManaCost.DataText = card?.ManaCost;
			_fieldType.DataText = card?.Type(Ui());
			_fieldCmc.DataText = card?.Cmc.ToString(Str.Culture);
			_fieldSetCode.DataText = card?.SetCode;
			_fieldSetName.DataText = card?.SetName;
			_fieldText.DataText = card?.Text(Ui());
			_fieldFlavor.DataText = card?.Flavor(Ui());
			_fieldArtist.DataText = card?.Artist;
			_fieldReleaseDate.DataText = card?.ReleaseDate;
			_fieldRarity.DataText = card?.Rarity;
			_fieldPricingLow.DataText = card?.PricingLow?.ToString(@"$0.##");
			_fieldPricingMid.DataText = card?.PricingMid?.ToString(@"$0.##");
			_fieldPricingHigh.DataText = card?.PricingHigh?.ToString(@"$0.##");
			_fieldLoyalty.DataText = card?.Loyalty;
			_fieldPower.DataText = card?.Power;
			_fieldToughness.DataText = card?.Toughness;
			_fieldRulings.DataText = card?.Rulings;
		}

		public override IEnumerable<FieldControl> Fields => _layout.Controls.Cast<FieldControl>();
	}
}