using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public interface IDeckTransformation
	{
		Deck Transform(Deck original, ICardCollection collection);
	}
}