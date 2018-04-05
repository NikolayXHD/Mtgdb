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
			_fieldImage.SearchOptions.Allow = false;

			SubscribeToFieldEvents();
		}

		protected override void LoadData(object dataSource)
		{
			var card = (Card) dataSource;
			_fieldImage.Image = card?.Image(Ui);
		}
	}
}
