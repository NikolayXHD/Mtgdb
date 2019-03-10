using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public delegate void DeckChangedEventHandler(
		bool listChanged,
		bool countChanged,
		Card card,
		bool touchedChanged,
		Zone? changedZone,
		bool changeTerminatesBatch);
}