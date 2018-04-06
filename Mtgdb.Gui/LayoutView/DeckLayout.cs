using System.Drawing;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public partial class DeckLayout : CardLayoutControlBase
	{
		public DeckLayout()
		{
			InitializeComponent();

			_fieldImage.FieldName = nameof(Card.Image);

			_fieldImage.AllowSort = false;

			_fieldImage.SearchOptions.Button.ShowOnlyWhenHotTracked = false;
			_fieldImage.SearchOptions.Button.Icon = Properties.Resources.search_like_hovered_32;
			_fieldImage.SearchOptions.Button.Margin = new Size(4, 4);

			SubscribeToFieldEvents();
		}

		protected override void LoadData(object dataSource)
		{
			var card = (Card) dataSource;

			_fieldImage.Image = card?.Image(Ui);
			_fieldImage.Text = card?.NameEn;
		}
	}
}
