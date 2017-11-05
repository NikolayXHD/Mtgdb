namespace Mtgdb.Dal
{
	public delegate void DeckChangedEventHandler(
		bool listChanged,
		bool countChanged,
		Card card,
		bool touchedChanged,
		Zone? changedZone);
}