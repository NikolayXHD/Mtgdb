using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public delegate void DeckChangedEventHandler(bool listChanged, bool countChanged, Card card, bool touchedChanged);
}