using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public partial class DeckLayout : LayoutControl
	{
		public DeckLayout()
		{
			InitializeComponent();

			_fieldImage.FieldName = nameof(Card.Image);
			_fieldImage.AllowSort = false;
		}

		protected override void LoadData(object dataSource)
		{
			var card = (Card) dataSource;
			_fieldImage.Image = card?.Image;
		}
	}
}
