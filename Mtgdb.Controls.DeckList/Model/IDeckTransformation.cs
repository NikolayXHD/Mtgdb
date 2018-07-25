using Mtgdb.Dal;

namespace Mtgdb.Controls
{
	public interface IDeckTransformation
	{
		Deck Transform(Deck original, ICardCollection collection);
	}
}