using Mtgdb.Dal;

namespace Mtgdb.Ui
{
	public delegate void CollectionChangedEventHandler(
		bool listChanged,
		bool countChanged,
		Card card);
}